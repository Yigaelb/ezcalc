/*----------------------------------------------------------------------------
Mathmatical Expression Parser
Copyright (C) 2002 Seth Williams
sethaw@yahoo.com

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
----------------------------------------------------------------------------*/
using System;
using BigDecimals;
using System.Text.RegularExpressions;
using System.Numerics;

namespace EZCalc
{
    /// <summary>Parses and Evaluates Mathmatical Expressions
	///
	/// </summary>
	/// <remarks>
	///
	/// Grammar for Parser in EBNF form:   <BR>
	///		note: some terms may not follow the typical <BR>
	///			naming conventions <BR><BR>
	///
	/// expression --> factor { ('+' | '-') expression}
	/// <BR>
	/// factor --> exp { ('*' | '/' | '%') exp}
	/// <BR>
	/// exp --> term { ('^' | '!') exp }
	/// <BR>
	/// term --> function | number | '(' expression ')' | '-' term
	/// <BR>
	/// function -->  functionName term | constantName
	///
	/// <BR>
	///  number --> digit {digit}
	/// <BR>
	/// word --> leter { letter }
	/// <BR>
	/// letter --> a | b | .. z | A | B | .. z
	/// <BR>
	/// digit --> 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 0
	/// <BR>
	/// functionName --> cos | sin | tan | sqrt | ...
	/// <BR>
	/// constant --> PI | g | c | ...
	/// <BR>
	/// </remarks>

