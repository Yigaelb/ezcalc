using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using java.math;
using System.Numerics;

namespace EZCalc
{
    public class BigDecimalE : BigDecimal
    {
        public BigDecimalE() : base(0) {}
        public BigDecimalE(System.Numerics.BigInteger snbi) : base(snbi.ToString()) {}
        public BigDecimalE(int intVal) : base(intVal.ToString()) {}

        public BigDecimalE(string sval) : base(sval)
        {
        }

        public static BigDecimalE operator +(BigDecimalE c1, BigDecimalE c2)
        {
            return (BigDecimalE)c1.add(c2);
        }
        public static BigDecimalE operator /(BigDecimalE c1, BigDecimalE c2)
        {
            return (BigDecimalE)c1.divide(c2,ROUND_HALF_DOWN);
        }
        public static BigDecimalE operator -(BigDecimalE c1, BigDecimalE c2)
        {
            return (BigDecimalE)c1.subtract(c2);
        }
        public static BigDecimalE operator *(BigDecimalE c1, BigDecimalE c2)
        {
            return (BigDecimalE)c1.multiply(c2);
        }
        public static System.Numerics.BigInteger operator %(BigDecimalE c1, BigDecimalE c2)
        {
            System.Numerics.BigInteger bi1 = c1;
            System.Numerics.BigInteger bi2 = c2;
            return bi1 % bi2;
        }
        public static implicit operator System.Numerics.BigInteger (BigDecimalE instance)
        {
            return System.Numerics.BigInteger.Parse(instance.toBigInteger().ToString());
        }
        public static implicit operator BigDecimalE(System.Numerics.BigInteger instance)
        {
            return new BigDecimalE(instance);
        }
        public static implicit operator int(BigDecimalE instance)
        {
            return instance.intValue();
        }

        public static implicit operator BigDecimalE( int v)
        {
            return new BigDecimalE(v);
        }
    }
}