	public class Expression
	{
        #region Class Variables
        public BigDecimal lastAnswer;
        private bool bDisplayHex;
        public bool DisplayHex
        {
            get
            {
                return bDisplayHex;
            }
            set
            {
                bDisplayHex = value;
            }
        }
        private int IntRemainderSize;
        public int RemainderDigits
        {
            get
            {
                return IntRemainderSize;
            }
            set
            {
                if (value < 10) IntRemainderSize = 10;
                else IntRemainderSize = value;
            }
        }
        private bool bDisplayBool;
        public bool DisplayBool
        {
            get
            {
                return bDisplayBool;
            }
            set
            {
                bDisplayBool = value;
            }
        }
        private bool bDisplayBoolDigits;
        public bool DisplayBoolDigits
        {
            get
            {
                return bDisplayBoolDigits;
            }
            set
            {
                bDisplayBoolDigits = value;
            }
        }
        private string strPreparsedCommand;
        public string PreParsedCommand
        {
            get
            {
                return strPreparsedCommand;
            }
        }
        private bool bWaitingLeftSubAdd;
        private bool bWaitingLeftDivMul;
        static BigDecimal bdZero = new BigDecimal("0");
        #endregion
        /// <summary>
        /// Tells the parser that all input angles will be in Radians
        /// </summary>
        public void useRadians()
		{
			angleFactor = 1;
		}
		/// <summary>
		/// Tells the parser that all input angles will be in Degrees
		/// </summary>
		public void useDegrees()
		{
			angleFactor = Math.PI / 180;
		}
		private double angleFactor;
		/// <summary>
		/// Computes the factorial of n
		/// </summary>
		static BigCSInteger factorial(int n)
		{
			BigCSInteger product= 1;
			for(int i=1; i <= n; ++i)
				product=product * i;
			return product;
		}
		/// <summary>
		/// Inputs a string of a Math expression and
		///   returns a result
		/// </summary>
		/// <remarks
		/// <param name="command">string of a Math expression and
		///   returns a result</param>
		/// <returns>If successsful returns the result of the
		/// expression.  Otherwise it returns a string
		/// descibing an error that occurred.</returns>
		public string ParseCommand(string command)
		{
			string returnValue;
            bWaitingLeftSubAdd = false;
            bWaitingLeftDivMul = false;
            bool bStart = true;
			command = command.Trim();
            command = PreParseCommand(command);
            try
            {
                while ( command.Length > 0 )
                {
                    if (!bStart)
                    {
                        if (command[0] == '(') command = "*" + command;
                        command = lastAnswer.ToString() + command;
                    }
                    lastAnswer = (ParseExpr(ref command));
                    bStart = false;
                }
                if (command.Length != 0)
                    throw new System.FormatException();
                returnValue = LimitRemainder(lastAnswer.ToString());
                returnValue = FixBDNegitive(returnValue);
            }
            catch (System.FormatException)
            {
                returnValue = "parse error! ";
            }
            catch (System.StackOverflowException)
            {
                returnValue = "error: Stack Overflow! ";
            }
            catch (System.DivideByZeroException)
            {
                returnValue = "error: Divide by Zero ";
            }
            catch (System.Exception)
            {
                returnValue = "parse error! ";
            }
			return returnValue;
		}
        public void GetOtherFormats(out string strHex, out string strBin)
        {
            string strLastValue;
            BigCSInteger BigIntReturn;
            int IntReturn = 0;

            strHex = "";
            strBin = "";

            try
            {
                if ((lastAnswer < 0) && (lastAnswer > -1)) return;
                else BigIntReturn = lastAnswer;
                strLastValue = lastAnswer.ToString();
                if ((BigIntReturn <= 1000000000) && (BigIntReturn > -1000000000))
                {
                    string strInt32 = RemoveRemainder(strLastValue);
                    IntReturn = int.Parse(strInt32);
                    DisplayHexAndBin(IntReturn, out strHex, out strBin);
                }
                else
                {
                    DisplayHexAndBin(BigIntReturn, out strHex, out strBin);
                }
            }
            catch (System.FormatException)
            {
                strHex += "\nparse error! ";
                strBin += "\nparse error! ";
            }
            catch (System.StackOverflowException)
            {
                strHex += "\nerror: Stack Overflow! ";
                strBin += "\nerror: Stack Overflow! ";
            }
            catch (System.DivideByZeroException)
            {
                strHex += "\nerror: Divide by Zero ";
                strBin += "\nerror: Divide by Zero ";
            }
            catch (System.Exception e)
            {
                strHex += "\nparse error! " + e.ToString();
                strBin += "\nparse error! " + e.ToString();
            }
            return;
        }
        private string PreParseCommand(string command)
        {
            string PreParsed = command;
            #region User Defined Expressions
            bool bFoundUsrDefExp = true;
            while (bFoundUsrDefExp)
            {
                bFoundUsrDefExp = false;
                for (int indxUD = 0; indxUD < Calculator.stnCrnt.NumUserDef; indxUD++)
                {
                    UserDefinedExpression ude = Calculator.stnCrnt.UsrDefExprs[indxUD];
                    if (!PreParsed.Contains(ude.Name)) continue;
                    string strRegExInput = @"(?<WithExp>" + ude.Name + @"\s*\((?<FuncInp>[^\(\)]+?)\))";
                    Match mtchInput = Regex.Match(PreParsed, strRegExInput);
                    if (!mtchInput.Success) continue;
                    ude.Input = mtchInput.Groups["FuncInp"].Value;
                    Match mtchFunc = Regex.Match(ude.Input, ude.InputRegEx);
                    if (!mtchFunc.Success) continue;
                    string strReplaceCommand = ude.Function;
                    for (int indxGrp = 1; indxGrp < mtchFunc.Groups.Count; indxGrp++)
                    {
                        while (strReplaceCommand.Contains("X" + indxGrp.ToString()))
                            strReplaceCommand = strReplaceCommand.Replace("X" + indxGrp.ToString(), mtchFunc.Groups[indxGrp].Value);
                    }
                    PreParsed = PreParsed.Replace(mtchInput.Groups["WithExp"].Value, strReplaceCommand);
                    bFoundUsrDefExp = true;
                }
            }
            #endregion
            PreParsed = PreParsed.Replace(" ", "");
            PreParsed = PreParsed.Replace(">>", ">");
            PreParsed = PreParsed.Replace("<<", "<");
            PreParsed = PreParsed.Replace("&&", "&");
            PreParsed = PreParsed.Replace("||", "|");
            PreParsed = PreParsed.Replace("ANS", "(ANS)");
            PreParsed = PreParsed.Replace("Ans", "(ANS)");
            PreParsed = PreParsed.Replace("ans", "(ANS)");
            strPreparsedCommand = PreParsed;
            return PreParsed;
        }
		private BigDecimal ParseExpr(ref string command)
		{
            BigDecimal op, op2;

			if (command == "")								// Handle the empty expression case
				return new BigDecimal("0");

			op = ParseFactor(ref command);					// parse left side of expression

			if (command != "")								// if a right side exists, parse it
			{

				if ((command[0] == '+')&&(!bWaitingLeftSubAdd))						// test for '+'
				{
					command = command.Substring(1,command.Length -1);	// skip to +

					if (command.Length == 0)
						throw new System.FormatException();		// no right hand side operator
                    bWaitingLeftSubAdd = true;
                    op2 = ParseExpr(ref command);				// parse remainder of the expression
                    bWaitingLeftSubAdd = false;
					op += op2;
				}
                else if ((command[0] == '-') && (!bWaitingLeftSubAdd))
				{
					command = command.Substring(1,command.Length -1);
					if (command.Length == 0)
						throw new System.FormatException();
                    bWaitingLeftSubAdd = true;
                    op2 = ParseExpr(ref command);
                    bWaitingLeftSubAdd = false;
					op -= op2;
				}
			}
			return op;
		}
        private BigDecimal ParseFactor(ref string command)
		{
            BigDecimal op, op2;
			op = ParseExp(ref command);
			while ((command != "") && 
                   (((command[0] == '*')&&(!bWaitingLeftDivMul))||
                    ((command[0] == '/' || command[0] == '\\') && (!bWaitingLeftDivMul)) ||
                    (command[0] == '%')))
			{
				if ((command[0] == '*')&&(!bWaitingLeftDivMul))
				{
					command = command.Substring(1,command.Length -1);
					if (command.Length == 0)
						throw new System.FormatException();
                    bWaitingLeftDivMul = true;
					op2 = ParseFactor(ref command);
                    bWaitingLeftDivMul = false;
					op *= op2;
				}
                else if ((command[0] == '/' || command[0] == '\\') && (!bWaitingLeftDivMul))
				{
					command = command.Substring(1,command.Length -1);
					if (command.Length == 0)
                        throw new System.FormatException();
                    bWaitingLeftDivMul = true;
					op2 = ParseFactor(ref command);
                    if (op2 == bdZero)									// don't allow divide 0
                        throw new System.DivideByZeroException();	// the division operation won't return
                    bWaitingLeftDivMul = false;
                    if (op == op2) op = 1;
                    else op = BigDecimalDevide(op, op2);							// throw the exception since we are using BigCSIntegers
				}
				else if (command[0] == '%')
				{
					command = command.Substring(1,command.Length -1);
					if (command.Length == 0)
						throw new System.FormatException();
					op2 = ParseFactor(ref command);
                    op = op % op2;
                }
			}
			return op;
		}
        private BigDecimal ParseExp(ref string command)
		{
            BigDecimal op, op2;
			op = ParseTerm(ref command);
			if (command != "")
			{
				if (command[0] == '!') // todo: is this the precedence of factorial?
				{
					command = command.Substring(1,command.Length -1);
                    int intOp = ((BigCSInteger)op).IntValue();
                    BigCSInteger bcsiOpFac = factorial(intOp);
                    return bcsiOpFac.ToBigDecimal();
				}
				else if (command[0] == '^')
				{
					command = command.Substring(1,command.Length -1);
					if (command.Length == 0)
						throw new System.FormatException();
					op2 = ParseExp(ref command);
                    //If result will be large, culculating using big integer
                    BigDecimal expoNoRem = RemoveRemainder(op2);
                    BigDecimal result = op;
                    double expFrec = DoubleValue(op2 - expoNoRem);
                    if (expoNoRem == 0) result = 1;
                    while (expoNoRem > 1)
                    {
                        result *= op;
                        expoNoRem -= 1;
                    }
                    double RsltFrec = Math.Pow(DoubleValue(op), expFrec);
                    op = result * (new BigDecimal(RsltFrec.ToString("F20")));
                }
                else if (command[0] == '>')
                {
                    command = command.Substring(1, command.Length - 1);
                    if (command.Length == 0)
                        throw new System.FormatException();
                    op2 = ParseExp(ref command);
                    BigCSInteger bcsi_op = new BigCSInteger(op.ToString(), 10);
                    bcsi_op = bcsi_op >> int.Parse(op2.ToString());
                    op = new BigDecimal(bcsi_op.ToString());
                }
                else if (command[0] == '<')
                {
                    command = command.Substring(1, command.Length - 1);
                    if (command.Length == 0)
                        throw new System.FormatException();
                    op2 = ParseExp(ref command);
                    BigCSInteger bcsi_op = new BigCSInteger(op.ToString(), 10);
                    bcsi_op = bcsi_op << int.Parse(op2.ToString());
                    op = new BigDecimal(bcsi_op.ToString());
                }
                else if (command[0] == '|')
                {
                    command = command.Substring(1, command.Length - 1);
                    if (command.Length == 0)
                        throw new System.FormatException();
                    op2 = ParseExp(ref command);
                    BigCSInteger bcsi_op = new BigCSInteger(op.ToString(), 10);
                    BigCSInteger bcsi_op2 = new BigCSInteger(op2.ToString(), 10);
                    op = new BigDecimal((bcsi_op | bcsi_op2).ToString());
                }
                else if (( (command[0] == 'X')||(command[0] == 'x')) &&
                          ( (command[1] == 'O')||(command[1] == 'o')) && 
                          ( (command[2] == 'R')||(command[2] == 'r')))
                {
                    command = command.Substring(3, command.Length - 3);
                    if (command.Length == 0)
                        throw new System.FormatException();
                    op2 = ParseExp(ref command);
                    BigCSInteger bcsi_op = new BigCSInteger(op.ToString(), 10);
                    BigCSInteger bcsi_op2 = new BigCSInteger(op2.ToString(), 10);
                    op = new BigDecimal((bcsi_op ^ bcsi_op2).ToString());
                }
                else if (command[0] == '&')
                {
                    command = command.Substring(1, command.Length - 1);
                    if (command.Length == 0)
                        throw new System.FormatException();
                    op2 = ParseExp(ref command);
                    BigCSInteger bcsi_op = new BigCSInteger(op.ToString(), 10);
                    BigCSInteger bcsi_op2 = new BigCSInteger(op2.ToString(), 10);
                    op = new BigDecimal((bcsi_op & bcsi_op2).ToString());
                }
			}
			return op;
		}
        private double ParseTermToDouble(ref string command)
        {
            BigDecimal result = ParseTerm(ref command);
            return double.Parse(result.ToString());
        }
        private BigDecimal ParseTerm(ref string command)
		{
            BigDecimal returnValue = 0;
			if (command.Length != 0)
			{
                if (string.Compare(command,0,"0x", 0,2,true) == 0 )
                { return new BigDecimal(ParseHexNumber(ref command).ToString()); }
                else if (string.Compare(command, 0, "0b", 0, 2, true) == 0)
                { return new BigDecimal(ParseBinNumber(ref command).ToString()); }
                else if (string.Compare(command, 0, "0h", 0, 2, true) == 0)
                { return new BigDecimal(ParseHexString(ref command).ToString()); }
				else if (char.IsDigit(command[0]))
				{ return ParseNumber(ref command);	}
				else if (char.IsLetter(command[0]) || ( command[0] == '~') )
				{ return ParseWord(ref command);}
				else if (command[0] == '-')				// handle unary '-' operator
				{
					command = command.Substring(1,command.Length -1);
                    return ParseTerm(ref command) * (-1);
				}
				else if (command[0] == '(')
				{
                    bool bRestoreWaitingLeftSubAdd = bWaitingLeftSubAdd;
                    bool bRestoreWaitingLeftDivMul = bWaitingLeftDivMul;
					command = command.Substring(1,command.Length -1);	// skip the open paren
                    bool bStart = true;
                    bWaitingLeftDivMul = false;
                    bWaitingLeftSubAdd = false;
                    while ( command.Length > 1)
                    {
                        if (command[0] == ')') break;

                        if (!bStart)
                        {
                            if (command[0] == '(') command = "*" + command;
                            command = returnValue.ToString() + command;
                        }
                        returnValue = ParseExpr(ref command);
                        bStart = false;
                    }
					if (command[0] != ')')								// make sure there is a close paren for each open parenthesis
						throw new System.FormatException();
					command = command.Substring(1,command.Length -1);	// skip the close paren
                    bWaitingLeftSubAdd = bRestoreWaitingLeftSubAdd;
                    bWaitingLeftDivMul = bRestoreWaitingLeftDivMul;
				}
				else
				{
					throw new System.FormatException();	// can't identify terminal
				}
			}
			else
			{
				throw new System.FormatException(); // don't allow empty string
			}
			return returnValue;
		}
        private BigDecimal ParseWord(ref string command)
		{
            double returnValue = 0;
            BigDecimal bdRetVal = 0;
            bool bReturnBigDec = false;
			if (command.Length != 0)
			{
				int i=0;
				string funcName="";
				while(i != command.Length && ( char.IsLetter(command[i]) || command[i] == '~') )	// build of string of letters
					funcName += command[i++];

				command = command.Substring(funcName.Length,command.Length -funcName.Length);
                funcName = funcName.ToLower();
				switch(funcName)
                {
                    case "cos":
                        returnValue = Math.Cos(ParseTermToDouble(ref command) * angleFactor);	 // calculate the cos, and adjust to degrees if needed
                        break;
                    case "sin":
                        returnValue = Math.Sin(ParseTermToDouble(ref command) * angleFactor);
                        break;
                    case "tan":
                        returnValue = Math.Tan(ParseTermToDouble(ref command) * angleFactor);
                        break;

                    case "acos":
                        returnValue = Math.Acos(ParseTermToDouble(ref command)) * (1.0 / angleFactor);
                        break;
                    case "asin":
                        returnValue = Math.Asin(ParseTermToDouble(ref command)) * (1.0 / angleFactor);
                        break;
                    case "atan":
                        returnValue = Math.Atan(ParseTermToDouble(ref command)) * (1.0 / angleFactor);
                        break;
                    case "log":
                        int baseInd = command.IndexOf('b');
                        int intBase = 10;
                        if ((baseInd > 0) && (baseInd < 3))
                        {
                            intBase = int.Parse(command.Substring(0, baseInd));
                            command = command.Substring(baseInd + 1);
                        }
                        returnValue = Math.Log(ParseTermToDouble(ref command), intBase);
                        break;
                    case "ln":
                        returnValue = Math.Log(ParseTermToDouble(ref command));
                        break;

                    case "floor":
                        returnValue = Math.Floor(ParseTermToDouble(ref command));
                        break;
                    case "ceil":
                        returnValue = Math.Ceiling(ParseTermToDouble(ref command));
                        break;
                    case "sqrt":
                        returnValue = Math.Sqrt(ParseTermToDouble(ref command));
                        break;
                    case "cosh":
                        returnValue = Math.Cosh(ParseTermToDouble(ref command));
                        break;
                    case "sinh":
                        returnValue = Math.Sinh(ParseTermToDouble(ref command));
                        break;
                    case "tanh":
                        returnValue = Math.Tanh(ParseTermToDouble(ref command));
                        break;
                    case "round":
                        returnValue = Math.Round(ParseTermToDouble(ref command));
                        break;
                    case "revbits":
                    case "reversebits":
                        bdRetVal = ParseTerm(ref command);
                        BigInteger biValue = BigInteger.Parse(bdRetVal.ToString());
                        BigInteger biNewValue = 0;
                        while( biValue > 0)
                        {
                            biNewValue *= 2;
                            int bitVal = (int)(biValue % 2);
                            biNewValue += bitVal;
                            biValue /= 2;
                        }
                        bdRetVal = new BigDecimal(biNewValue.ToString());
                        bReturnBigDec = true;
                        break;
                    case "revbytes":
                    case "reversebytes":
                        bdRetVal = ParseTerm(ref command);
                        biValue = BigInteger.Parse(bdRetVal.ToString());
                        biNewValue = 0;
                        while (biValue > 0)
                        {
                            biNewValue <<= 8;
                            int bitVal = (int)(biValue % 0x100);
                            biNewValue += bitVal;
                            biValue >>= 8;
                        }
                        bdRetVal = new BigDecimal(biNewValue.ToString());
                        bReturnBigDec = true;
                        break;
                    case "revnibles":
                    case "revversenibles":
                        bdRetVal = ParseTerm(ref command);
                        biValue = BigInteger.Parse(bdRetVal.ToString());
                        biNewValue = 0;
                        while (biValue > 0)
                        {
                            biNewValue <<= 4;
                            int bitVal = (int)(biValue % 0x10);
                            biNewValue += bitVal;
                            biValue >>= 4;
                        }
                        bdRetVal = new BigDecimal(biNewValue.ToString());
                        bReturnBigDec = true;
                        break;
                    case "ascii":
                    case "asci":
                        Match mtAsci = Regex.Match(command, @"\((?<ascii>\w*)\)");
                        string strAscii = mtAsci.Groups["ascii"].Value;
                        byte[] asciiBytes = System.Text.Encoding.ASCII.GetBytes(strAscii);
                        Int64 intVal = 0;
                        for( int indByte = 0; indByte < asciiBytes.Length; indByte++)
                        {
                            intVal <<= 8;
                            intVal += asciiBytes[indByte];
                        }
                        command = command.Replace(strAscii, intVal.ToString());
                        returnValue = ParseTermToDouble(ref command);
                        break;
                    case "rand":
                    case "random":
                    case "rnd":
                        Random r = new Random();
                        returnValue = r.Next((int)ParseTermToDouble(ref command));
                        break;
                    case "abs":
                        returnValue = Math.Abs(ParseTermToDouble(ref command));
                        break;
                    case "~":
                        uint temp = (uint)ParseTermToDouble(ref command);
                        temp = ~temp;
                        returnValue = (double)temp;
                        break;
                    // Constants
                    case "e": // exponent
                        returnValue = Math.E;
                        break;
                    case "pi": // pi
                        returnValue = Math.PI;
                        break;
                    case "g":	// acceleration of gravity on earth (m/s)
                        returnValue = 9.80665;
                        break;
                    case "r":	// Gas Las constant - J / (mol * k)
                        returnValue = 9.80665;
                        break;
                    case "c":	// Speed of Light in vacuum (m/s)
                        returnValue = 2.9979;
                        break;
                    case "ans":
                        return lastAnswer;
					default:
						throw new System.FormatException();
				}
			}
            if (bReturnBigDec) return bdRetVal;
			return new BigDecimal(returnValue.ToString("F20"));
		}
        private BigDecimal ParseNumber(ref string command)
		{
			bool foundDecimal=false;
			string temp="";
			int i=0;

			// build a string with the number
			while(i!=command.Length && (char.IsDigit(command[i]) || command[i]=='.'))
			{
				if (command[i]=='.')
				{
					if (!foundDecimal)
						foundDecimal =true;
					else
						throw new System.FormatException();	// only allow one decimal point per number
				}
				temp += command[i++];
			}
			if (temp[0] == '.' || temp[temp.Length -1] == '.')
				throw new System.FormatException(); // don't allow the first last char to be a decimal point

			command = command.Substring(i,command.Length -i);
			return new BigDecimal(temp);
		}
        private bool IsHexDigit(char c)
        {
            if ((c >= '0') && (c <= '9')) return true;
            if ((char.ToLower(c) >= 'a') && (char.ToLower(c) <= 'f')) return true;
            return false;
        }
        private BigCSInteger ParseHexNumber(ref string command)
        {
            string temp = "";
            command = command.ToUpper();
            int i = 2;
            BigCSInteger result;

            // build a string with the number
            while (i != command.Length && (IsHexDigit(command[i]) || (command[i] == '.')))
            {
                if ((command[i] == '.') || (command[i] > 'F'))
                {
                    throw new System.FormatException();	// only allow one decimal point per number
                }
                temp += command[i++];
            }
            if (temp[0] == '.' || temp[temp.Length - 1] == '.')
                throw new System.FormatException(); // don't allow the first last char to be a decimal point

            command = command.Substring(i, command.Length - i);
            result = new BigCSInteger(temp, 16);
            return (BigCSInteger)result;
        }
        private BigCSInteger ParseHexString(ref string command)
        {
            string temp = "";
            command = command.ToUpper();
            int i = 2;
            BigCSInteger result;

            // build a string with the number
            while (i != command.Length && (IsHexDigit(command[i]) || command[i] == '.'))
            {
                if ((command[i] == '.') || (command[i] > 'F'))
                {
                    throw new System.FormatException();	// only allow one decimal point per number
                }
                temp += command[i++];
            }
            if (temp[0] == '.' || temp[temp.Length - 1] == '.')
                throw new System.FormatException(); // don't allow the first last char to be a decimal point

            command = command.Substring(i, command.Length - i);

            #region reversing string
            string tmpRev = temp;
            if ((tmpRev.Length % 2) != 0) tmpRev = "0" + tmpRev;
            temp = "";
            for (i = 2; i <= tmpRev.Length; i += 2)
            {
                temp += tmpRev.Substring(tmpRev.Length - i, 2);
            }
            #endregion

            result = new BigCSInteger(temp, 16);
            return (BigCSInteger)result;
        }
        private BigCSInteger ParseBinNumber(ref string command)
        {
            Regex ex = new Regex(@"\[\d{1,10}]");
            command = ex.Replace(command, "");
            string temp = "";
            command = command.ToUpper();
            int i = 2;
            BigCSInteger result;

            // build a string with the number
            while ((i != command.Length) && ( (command[i] == '0') || (command[i] == '1') ) )
            {
                temp += command[i++];
            }

            command = command.Substring(i, command.Length - i);
            result = new BigCSInteger(temp, 2);
            return (BigCSInteger)result;
        }
        public static double DoubleValue(BigDecimal bd)
        {
            return double.Parse(bd.ToString());
        }
        public static decimal DecimalValue(BigDecimal bd)
        {
            return decimal.Parse(bd.ToString());
        }
        public static BigDecimal BigDecimalValue(double d)
        {
            return new BigDecimal(d.ToString("F20"));
        }
        public BigDecimal RemoveRemainder(BigDecimal bd)
        {
            string strD = bd.ToString();
            int index = strD.IndexOf('.');
            if (index < 0) return bd;
            else strD = strD.Substring(0, index);
            return new BigDecimal(strD);
        }
        public string RemoveRemainder(string val)
        {
            int index = val.IndexOf('.');
            if (index < 0) return val;
            else val = val.Substring(0, index);
            return val;
        }
        private string LimitRemainder(string val)
        {
            int index = val.IndexOf('.');
            if (index < 0) return val;
            if (val.Length > (index + IntRemainderSize + 1)) return val.Substring(0, index + IntRemainderSize + 1);
            else return val;
        }
        private string FixBDNegitive(string val)
        {
            if(val.Contains(".-"))
            {
                val = "-"+val.Replace(".-",".");
            }
            return val;
        }
        private string DisplayHexAndBin(BigCSInteger val)
        {
            string returnString = "";
            if (DisplayHex && ((val > 9) || (val < -9))) returnString += "   (0x" + val.ToHexString() + ")";
            if (DisplayBool && ((val > 1) || (val < -1)))
            {
                int ind = 0, nibble = 0;
                string strBoolReslt = val.ToString(2);
                bool isNegetive = false;
                if (strBoolReslt[0] == '-')
                {
                    isNegetive = true;
                    strBoolReslt = strBoolReslt.Remove(0, 1);
                }
                int zeroPading = (4 - (strBoolReslt.Length % 4)) % 4;
                for (ind = 0; ind < zeroPading; ind++) strBoolReslt = "0" + strBoolReslt;
                ind = 0;
                int NumInserts = strBoolReslt.Length / 4;
                if ((strBoolReslt.Length > 4) && (DisplayBoolDigits))
                    {
                    while (ind < strBoolReslt.Length)
                    {
                        string strNible = "[" + nibble.ToString() + "] ";
                        strBoolReslt = strBoolReslt.Insert(strBoolReslt.Length - (ind), strNible);
                        ind += 4 + strNible.Length;
                        nibble += 4;
                    }
                }
                if (isNegetive) strBoolReslt = "-" + strBoolReslt;
                if (val > 0xFFFFFFFF) returnString += "\n";
                returnString += "   (0b" + strBoolReslt + ")";
            }
            return returnString;
        }
        private string DisplayHexAndBin(Int64 val)
        {
            string returnString = "";
            if (DisplayHex && ((val > 9) || (val < -9))) returnString += "   (0x" + val.ToString("X") + ")";
            if (DisplayBool && ((val > 1) || (val < -1)))
            {
                int ind = 0, nibble = 0;
                string strBoolReslt = Convert.ToString(val, 2);
                int zeroPading = (4 - (strBoolReslt.Length % 4)) % 4;
                for (ind = 0; ind < zeroPading; ind++) strBoolReslt = "0" + strBoolReslt;
                ind = 0;
                int NumInserts = strBoolReslt.Length / 4;
                if ( (strBoolReslt.Length > 4) && ( DisplayBoolDigits ) )
                {
                    while (ind < strBoolReslt.Length)
                    {
                        string strNible = "[" + nibble.ToString() + "] ";
                        strBoolReslt = strBoolReslt.Insert(strBoolReslt.Length - (ind), strNible);
                        ind += 4 + strNible.Length;
                        nibble += 4;
                    }
                }
                if (val > 0xFFFFFFFF) returnString += "\n";
                returnString += "   (0b" + strBoolReslt + ")";
            }
            return returnString;
        }
        private void DisplayHexAndBin(BigCSInteger val, out string strHex, out string strBin)
        {
            strHex = "";
            strBin = "";
            if (DisplayHex ) strHex += "(0x" + val.ToHexString() + ")";
            if (DisplayBool )
            {
                int ind = 0, nibble = 0;
                string strBoolReslt = val.ToString(2);
                int zeroPading = (4 - (strBoolReslt.Length % 4)) % 4;
                for (ind = 0; ind < zeroPading; ind++) strBoolReslt = "0" + strBoolReslt;
                ind = 0;
                int NumInserts = strBoolReslt.Length / 4;
                if ((strBoolReslt.Length > 4) && (DisplayBoolDigits))
                {
                    while (ind < strBoolReslt.Length)
                    {
                        string strNible = "[" + nibble.ToString() + "] ";
                        strBoolReslt = strBoolReslt.Insert(strBoolReslt.Length - (ind), strNible);
                        ind += 4 + strNible.Length;
                        nibble += 4;
                    }
                }
                strBin += "(0b" + strBoolReslt + ")";
            }
            return ;
        }
        private BigDecimal BigDecimalDevide(BigDecimal op, BigDecimal op2)
        {
             return BigDecimalDevideDoubleFix(op, op2);
        }
        private BigDecimal BigDecimalDevideDoubleFix(BigDecimal op, BigDecimal op2)
        {
            bool bFixed = true;
            double dOp, dOp2, dResult, dResultFromBD;
            BigDecimal result;

            result = op / op2;
            dResultFromBD = Math.Abs(DoubleValue(result));
            dOp = DoubleValue(op);
            dOp2 = DoubleValue(op2);
            dResult = Math.Abs(dOp / dOp2);
            while (bFixed)
            {
                bFixed = false;
                if (dResult < (dResultFromBD - (dResultFromBD / 100)))
                {
                    result /= 10;
                    dResultFromBD = Math.Abs(DoubleValue(result));
                    bFixed = true;
                }
                if (dResult > (dResultFromBD + (dResultFromBD / 100)))
                {
                    result *= 10;
                    dResultFromBD = Math.Abs(DoubleValue(result));
                    bFixed = true;
                }
            }
            return result;
        }
        /* BigDecimalDevideDecimalFix works incorrectly because of a bug in c# decimal type.
         * This is also probably the reason BigDecimal works incorrectly.
        private BigDecimal BigDecimalDevideDecimalFix(BigDecimal op, BigDecimal op2)
        {
            decimal dOp, dOp2, dResult, dResultFromBD;
            BigDecimal result;

            result = op / op2;
            dResultFromBD = DecimalValue(result);
            dOp = DecimalValue(op);
            dOp2 = DecimalValue(op2);
            dResult = dOp / dOp2;
            if (dResult < (dResultFromBD + (dResultFromBD / 100)))
            {
                result /= 10;
                dResultFromBD = DecimalValue(result);
            }
            if (dResult > (dResultFromBD - (dResultFromBD / 100)))
            {
                result *= 10;
                dResultFromBD = DecimalValue(result);
            }
            return result;
        }*/
	}
}
