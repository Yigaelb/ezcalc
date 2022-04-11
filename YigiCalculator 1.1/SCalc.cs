using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using BigDecimals;
using System.Threading;
using System.Text.RegularExpressions;



namespace EZCalc
{
	public class Calculator : System.Windows.Forms.Form
    {
        #region Version, version date, contact, notes
        private Version version = new Version(4, 23);
        private string strContact = "yigaelbennatan@outlook.com";
        private string strRepo = "https://github.com/Yigaelb/ezcalc.git";
        private string[] versionNotes = {@"2022-04-11 Added to git hub 4.23",
                                         @"2016-05-25 Fixed x^0 4.22",
                                         @"2016-01-25 Fixed coloring and spacing of display 4.21",
                                         @"2016-01-22 Several Answers in one line 4.20",
                                         @"2016-01-22 Reverse bits, bytes nibbles and Command sub menu reorg. Build 4.12",
                                         @"2016-01-22 Form Centers when out of screen. Build 4.12",
                                         @"2015-12-23 Added ascii. Build 4.11",
                                         @"2015-10-25 Added Random rand/rnd/random",
                                         @"2015-08-11 Fixed bug - User defined expressions. ANSXOR fix.",
                                         @"2015-07-27 Fixed bug - Fixed parse of hex numbers.",
                                         @"2015-07-21 Fixed bug - ANS not written before % in new line.",
                                         @"2015-07-09 Fixed bug - DisplayHexAndBin(Int64 val) bin digits.",
                                         @"2014-07-31 Added NOT.",
                                         @"2012-08-30 Added XOR.",
                                         @"2012-08-09 Added '0h' button with tool tip.",
                                         @"2012-06-05 Boolian input ignores \[\d{1,10}].",
                                         "2012-05-08 Bug fixes and color handling change.",
                                         "2012-05-07 White on black colors.",
                                         "2012-04-30 Activating main form after load form closes.",
                                         "2012-04-22 Added loading form.",
                                         "2012-04-19 Cnt+Tab and Cnt+Shift+Tab for navagating windows. Windows Optional.",
                                         "2012-04-18 4 Windows.",
                                         "2012-04-17 Bin Digits.",
                                         "2012-04-09 better handling of empty lines.",
                                         "2012-04-09 when pasting text, removed \r and \n",
                                         "2012-04-03 when pasting text, formating and spaces removed",
                                         "2012-04-01 Channed value from which to use BigInt to parse to 1000000000",
                                         "2012-03-26 Fixed Multiple '*,/,%' error",
                                         "2012-03-25 Added backspace key",
                                         "2012-03-25 Added log with different bases",
                                         "2012-03-13 Minus Div Fix, plus extra keys",
                                         "2012-03-05 Spelling fix Display",
                                         "2012-03-03 Fixed BigDecimal Devide, build 3.20" };
        private DateTime verDate = new DateTime(2016, 5, 25, 8, 0, 0);
        #endregion
        #region class variables
        private const int NumUsrDefManuItemsBeforeExprssions = 3;
        private const int cnstNumWin = 4;
        private const int MaxUserDefExp = 40;
        private Button[] abtnWindows = new Button[cnstNumWin];
        private BigDecimal[] abdLastAnswer = new BigDecimal[cnstNumWin];
        private bool CursorToEndOnKeyUp = false;
        public static Properties.Settings stnCrnt = Properties.Settings.Default;
        private bool BreakLoop = false;
        private string lastInput = "", CurrentResult = "", CurrentCommand;
        private int ProcessStart, ProcessEnd, intMaxOperationTime = 2;
        public static CalculatorState State = CalculatorState.Initializing;
        private frmLoading frmLoad;
        Thread thrdLoading;
        ColorTheme clrthInactive = new ColorTheme();
        ColorTheme clrthActive = new ColorTheme();
        public int MaxOperationTime
        {
            get
            {
                return intMaxOperationTime;
            }
            set
            {
                if (value < 1) intMaxOperationTime = 1;
                else intMaxOperationTime = value;
            }
        }
        #endregion
        #region Controls
        private Expression expression;
        private RichTextBox rtb_Display;
        private Button btn_Clear;
        private CheckBox cb_HexDisplay;
        private CheckBox cb_BoolDisplay;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem commandsToolStripMenuItem;
        private ToolStripMenuItem aNSToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem fontSizeToolStripMenuItem;
        private FontDialog fd_Display;
        private ToolStripMenuItem lastInputToolStripMenuItem;
        private ToolStripMenuItem saveTextForNextRunToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private Button btn_0;
        private Button btn_1;
        private Button btn_2;
        private Button btn_3;
        private Button btn_4;
        private Button btn_5;
        private Button btn_6;
        private Button btn_7;
        private Button btn_8;
        private Button btn_9;
        private Button btn_dot;
        private Button btn_div;
        private Button btn_mul;
        private Button btn_min;
        private Button btn_plus;
        private Button btn_equ;
        private Button btn_mod;
        private ToolStripMenuItem showNumberPadToolStripMenuItem;
        private Button btn_ans;
        private Button btn_lastInput;
        private CheckBox cb_showPad;
        private Button btn_sftRight;
        private Button btn_sftLeft;
        private Button btn_or;
        private Button btn_and;
        private ToolStripMenuItem limitDigitsAfter0ToToolStripMenuItem;
        private ToolStripComboBox toolStripRemainderDigits;
        private ToolStripMenuItem maxOperationTimeInSeconds;
        private ToolStripComboBox toolStripCmbTimeOut;
        private Button btn_B;
        private Button btn_A;
        private Button btn_D;
        private Button btn_C;
        private Button btn_F;
        private Button btn_E;
        private Button btn_0x;
        private Button btn_0b;
        private Button btn_BackSpace;
        private CheckBox cb_BinDigits;
        private Label lblDisplay;
        private Button btn_Win4;
        private Button btn_Win3;
        private Button btn_Win2;
        private Button btn_Win1;
        private ToolStripMenuItem enableWindowKeysToolStripMenuItem;
        private ToolStripMenuItem colorThemToolStripMenuItem;
        private Button btn_0h;
        #endregion       
        private ToolTip tlTpCalc;
        private Button btn_xor;
        private Button btn_not;
        private ToolStripMenuItem tlItmUD;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem exportImportToolStripMenuItem;
        private ToolStripMenuItem importToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
        private OpenFileDialog ofdUserDefined;
        private SaveFileDialog sfdUserDefined;
        private ToolStripMenuItem tsShowPreParsed;
        private ToolStripMenuItem mathSubMenu;
        private ToolStripMenuItem cosToolStripMenuItem1;
        private ToolStripMenuItem sinToolStripMenuItem1;
        private ToolStripMenuItem tanToolStripMenuItem1;
        private ToolStripMenuItem acosToolStripMenuItem1;
        private ToolStripMenuItem asinToolStripMenuItem1;
        private ToolStripMenuItem atanToolStripMenuItem1;
        private ToolStripMenuItem log2bToolStripMenuItem1;
        private ToolStripMenuItem logToolStripMenuItem1;
        private ToolStripMenuItem lnToolStripMenuItem1;
        private ToolStripMenuItem floorToolStripMenuItem1;
        private ToolStripMenuItem ceilToolStripMenuItem1;
        private ToolStripMenuItem sqrtToolStripMenuItem1;
        private ToolStripMenuItem coshToolStripMenuItem1;
        private ToolStripMenuItem sinhToolStripMenuItem1;
        private ToolStripMenuItem tanhToolStripMenuItem1;
        private ToolStripMenuItem roundToolStripMenuItem1;
        private ToolStripMenuItem absToolStripMenuItem1;
        private ToolStripMenuItem Constants;
        private ToolStripMenuItem bitwiseToolMenuItem;
        private ToolStripMenuItem eToolStripMenuItem1;
        private ToolStripMenuItem pIToolStripMenuItem1;
        private ToolStripMenuItem gToolStripMenuItem1;
        private ToolStripMenuItem rToolStripMenuItem1;
        private ToolStripMenuItem revBitsToolStripMenuItem;
        private ToolStripMenuItem revBytesToolStripMenuItem1;
        private ToolStripMenuItem revNiblesToolStripMenuItem;
        private ToolStripMenuItem asciToolStripMenuItem1;
        private IContainer components;
    
        public Calculator()
		{
            thrdLoading = new Thread(new ThreadStart(loadingWindow));
            thrdLoading.Start();
			InitializeComponent();
			rtb_Display.Focus();
            rtb_Display.TabIndex = 0;
			expression = new Expression();
			expression.useRadians();
			expression.lastAnswer = new BigDecimal("0");
		}
        private void loadingWindow()
        {
            this.WindowState = FormWindowState.Minimized;
            frmLoad = new frmLoading( );
            frmLoad.ShowDialog();
        }
        #region Generated Code
        protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
        /// <summary>The main entry point for the application.
		/// 
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Calculator());
		}
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Calculator));
            this.rtb_Display = new System.Windows.Forms.RichTextBox();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.cb_HexDisplay = new System.Windows.Forms.CheckBox();
            this.cb_BoolDisplay = new System.Windows.Forms.CheckBox();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.commandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aNSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastInputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mathSubMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.cosToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sinToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tanToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.acosToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.asinToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.atanToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.log2bToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.lnToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.floorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ceilToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sqrtToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.coshToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sinhToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tanhToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.roundToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.absToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTextForNextRunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.limitDigitsAfter0ToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripRemainderDigits = new System.Windows.Forms.ToolStripComboBox();
            this.maxOperationTimeInSeconds = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripCmbTimeOut = new System.Windows.Forms.ToolStripComboBox();
            this.colorThemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showNumberPadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableWindowKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tlItmUD = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsShowPreParsed = new System.Windows.Forms.ToolStripMenuItem();
            this.fd_Display = new System.Windows.Forms.FontDialog();
            this.btn_0 = new System.Windows.Forms.Button();
            this.btn_1 = new System.Windows.Forms.Button();
            this.btn_2 = new System.Windows.Forms.Button();
            this.btn_3 = new System.Windows.Forms.Button();
            this.btn_4 = new System.Windows.Forms.Button();
            this.btn_5 = new System.Windows.Forms.Button();
            this.btn_6 = new System.Windows.Forms.Button();
            this.btn_7 = new System.Windows.Forms.Button();
            this.btn_8 = new System.Windows.Forms.Button();
            this.btn_9 = new System.Windows.Forms.Button();
            this.btn_dot = new System.Windows.Forms.Button();
            this.btn_div = new System.Windows.Forms.Button();
            this.btn_mul = new System.Windows.Forms.Button();
            this.btn_min = new System.Windows.Forms.Button();
            this.btn_plus = new System.Windows.Forms.Button();
            this.btn_equ = new System.Windows.Forms.Button();
            this.btn_mod = new System.Windows.Forms.Button();
            this.btn_ans = new System.Windows.Forms.Button();
            this.btn_lastInput = new System.Windows.Forms.Button();
            this.cb_showPad = new System.Windows.Forms.CheckBox();
            this.btn_sftRight = new System.Windows.Forms.Button();
            this.btn_sftLeft = new System.Windows.Forms.Button();
            this.btn_or = new System.Windows.Forms.Button();
            this.btn_and = new System.Windows.Forms.Button();
            this.btn_B = new System.Windows.Forms.Button();
            this.btn_A = new System.Windows.Forms.Button();
            this.btn_D = new System.Windows.Forms.Button();
            this.btn_C = new System.Windows.Forms.Button();
            this.btn_F = new System.Windows.Forms.Button();
            this.btn_E = new System.Windows.Forms.Button();
            this.btn_0x = new System.Windows.Forms.Button();
            this.btn_0b = new System.Windows.Forms.Button();
            this.btn_BackSpace = new System.Windows.Forms.Button();
            this.cb_BinDigits = new System.Windows.Forms.CheckBox();
            this.lblDisplay = new System.Windows.Forms.Label();
            this.btn_Win4 = new System.Windows.Forms.Button();
            this.btn_Win3 = new System.Windows.Forms.Button();
            this.btn_Win2 = new System.Windows.Forms.Button();
            this.btn_Win1 = new System.Windows.Forms.Button();
            this.btn_0h = new System.Windows.Forms.Button();
            this.tlTpCalc = new System.Windows.Forms.ToolTip(this.components);
            this.btn_xor = new System.Windows.Forms.Button();
            this.btn_not = new System.Windows.Forms.Button();
            this.ofdUserDefined = new System.Windows.Forms.OpenFileDialog();
            this.sfdUserDefined = new System.Windows.Forms.SaveFileDialog();
            this.Constants = new System.Windows.Forms.ToolStripMenuItem();
            this.bitwiseToolMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pIToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.gToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.revBitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revBytesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.revNiblesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asciToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtb_Display
            // 
            this.rtb_Display.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rtb_Display.Location = new System.Drawing.Point(14, 55);
            this.rtb_Display.Name = "rtb_Display";
            this.rtb_Display.Size = new System.Drawing.Size(405, 247);
            this.rtb_Display.TabIndex = 0;
            this.rtb_Display.Text = "";
            this.rtb_Display.TextChanged += new System.EventHandler(this.rtb_Display_TextChanged);
            this.rtb_Display.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtb_Display_KeyDown);
            this.rtb_Display.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rtb_Display_KeyPress);
            this.rtb_Display.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rtb_Display_KeyUp);
            // 
            // btn_Clear
            // 
            this.btn_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Clear.Location = new System.Drawing.Point(11, 464);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(75, 23);
            this.btn_Clear.TabIndex = 1;
            this.btn_Clear.Text = "Clear";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // cb_HexDisplay
            // 
            this.cb_HexDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_HexDisplay.AutoSize = true;
            this.cb_HexDisplay.Location = new System.Drawing.Point(153, 468);
            this.cb_HexDisplay.Name = "cb_HexDisplay";
            this.cb_HexDisplay.Size = new System.Drawing.Size(45, 17);
            this.cb_HexDisplay.TabIndex = 2;
            this.cb_HexDisplay.Text = "Hex";
            this.cb_HexDisplay.UseVisualStyleBackColor = true;
            this.cb_HexDisplay.CheckedChanged += new System.EventHandler(this.cb_HexDisplay_CheckedChanged);
            // 
            // cb_BoolDisplay
            // 
            this.cb_BoolDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_BoolDisplay.AutoSize = true;
            this.cb_BoolDisplay.Location = new System.Drawing.Point(204, 468);
            this.cb_BoolDisplay.Name = "cb_BoolDisplay";
            this.cb_BoolDisplay.Size = new System.Drawing.Size(41, 17);
            this.cb_BoolDisplay.TabIndex = 2;
            this.cb_BoolDisplay.Text = "Bin";
            this.cb_BoolDisplay.UseVisualStyleBackColor = true;
            this.cb_BoolDisplay.CheckedChanged += new System.EventHandler(this.cb_BoolDisplay_CheckedChanged);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commandsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.tlItmUD,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(451, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // commandsToolStripMenuItem
            // 
            this.commandsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aNSToolStripMenuItem,
            this.lastInputToolStripMenuItem,
            this.mathSubMenu,
            this.Constants,
            this.bitwiseToolMenuItem});
            this.commandsToolStripMenuItem.Name = "commandsToolStripMenuItem";
            this.commandsToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.commandsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.commandsToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.commandsToolStripMenuItem.Text = "Commands";
            // 
            // aNSToolStripMenuItem
            // 
            this.aNSToolStripMenuItem.Name = "aNSToolStripMenuItem";
            this.aNSToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.aNSToolStripMenuItem.Text = "ANS";
            this.aNSToolStripMenuItem.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // lastInputToolStripMenuItem
            // 
            this.lastInputToolStripMenuItem.Name = "lastInputToolStripMenuItem";
            this.lastInputToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.lastInputToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.lastInputToolStripMenuItem.Text = "Last Input";
            this.lastInputToolStripMenuItem.Click += new System.EventHandler(this.lastInputToolStripMenuItem_Click);
            // 
            // mathSubMenu
            // 
            this.mathSubMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cosToolStripMenuItem1,
            this.sinToolStripMenuItem1,
            this.tanToolStripMenuItem1,
            this.acosToolStripMenuItem1,
            this.asinToolStripMenuItem1,
            this.atanToolStripMenuItem1,
            this.log2bToolStripMenuItem1,
            this.logToolStripMenuItem1,
            this.lnToolStripMenuItem1,
            this.floorToolStripMenuItem1,
            this.ceilToolStripMenuItem1,
            this.sqrtToolStripMenuItem1,
            this.coshToolStripMenuItem1,
            this.sinhToolStripMenuItem1,
            this.tanhToolStripMenuItem1,
            this.roundToolStripMenuItem1,
            this.absToolStripMenuItem1});
            this.mathSubMenu.Name = "mathSubMenu";
            this.mathSubMenu.Size = new System.Drawing.Size(175, 22);
            this.mathSubMenu.Text = "Math";
            // 
            // cosToolStripMenuItem1
            // 
            this.cosToolStripMenuItem1.Name = "cosToolStripMenuItem1";
            this.cosToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.cosToolStripMenuItem1.Text = "cos";
            this.cosToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // sinToolStripMenuItem1
            // 
            this.sinToolStripMenuItem1.Name = "sinToolStripMenuItem1";
            this.sinToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.sinToolStripMenuItem1.Text = "sin";
            this.sinToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // tanToolStripMenuItem1
            // 
            this.tanToolStripMenuItem1.Name = "tanToolStripMenuItem1";
            this.tanToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.tanToolStripMenuItem1.Text = "tan";
            this.tanToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // acosToolStripMenuItem1
            // 
            this.acosToolStripMenuItem1.Name = "acosToolStripMenuItem1";
            this.acosToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.acosToolStripMenuItem1.Text = "acos";
            this.acosToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // asinToolStripMenuItem1
            // 
            this.asinToolStripMenuItem1.Name = "asinToolStripMenuItem1";
            this.asinToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.asinToolStripMenuItem1.Text = "asin";
            this.asinToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // atanToolStripMenuItem1
            // 
            this.atanToolStripMenuItem1.Name = "atanToolStripMenuItem1";
            this.atanToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.atanToolStripMenuItem1.Text = "atan";
            this.atanToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // log2bToolStripMenuItem1
            // 
            this.log2bToolStripMenuItem1.Name = "log2bToolStripMenuItem1";
            this.log2bToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.log2bToolStripMenuItem1.Text = "log2b";
            this.log2bToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // logToolStripMenuItem1
            // 
            this.logToolStripMenuItem1.Name = "logToolStripMenuItem1";
            this.logToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.logToolStripMenuItem1.Text = "log";
            this.logToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // lnToolStripMenuItem1
            // 
            this.lnToolStripMenuItem1.Name = "lnToolStripMenuItem1";
            this.lnToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.lnToolStripMenuItem1.Text = "ln";
            this.lnToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // floorToolStripMenuItem1
            // 
            this.floorToolStripMenuItem1.Name = "floorToolStripMenuItem1";
            this.floorToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.floorToolStripMenuItem1.Text = "floor";
            this.floorToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // ceilToolStripMenuItem1
            // 
            this.ceilToolStripMenuItem1.Name = "ceilToolStripMenuItem1";
            this.ceilToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.ceilToolStripMenuItem1.Text = "ceil";
            this.ceilToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // sqrtToolStripMenuItem1
            // 
            this.sqrtToolStripMenuItem1.Name = "sqrtToolStripMenuItem1";
            this.sqrtToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.sqrtToolStripMenuItem1.Text = "sqrt";
            this.sqrtToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // coshToolStripMenuItem1
            // 
            this.coshToolStripMenuItem1.Name = "coshToolStripMenuItem1";
            this.coshToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.coshToolStripMenuItem1.Text = "cosh";
            this.coshToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // sinhToolStripMenuItem1
            // 
            this.sinhToolStripMenuItem1.Name = "sinhToolStripMenuItem1";
            this.sinhToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.sinhToolStripMenuItem1.Text = "sinh";
            this.sinhToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // tanhToolStripMenuItem1
            // 
            this.tanhToolStripMenuItem1.Name = "tanhToolStripMenuItem1";
            this.tanhToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.tanhToolStripMenuItem1.Text = "tanh";
            this.tanhToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // roundToolStripMenuItem1
            // 
            this.roundToolStripMenuItem1.Name = "roundToolStripMenuItem1";
            this.roundToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.roundToolStripMenuItem1.Text = "round";
            this.roundToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // absToolStripMenuItem1
            // 
            this.absToolStripMenuItem1.Name = "absToolStripMenuItem1";
            this.absToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.absToolStripMenuItem1.Text = "abs";
            this.absToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontSizeToolStripMenuItem,
            this.saveTextForNextRunToolStripMenuItem,
            this.limitDigitsAfter0ToToolStripMenuItem,
            this.maxOperationTimeInSeconds,
            this.colorThemToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // fontSizeToolStripMenuItem
            // 
            this.fontSizeToolStripMenuItem.Name = "fontSizeToolStripMenuItem";
            this.fontSizeToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.fontSizeToolStripMenuItem.Text = "Font";
            this.fontSizeToolStripMenuItem.Click += new System.EventHandler(this.fontSizeToolStripMenuItem_Click);
            // 
            // saveTextForNextRunToolStripMenuItem
            // 
            this.saveTextForNextRunToolStripMenuItem.CheckOnClick = true;
            this.saveTextForNextRunToolStripMenuItem.Name = "saveTextForNextRunToolStripMenuItem";
            this.saveTextForNextRunToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.saveTextForNextRunToolStripMenuItem.Text = "Save text for next run";
            // 
            // limitDigitsAfter0ToToolStripMenuItem
            // 
            this.limitDigitsAfter0ToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripRemainderDigits});
            this.limitDigitsAfter0ToToolStripMenuItem.Name = "limitDigitsAfter0ToToolStripMenuItem";
            this.limitDigitsAfter0ToToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.limitDigitsAfter0ToToolStripMenuItem.Text = "Limit digits after \'0\' to";
            // 
            // toolStripRemainderDigits
            // 
            this.toolStripRemainderDigits.AutoCompleteCustomSource.AddRange(new string[] {
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100",
            "150",
            "200"});
            this.toolStripRemainderDigits.AutoSize = false;
            this.toolStripRemainderDigits.CausesValidation = false;
            this.toolStripRemainderDigits.DropDownWidth = 12;
            this.toolStripRemainderDigits.Items.AddRange(new object[] {
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100",
            "150",
            "200"});
            this.toolStripRemainderDigits.Name = "toolStripRemainderDigits";
            this.toolStripRemainderDigits.Size = new System.Drawing.Size(50, 23);
            this.toolStripRemainderDigits.Text = "20";
            this.toolStripRemainderDigits.SelectedIndexChanged += new System.EventHandler(this.toolStripRemainderDigits_DropDownClosed);
            // 
            // maxOperationTimeInSeconds
            // 
            this.maxOperationTimeInSeconds.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCmbTimeOut});
            this.maxOperationTimeInSeconds.Name = "maxOperationTimeInSeconds";
            this.maxOperationTimeInSeconds.Size = new System.Drawing.Size(236, 22);
            this.maxOperationTimeInSeconds.Text = "Max operation time in seconds";
            // 
            // toolStripCmbTimeOut
            // 
            this.toolStripCmbTimeOut.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "5",
            "10",
            "30",
            "60",
            "120",
            "240",
            "480",
            "960"});
            this.toolStripCmbTimeOut.Name = "toolStripCmbTimeOut";
            this.toolStripCmbTimeOut.Size = new System.Drawing.Size(75, 23);
            this.toolStripCmbTimeOut.Text = "2";
            this.toolStripCmbTimeOut.DropDownClosed += new System.EventHandler(this.toolStripCmbTimeOut_DropDownClosed);
            // 
            // colorThemToolStripMenuItem
            // 
            this.colorThemToolStripMenuItem.CheckOnClick = true;
            this.colorThemToolStripMenuItem.Name = "colorThemToolStripMenuItem";
            this.colorThemToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.colorThemToolStripMenuItem.Text = "White on Black text";
            this.colorThemToolStripMenuItem.Click += new System.EventHandler(this.colorThemToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showNumberPadToolStripMenuItem,
            this.enableWindowKeysToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // showNumberPadToolStripMenuItem
            // 
            this.showNumberPadToolStripMenuItem.CheckOnClick = true;
            this.showNumberPadToolStripMenuItem.Name = "showNumberPadToolStripMenuItem";
            this.showNumberPadToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.showNumberPadToolStripMenuItem.Text = "Show number pad";
            this.showNumberPadToolStripMenuItem.Click += new System.EventHandler(this.showNumberPadToolStripMenuItem_Click);
            // 
            // enableWindowKeysToolStripMenuItem
            // 
            this.enableWindowKeysToolStripMenuItem.Checked = true;
            this.enableWindowKeysToolStripMenuItem.CheckOnClick = true;
            this.enableWindowKeysToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableWindowKeysToolStripMenuItem.Name = "enableWindowKeysToolStripMenuItem";
            this.enableWindowKeysToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.enableWindowKeysToolStripMenuItem.Text = "Enable Window Keys";
            this.enableWindowKeysToolStripMenuItem.Click += new System.EventHandler(this.enableWindowKeysToolStripMenuItem_Click);
            // 
            // tlItmUD
            // 
            this.tlItmUD.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.exportImportToolStripMenuItem,
            this.tsShowPreParsed});
            this.tlItmUD.Name = "tlItmUD";
            this.tlItmUD.Size = new System.Drawing.Size(86, 20);
            this.tlItmUD.Text = "User Defined";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.UserDefMenuItem_Click);
            // 
            // exportImportToolStripMenuItem
            // 
            this.exportImportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.exportImportToolStripMenuItem.Name = "exportImportToolStripMenuItem";
            this.exportImportToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.exportImportToolStripMenuItem.Text = "Export/Import";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // tsShowPreParsed
            // 
            this.tsShowPreParsed.CheckOnClick = true;
            this.tsShowPreParsed.Name = "tsShowPreParsed";
            this.tsShowPreParsed.Size = new System.Drawing.Size(218, 22);
            this.tsShowPreParsed.Text = "Show Preparsed Command";
            this.tsShowPreParsed.Click += new System.EventHandler(this.ShowPreParsed_Click);
            // 
            // btn_0
            // 
            this.btn_0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_0.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_0.BackColor = System.Drawing.Color.LightSalmon;
            this.btn_0.Location = new System.Drawing.Point(14, 428);
            this.btn_0.Name = "btn_0";
            this.btn_0.Size = new System.Drawing.Size(33, 30);
            this.btn_0.TabIndex = 4;
            this.btn_0.Tag = "PadNumbers";
            this.btn_0.Text = "0";
            this.btn_0.UseVisualStyleBackColor = false;
            this.btn_0.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_1
            // 
            this.btn_1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_1.BackColor = System.Drawing.Color.LightSalmon;
            this.btn_1.Location = new System.Drawing.Point(14, 392);
            this.btn_1.Name = "btn_1";
            this.btn_1.Size = new System.Drawing.Size(33, 30);
            this.btn_1.TabIndex = 4;
            this.btn_1.Tag = "PadNumbers";
            this.btn_1.Text = "1";
            this.btn_1.UseVisualStyleBackColor = false;
            this.btn_1.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_2
            // 
            this.btn_2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_2.BackColor = System.Drawing.Color.LightSalmon;
            this.btn_2.Location = new System.Drawing.Point(53, 392);
            this.btn_2.Name = "btn_2";
            this.btn_2.Size = new System.Drawing.Size(33, 30);
            this.btn_2.TabIndex = 4;
            this.btn_2.Tag = "PadNumbers";
            this.btn_2.Text = "2";
            this.btn_2.UseVisualStyleBackColor = false;
            this.btn_2.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_3
            // 
            this.btn_3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_3.BackColor = System.Drawing.Color.LightSalmon;
            this.btn_3.Location = new System.Drawing.Point(92, 392);
            this.btn_3.Name = "btn_3";
            this.btn_3.Size = new System.Drawing.Size(33, 30);
            this.btn_3.TabIndex = 4;
            this.btn_3.Tag = "PadNumbers";
            this.btn_3.Text = "3";
            this.btn_3.UseVisualStyleBackColor = false;
            this.btn_3.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_4
            // 
            this.btn_4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_4.BackColor = System.Drawing.Color.LightSalmon;
            this.btn_4.Location = new System.Drawing.Point(14, 358);
            this.btn_4.Name = "btn_4";
            this.btn_4.Size = new System.Drawing.Size(33, 30);
            this.btn_4.TabIndex = 4;
            this.btn_4.Tag = "PadNumbers";
            this.btn_4.Text = "4";
            this.btn_4.UseVisualStyleBackColor = false;
            this.btn_4.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_5
            // 
            this.btn_5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_5.BackColor = System.Drawing.Color.LightSalmon;
            this.btn_5.Location = new System.Drawing.Point(53, 358);
            this.btn_5.Name = "btn_5";
            this.btn_5.Size = new System.Drawing.Size(33, 30);
            this.btn_5.TabIndex = 4;
            this.btn_5.Tag = "PadNumbers";
            this.btn_5.Text = "5";
            this.btn_5.UseVisualStyleBackColor = false;
            this.btn_5.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_6
            // 
            this.btn_6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_6.BackColor = System.Drawing.Color.LightSalmon;
            this.btn_6.Location = new System.Drawing.Point(91, 358);
            this.btn_6.Name = "btn_6";
            this.btn_6.Size = new System.Drawing.Size(33, 30);
            this.btn_6.TabIndex = 4;
            this.btn_6.Tag = "PadNumbers";
            this.btn_6.Text = "6";
            this.btn_6.UseVisualStyleBackColor = false;
            this.btn_6.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_7
            // 
            this.btn_7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_7.BackColor = System.Drawing.Color.LightSalmon;
            this.btn_7.Location = new System.Drawing.Point(14, 322);
            this.btn_7.Name = "btn_7";
            this.btn_7.Size = new System.Drawing.Size(33, 30);
            this.btn_7.TabIndex = 4;
            this.btn_7.Tag = "PadNumbers";
            this.btn_7.Text = "7";
            this.btn_7.UseVisualStyleBackColor = false;
            this.btn_7.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_8
            // 
            this.btn_8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_8.BackColor = System.Drawing.Color.LightSalmon;
            this.btn_8.Location = new System.Drawing.Point(52, 322);
            this.btn_8.Name = "btn_8";
            this.btn_8.Size = new System.Drawing.Size(33, 30);
            this.btn_8.TabIndex = 4;
            this.btn_8.Tag = "PadNumbers";
            this.btn_8.Text = "8";
            this.btn_8.UseVisualStyleBackColor = false;
            this.btn_8.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_9
            // 
            this.btn_9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_9.BackColor = System.Drawing.Color.LightSalmon;
            this.btn_9.Location = new System.Drawing.Point(89, 322);
            this.btn_9.Name = "btn_9";
            this.btn_9.Size = new System.Drawing.Size(35, 30);
            this.btn_9.TabIndex = 4;
            this.btn_9.Tag = "PadNumbers";
            this.btn_9.Text = "9";
            this.btn_9.UseVisualStyleBackColor = false;
            this.btn_9.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_dot
            // 
            this.btn_dot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_dot.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_dot.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_dot.Location = new System.Drawing.Point(53, 428);
            this.btn_dot.Name = "btn_dot";
            this.btn_dot.Size = new System.Drawing.Size(33, 30);
            this.btn_dot.TabIndex = 4;
            this.btn_dot.Text = ".";
            this.btn_dot.UseVisualStyleBackColor = true;
            this.btn_dot.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_div
            // 
            this.btn_div.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_div.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_div.BackColor = System.Drawing.Color.Khaki;
            this.btn_div.Location = new System.Drawing.Point(210, 322);
            this.btn_div.Name = "btn_div";
            this.btn_div.Size = new System.Drawing.Size(33, 30);
            this.btn_div.TabIndex = 4;
            this.btn_div.Tag = "PadOpp";
            this.btn_div.Text = "/";
            this.btn_div.UseVisualStyleBackColor = true;
            this.btn_div.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_mul
            // 
            this.btn_mul.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_mul.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_mul.BackColor = System.Drawing.Color.Khaki;
            this.btn_mul.Location = new System.Drawing.Point(210, 358);
            this.btn_mul.Name = "btn_mul";
            this.btn_mul.Size = new System.Drawing.Size(33, 30);
            this.btn_mul.TabIndex = 4;
            this.btn_mul.Tag = "PadOpp";
            this.btn_mul.Text = "*";
            this.btn_mul.UseVisualStyleBackColor = true;
            this.btn_mul.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_min
            // 
            this.btn_min.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_min.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_min.BackColor = System.Drawing.Color.Khaki;
            this.btn_min.Location = new System.Drawing.Point(209, 392);
            this.btn_min.Name = "btn_min";
            this.btn_min.Size = new System.Drawing.Size(33, 30);
            this.btn_min.TabIndex = 4;
            this.btn_min.Tag = "PadOpp";
            this.btn_min.Text = "-";
            this.btn_min.UseVisualStyleBackColor = true;
            this.btn_min.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_plus
            // 
            this.btn_plus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_plus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_plus.BackColor = System.Drawing.Color.Khaki;
            this.btn_plus.Location = new System.Drawing.Point(209, 428);
            this.btn_plus.Name = "btn_plus";
            this.btn_plus.Size = new System.Drawing.Size(33, 30);
            this.btn_plus.TabIndex = 4;
            this.btn_plus.Tag = "PadOpp";
            this.btn_plus.Text = "+";
            this.btn_plus.UseVisualStyleBackColor = true;
            this.btn_plus.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_equ
            // 
            this.btn_equ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_equ.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_equ.BackColor = System.Drawing.Color.LightGreen;
            this.btn_equ.Location = new System.Drawing.Point(248, 428);
            this.btn_equ.Name = "btn_equ";
            this.btn_equ.Size = new System.Drawing.Size(33, 30);
            this.btn_equ.TabIndex = 4;
            this.btn_equ.Text = "=";
            this.btn_equ.UseVisualStyleBackColor = true;
            this.btn_equ.Click += new System.EventHandler(this.btn_equ_Click);
            // 
            // btn_mod
            // 
            this.btn_mod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_mod.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_mod.BackColor = System.Drawing.Color.Khaki;
            this.btn_mod.Location = new System.Drawing.Point(248, 392);
            this.btn_mod.Name = "btn_mod";
            this.btn_mod.Size = new System.Drawing.Size(33, 30);
            this.btn_mod.TabIndex = 4;
            this.btn_mod.Tag = "PadOpp";
            this.btn_mod.Text = "%";
            this.btn_mod.UseVisualStyleBackColor = true;
            this.btn_mod.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_ans
            // 
            this.btn_ans.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_ans.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_ans.BackColor = System.Drawing.Color.PaleTurquoise;
            this.btn_ans.Location = new System.Drawing.Point(286, 428);
            this.btn_ans.Name = "btn_ans";
            this.btn_ans.Size = new System.Drawing.Size(74, 30);
            this.btn_ans.TabIndex = 4;
            this.btn_ans.Text = "ANS";
            this.btn_ans.UseVisualStyleBackColor = true;
            this.btn_ans.Click += new System.EventHandler(this.btn_ans_Click);
            // 
            // btn_lastInput
            // 
            this.btn_lastInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_lastInput.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_lastInput.BackColor = System.Drawing.Color.PaleTurquoise;
            this.btn_lastInput.Location = new System.Drawing.Point(287, 392);
            this.btn_lastInput.Name = "btn_lastInput";
            this.btn_lastInput.Size = new System.Drawing.Size(74, 30);
            this.btn_lastInput.TabIndex = 4;
            this.btn_lastInput.Text = "Last Input";
            this.btn_lastInput.UseVisualStyleBackColor = true;
            this.btn_lastInput.Click += new System.EventHandler(this.btn_lastInput_Click);
            // 
            // cb_showPad
            // 
            this.cb_showPad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_showPad.AutoSize = true;
            this.cb_showPad.Location = new System.Drawing.Point(327, 468);
            this.cb_showPad.Name = "cb_showPad";
            this.cb_showPad.Size = new System.Drawing.Size(45, 17);
            this.cb_showPad.TabIndex = 2;
            this.cb_showPad.Text = "Pad";
            this.cb_showPad.UseVisualStyleBackColor = true;
            this.cb_showPad.CheckedChanged += new System.EventHandler(this.cb_showPad_CheckedChanged);
            // 
            // btn_sftRight
            // 
            this.btn_sftRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_sftRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_sftRight.BackColor = System.Drawing.Color.NavajoWhite;
            this.btn_sftRight.Location = new System.Drawing.Point(248, 358);
            this.btn_sftRight.Name = "btn_sftRight";
            this.btn_sftRight.Size = new System.Drawing.Size(33, 30);
            this.btn_sftRight.TabIndex = 4;
            this.btn_sftRight.Tag = "PadOpp";
            this.btn_sftRight.Text = ">>";
            this.btn_sftRight.UseVisualStyleBackColor = true;
            this.btn_sftRight.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_sftLeft
            // 
            this.btn_sftLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_sftLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_sftLeft.BackColor = System.Drawing.Color.NavajoWhite;
            this.btn_sftLeft.Location = new System.Drawing.Point(248, 322);
            this.btn_sftLeft.Name = "btn_sftLeft";
            this.btn_sftLeft.Size = new System.Drawing.Size(33, 30);
            this.btn_sftLeft.TabIndex = 4;
            this.btn_sftLeft.Tag = "PadOpp";
            this.btn_sftLeft.Text = "<<";
            this.btn_sftLeft.UseVisualStyleBackColor = true;
            this.btn_sftLeft.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_or
            // 
            this.btn_or.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_or.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_or.BackColor = System.Drawing.Color.NavajoWhite;
            this.btn_or.Location = new System.Drawing.Point(321, 322);
            this.btn_or.Name = "btn_or";
            this.btn_or.Size = new System.Drawing.Size(33, 30);
            this.btn_or.TabIndex = 4;
            this.btn_or.Tag = "PadOpp";
            this.btn_or.Text = "or";
            this.btn_or.UseVisualStyleBackColor = true;
            this.btn_or.Click += new System.EventHandler(this.btn_or_Click);
            // 
            // btn_and
            // 
            this.btn_and.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_and.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_and.BackColor = System.Drawing.Color.NavajoWhite;
            this.btn_and.Location = new System.Drawing.Point(288, 322);
            this.btn_and.Name = "btn_and";
            this.btn_and.Size = new System.Drawing.Size(33, 30);
            this.btn_and.TabIndex = 4;
            this.btn_and.Tag = "PadOpp";
            this.btn_and.Text = "and";
            this.btn_and.UseVisualStyleBackColor = true;
            this.btn_and.Click += new System.EventHandler(this.btn_and_Click);
            // 
            // btn_B
            // 
            this.btn_B.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_B.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_B.BackColor = System.Drawing.Color.DarkSalmon;
            this.btn_B.Location = new System.Drawing.Point(163, 322);
            this.btn_B.Name = "btn_B";
            this.btn_B.Size = new System.Drawing.Size(35, 30);
            this.btn_B.TabIndex = 4;
            this.btn_B.Tag = "PadNumbers";
            this.btn_B.Text = "B";
            this.btn_B.UseVisualStyleBackColor = false;
            this.btn_B.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_A
            // 
            this.btn_A.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_A.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_A.BackColor = System.Drawing.Color.DarkSalmon;
            this.btn_A.Location = new System.Drawing.Point(126, 322);
            this.btn_A.Name = "btn_A";
            this.btn_A.Size = new System.Drawing.Size(33, 30);
            this.btn_A.TabIndex = 4;
            this.btn_A.Tag = "PadNumbers";
            this.btn_A.Text = "A";
            this.btn_A.UseVisualStyleBackColor = false;
            this.btn_A.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_D
            // 
            this.btn_D.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_D.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_D.BackColor = System.Drawing.Color.DarkSalmon;
            this.btn_D.Location = new System.Drawing.Point(165, 358);
            this.btn_D.Name = "btn_D";
            this.btn_D.Size = new System.Drawing.Size(33, 30);
            this.btn_D.TabIndex = 4;
            this.btn_D.Tag = "PadNumbers";
            this.btn_D.Text = "D";
            this.btn_D.UseVisualStyleBackColor = false;
            this.btn_D.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_C
            // 
            this.btn_C.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_C.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_C.BackColor = System.Drawing.Color.DarkSalmon;
            this.btn_C.Location = new System.Drawing.Point(127, 358);
            this.btn_C.Name = "btn_C";
            this.btn_C.Size = new System.Drawing.Size(33, 30);
            this.btn_C.TabIndex = 4;
            this.btn_C.Tag = "PadNumbers";
            this.btn_C.Text = "C";
            this.btn_C.UseVisualStyleBackColor = false;
            this.btn_C.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_F
            // 
            this.btn_F.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_F.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_F.BackColor = System.Drawing.Color.DarkSalmon;
            this.btn_F.Location = new System.Drawing.Point(166, 392);
            this.btn_F.Name = "btn_F";
            this.btn_F.Size = new System.Drawing.Size(33, 30);
            this.btn_F.TabIndex = 4;
            this.btn_F.Tag = "PadNumbers";
            this.btn_F.Text = "F";
            this.btn_F.UseVisualStyleBackColor = false;
            this.btn_F.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_E
            // 
            this.btn_E.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_E.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_E.BackColor = System.Drawing.Color.DarkSalmon;
            this.btn_E.Location = new System.Drawing.Point(127, 392);
            this.btn_E.Name = "btn_E";
            this.btn_E.Size = new System.Drawing.Size(33, 30);
            this.btn_E.TabIndex = 4;
            this.btn_E.Tag = "PadNumbers";
            this.btn_E.Text = "E";
            this.btn_E.UseVisualStyleBackColor = false;
            this.btn_E.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_0x
            // 
            this.btn_0x.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_0x.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_0x.BackColor = System.Drawing.Color.NavajoWhite;
            this.btn_0x.Location = new System.Drawing.Point(92, 428);
            this.btn_0x.Name = "btn_0x";
            this.btn_0x.Size = new System.Drawing.Size(33, 30);
            this.btn_0x.TabIndex = 4;
            this.btn_0x.Tag = "PadOpp";
            this.btn_0x.Text = "0x";
            this.btn_0x.UseVisualStyleBackColor = true;
            this.btn_0x.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_0b
            // 
            this.btn_0b.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_0b.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_0b.BackColor = System.Drawing.Color.NavajoWhite;
            this.btn_0b.Location = new System.Drawing.Point(163, 428);
            this.btn_0b.Name = "btn_0b";
            this.btn_0b.Size = new System.Drawing.Size(33, 30);
            this.btn_0b.TabIndex = 4;
            this.btn_0b.Tag = "PadOpp";
            this.btn_0b.Text = "0b";
            this.btn_0b.UseVisualStyleBackColor = true;
            this.btn_0b.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // btn_BackSpace
            // 
            this.btn_BackSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_BackSpace.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_BackSpace.BackColor = System.Drawing.Color.DarkSalmon;
            this.btn_BackSpace.Location = new System.Drawing.Point(360, 322);
            this.btn_BackSpace.Name = "btn_BackSpace";
            this.btn_BackSpace.Size = new System.Drawing.Size(35, 30);
            this.btn_BackSpace.TabIndex = 4;
            this.btn_BackSpace.Tag = "PadNumbers";
            this.btn_BackSpace.Text = "<--";
            this.btn_BackSpace.UseVisualStyleBackColor = false;
            this.btn_BackSpace.Click += new System.EventHandler(this.btn_BackSpace_Click);
            // 
            // cb_BinDigits
            // 
            this.cb_BinDigits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_BinDigits.AutoSize = true;
            this.cb_BinDigits.Location = new System.Drawing.Point(251, 468);
            this.cb_BinDigits.Name = "cb_BinDigits";
            this.cb_BinDigits.Size = new System.Drawing.Size(70, 17);
            this.cb_BinDigits.TabIndex = 2;
            this.cb_BinDigits.Text = "Bin Digits";
            this.cb_BinDigits.UseVisualStyleBackColor = true;
            this.cb_BinDigits.CheckedChanged += new System.EventHandler(this.cb_BinDigits_CheckedChanged);
            // 
            // lblDisplay
            // 
            this.lblDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDisplay.AutoSize = true;
            this.lblDisplay.Location = new System.Drawing.Point(106, 464);
            this.lblDisplay.Name = "lblDisplay";
            this.lblDisplay.Size = new System.Drawing.Size(41, 13);
            this.lblDisplay.TabIndex = 5;
            this.lblDisplay.Text = "Display";
            this.lblDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_Win4
            // 
            this.btn_Win4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Win4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_Win4.Location = new System.Drawing.Point(300, 24);
            this.btn_Win4.Name = "btn_Win4";
            this.btn_Win4.Size = new System.Drawing.Size(65, 23);
            this.btn_Win4.TabIndex = 9;
            this.btn_Win4.Text = "Window 4";
            this.btn_Win4.UseVisualStyleBackColor = true;
            this.btn_Win4.Click += new System.EventHandler(this.btn_Win4_Click);
            // 
            // btn_Win3
            // 
            this.btn_Win3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Win3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_Win3.Location = new System.Drawing.Point(209, 24);
            this.btn_Win3.Name = "btn_Win3";
            this.btn_Win3.Size = new System.Drawing.Size(65, 23);
            this.btn_Win3.TabIndex = 10;
            this.btn_Win3.Text = "Window 3";
            this.btn_Win3.UseVisualStyleBackColor = true;
            this.btn_Win3.Click += new System.EventHandler(this.btn_Win3_Click);
            // 
            // btn_Win2
            // 
            this.btn_Win2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Win2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_Win2.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Win2.Location = new System.Drawing.Point(126, 24);
            this.btn_Win2.Name = "btn_Win2";
            this.btn_Win2.Size = new System.Drawing.Size(65, 23);
            this.btn_Win2.TabIndex = 7;
            this.btn_Win2.Text = "Window 2";
            this.btn_Win2.UseVisualStyleBackColor = false;
            this.btn_Win2.Click += new System.EventHandler(this.btn_Win2_Click);
            // 
            // btn_Win1
            // 
            this.btn_Win1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Win1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_Win1.Location = new System.Drawing.Point(44, 26);
            this.btn_Win1.Name = "btn_Win1";
            this.btn_Win1.Size = new System.Drawing.Size(65, 23);
            this.btn_Win1.TabIndex = 8;
            this.btn_Win1.Text = "Window 1";
            this.btn_Win1.UseVisualStyleBackColor = true;
            this.btn_Win1.Click += new System.EventHandler(this.btn_Win1_Click);
            // 
            // btn_0h
            // 
            this.btn_0h.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_0h.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_0h.BackColor = System.Drawing.Color.NavajoWhite;
            this.btn_0h.Location = new System.Drawing.Point(127, 428);
            this.btn_0h.Name = "btn_0h";
            this.btn_0h.Size = new System.Drawing.Size(33, 30);
            this.btn_0h.TabIndex = 4;
            this.btn_0h.Tag = "PadOpp";
            this.btn_0h.Text = "0h";
            this.tlTpCalc.SetToolTip(this.btn_0h, "byte stream");
            this.btn_0h.UseVisualStyleBackColor = true;
            this.btn_0h.Click += new System.EventHandler(this.btn_pad_Click);
            // 
            // tlTpCalc
            // 
            this.tlTpCalc.Popup += new System.Windows.Forms.PopupEventHandler(this.tlTpCalc_Popup);
            // 
            // btn_xor
            // 
            this.btn_xor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_xor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_xor.BackColor = System.Drawing.Color.NavajoWhite;
            this.btn_xor.Location = new System.Drawing.Point(321, 356);
            this.btn_xor.Name = "btn_xor";
            this.btn_xor.Size = new System.Drawing.Size(33, 30);
            this.btn_xor.TabIndex = 4;
            this.btn_xor.Tag = "PadOpp";
            this.btn_xor.Text = "xor";
            this.btn_xor.UseVisualStyleBackColor = true;
            this.btn_xor.Click += new System.EventHandler(this.btn_xor_Click);
            // 
            // btn_not
            // 
            this.btn_not.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_not.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_not.BackColor = System.Drawing.Color.NavajoWhite;
            this.btn_not.Location = new System.Drawing.Point(288, 356);
            this.btn_not.Name = "btn_not";
            this.btn_not.Size = new System.Drawing.Size(33, 30);
            this.btn_not.TabIndex = 4;
            this.btn_not.Tag = "PadOpp";
            this.btn_not.Text = "not";
            this.btn_not.UseVisualStyleBackColor = true;
            this.btn_not.Click += new System.EventHandler(this.btn_not_Click);
            // 
            // ofdUserDefined
            // 
            this.ofdUserDefined.DefaultExt = "ezud";
            this.ofdUserDefined.FileName = "EZcalcUserDefinedExp";
            this.ofdUserDefined.Title = "Please select file from which to import user defined expressions";
            // 
            // sfdUserDefined
            // 
            this.sfdUserDefined.DefaultExt = "ezud";
            this.sfdUserDefined.Title = "Please select file to export user defined expressions to";
            // 
            // Constants
            // 
            this.Constants.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.eToolStripMenuItem1,
            this.pIToolStripMenuItem1,
            this.gToolStripMenuItem1,
            this.rToolStripMenuItem1});
            this.Constants.Name = "Constants";
            this.Constants.Size = new System.Drawing.Size(175, 22);
            this.Constants.Text = "Constants";
            // 
            // bitwiseToolMenuItem
            // 
            this.bitwiseToolMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.revBitsToolStripMenuItem,
            this.revBytesToolStripMenuItem1,
            this.revNiblesToolStripMenuItem,
            this.asciToolStripMenuItem1});
            this.bitwiseToolMenuItem.Name = "bitwiseToolMenuItem";
            this.bitwiseToolMenuItem.Size = new System.Drawing.Size(175, 22);
            this.bitwiseToolMenuItem.Text = "Bits and Bytes";
            // 
            // eToolStripMenuItem1
            // 
            this.eToolStripMenuItem1.Name = "eToolStripMenuItem1";
            this.eToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.eToolStripMenuItem1.Text = "e";
            this.eToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // pIToolStripMenuItem1
            // 
            this.pIToolStripMenuItem1.Name = "pIToolStripMenuItem1";
            this.pIToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.pIToolStripMenuItem1.Text = "PI";
            this.pIToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // gToolStripMenuItem1
            // 
            this.gToolStripMenuItem1.Name = "gToolStripMenuItem1";
            this.gToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.gToolStripMenuItem1.Text = "g";
            this.gToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // rToolStripMenuItem1
            // 
            this.rToolStripMenuItem1.Name = "rToolStripMenuItem1";
            this.rToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.rToolStripMenuItem1.Text = "R";
            this.rToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // revBitsToolStripMenuItem
            // 
            this.revBitsToolStripMenuItem.Name = "revBitsToolStripMenuItem";
            this.revBitsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.revBitsToolStripMenuItem.Text = "RevBits";
            this.revBitsToolStripMenuItem.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // revBytesToolStripMenuItem1
            // 
            this.revBytesToolStripMenuItem1.Name = "revBytesToolStripMenuItem1";
            this.revBytesToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.revBytesToolStripMenuItem1.Text = "RevBytes";
            this.revBytesToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // revNiblesToolStripMenuItem
            // 
            this.revNiblesToolStripMenuItem.Name = "revNiblesToolStripMenuItem";
            this.revNiblesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.revNiblesToolStripMenuItem.Text = "RevNibles";
            this.revNiblesToolStripMenuItem.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // asciToolStripMenuItem1
            // 
            this.asciToolStripMenuItem1.Name = "asciToolStripMenuItem1";
            this.asciToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.asciToolStripMenuItem1.Text = "Asci()";
            this.asciToolStripMenuItem1.Click += new System.EventHandler(this.CommandMenuItem_Click);
            // 
            // Calculator
            // 
            this.ClientSize = new System.Drawing.Size(451, 494);
            this.Controls.Add(this.btn_Win4);
            this.Controls.Add(this.btn_Win3);
            this.Controls.Add(this.btn_Win2);
            this.Controls.Add(this.btn_Win1);
            this.Controls.Add(this.lblDisplay);
            this.Controls.Add(this.rtb_Display);
            this.Controls.Add(this.cb_showPad);
            this.Controls.Add(this.cb_BinDigits);
            this.Controls.Add(this.cb_BoolDisplay);
            this.Controls.Add(this.cb_HexDisplay);
            this.Controls.Add(this.btn_Clear);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btn_lastInput);
            this.Controls.Add(this.btn_0);
            this.Controls.Add(this.btn_1);
            this.Controls.Add(this.btn_E);
            this.Controls.Add(this.btn_2);
            this.Controls.Add(this.btn_F);
            this.Controls.Add(this.btn_3);
            this.Controls.Add(this.btn_4);
            this.Controls.Add(this.btn_C);
            this.Controls.Add(this.btn_5);
            this.Controls.Add(this.btn_D);
            this.Controls.Add(this.btn_6);
            this.Controls.Add(this.btn_7);
            this.Controls.Add(this.btn_A);
            this.Controls.Add(this.btn_8);
            this.Controls.Add(this.btn_BackSpace);
            this.Controls.Add(this.btn_B);
            this.Controls.Add(this.btn_9);
            this.Controls.Add(this.btn_dot);
            this.Controls.Add(this.btn_div);
            this.Controls.Add(this.btn_ans);
            this.Controls.Add(this.btn_0h);
            this.Controls.Add(this.btn_0x);
            this.Controls.Add(this.btn_0b);
            this.Controls.Add(this.btn_and);
            this.Controls.Add(this.btn_xor);
            this.Controls.Add(this.btn_not);
            this.Controls.Add(this.btn_or);
            this.Controls.Add(this.btn_sftLeft);
            this.Controls.Add(this.btn_sftRight);
            this.Controls.Add(this.btn_mod);
            this.Controls.Add(this.btn_mul);
            this.Controls.Add(this.btn_min);
            this.Controls.Add(this.btn_plus);
            this.Controls.Add(this.btn_equ);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(225, 363);
            this.Name = "Calculator";
            this.Text = "EZ Calc";
            this.Activated += new System.EventHandler(this.Calculator_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Calculator_FormClosing);
            this.Load += new System.EventHandler(this.Calculator_Load);
            this.SizeChanged += new System.EventHandler(this.Calculator_ResizeEnd);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        #region Main Logic
        private void runProcessTrhead(int start, int end)
        {
            Thread thrdProcessCommand;
            thrdProcessCommand = new Thread(new ThreadStart(processCommand));
            CurrentCommand = rtb_Display.Text.Substring(start, end - start);
            rtb_Display.SelectionColor = clrthActive.Answer;
            ProcessStart = start;
            ProcessEnd = end;
            thrdProcessCommand.Start();
            thrdProcessCommand.Join(MaxOperationTime * 1000);
            if (thrdProcessCommand.IsAlive) thrdProcessCommand.Abort();
            if (State == CalculatorState.CulculatingResult)
            {
                rtb_Display.AppendText("\nOperation timed out.");
                if (CurrentCommand.Contains("^"))
                {
                    rtb_Display.AppendText(" Try whole number power operations.");
                }
                rtb_Display.AppendText("\n");
                State = CalculatorState.WaitingForInput;
                return;
            }
            if (CurrentCommand.Length == 0) return;
            if((CurrentCommand != expression.PreParsedCommand)&&(stnCrnt.ShowPreParsed))
            {
                CurrentResult = ("\n" + expression.PreParsedCommand + "\n") + CurrentResult;
                //rtb_Display.AppendText("\n" + expression.PreParsedCommand + "\n\n");
            }
            rtb_Display.AppendText(CurrentResult);
            if (cb_BoolDisplay.Checked) ChangeBitNumberColor(start);
            rtb_Display.AppendText("\n");
            rtb_Display.Focus();
            rtb_Display.SelectionColor = clrthActive.Input;
        }
        private void processCommand()
        {
            processCommand(CurrentCommand, out CurrentResult);
        }
        private void processCommand(string command, out string result) 
		{
            string strDecResults = "", strHexResults = "", strBinResults = "";
            string strDec, strHex, strBin;
            char[] delimiters = new char[] { ';', ',' };
            string[] commands = command.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            result = "";
            int intCmd = 0;
            foreach ( string strCommand in commands)
            {
                lastInput = strCommand;
                State = CalculatorState.CulculatingResult;
                if (intCmd > 0)
                {
                    strDecResults += ", ";
                    if (expression.DisplayHex) strHexResults += ", ";
                    if (expression.DisplayBool) strBinResults += ", ";
                }
                strDec = expression.ParseCommand(strCommand);
                State = CalculatorState.CulculatingFormats;
                expression.GetOtherFormats( out strHex, out strBin);
                strDecResults += strDec;
                if (expression.DisplayHex) strHexResults += strHex;
                if (expression.DisplayBool) strBinResults += strBin;
                intCmd++;
            }
            result = strDecResults;
            if( expression.DisplayHex )
            {
                if (intCmd > 1) result += "\n";
                else result += "  ";
                result += strHexResults;
            }
            if (expression.DisplayBool)
            {
                if (intCmd > 1) result += "\n";
                else result += "  ";
                result += strBinResults;
            }

            State = CalculatorState.WaitingForInput;
        }
        private void ChangeBitNumberColor(int StartPos)
        {
            int bit = 0;
            int index = rtb_Display.Find("[0]", StartPos, RichTextBoxFinds.NoHighlight);
            int PosOfBinResult = index;
            while (index > 0)
            {
                rtb_Display.Select(index, bit.ToString().Length + 2);
                rtb_Display.SelectionColor = clrthActive.BitIndex;
                bit += 4;
                index = rtb_Display.Find("[" + bit + "]", StartPos, RichTextBoxFinds.NoHighlight);
                //Looking for another result for multiple result displays
                if( index <= 0 )
                {
                    StartPos = PosOfBinResult + 1;
                    bit = 0;
                    index = rtb_Display.Find("[" + bit + "]", StartPos, RichTextBoxFinds.NoHighlight);
                    PosOfBinResult = index;
                }
            }
            rtb_Display.HideSelection = true;
        }
        #endregion
        /// <summary>
		/// Finds the last math expression, and then begins processing it
        /// </summary>
        private void Calculator_Load(object sender, EventArgs e)
        {
            this.Text += " " + version.Major + "." + version.Minor.ToString("0#");
            FormWindowState l_WindowState = FormWindowState.Normal;
            try
            {
                #region Properties
                cb_HexDisplay.Checked = stnCrnt.DisplayHex;
                cb_BoolDisplay.Checked = stnCrnt.DisplayBool;
                cb_BinDigits.Checked = stnCrnt.DisplayBoolDigits;
                rtb_Display.Font = (Font)stnCrnt.DisplayFont.Clone();
                colorThemToolStripMenuItem.Checked = (stnCrnt.clrThemEnum == ColorThemeEnum.WhiteOnBlack );
                this.Size = stnCrnt.CulculatorSize;
                this.Location = stnCrnt.CulculatorLocation;
                saveTextForNextRunToolStripMenuItem.Checked = stnCrnt.SaveText;
                showNumberPadToolStripMenuItem.Checked = stnCrnt.ShowPad;
                tsShowPreParsed.Checked = stnCrnt.ShowPreParsed;
                enableWindowKeysToolStripMenuItem.Checked = stnCrnt.ShowWindows;
                toolStripRemainderDigits.Text = stnCrnt.RemainderSize.ToString();
                expression.RemainderDigits = stnCrnt.RemainderSize;
                toolStripCmbTimeOut.Text = stnCrnt.MaxOppTime.ToString();
                MaxOperationTime = stnCrnt.MaxOppTime;
                rtb_Display.Rtf = stnCrnt.WindowsText[stnCrnt.WindowCurrent];
                rtb_Display.Select(rtb_Display.Text.Length, 0);
                l_WindowState = stnCrnt.WndState;
                if (stnCrnt.WindowsText == null)
                {
                    stnCrnt.WindowsText = new string[cnstNumWin];
                    for (int ind = 0; ind < cnstNumWin; ind++)
                    {
                        stnCrnt.WindowsText[ind] = "";
                    }
                    stnCrnt.WindowCurrent = 0;
                }
                stnCrnt.CulculatorSize = new System.Drawing.Size(331, 474);
                stnCrnt.CulculatorLocation = new Point(105, 262);
                stnCrnt.RemainderSize = int.Parse(toolStripRemainderDigits.Text);
                stnCrnt.MaxOppTime = MaxOperationTime;
                if (stnCrnt.UsrDefExprs == null) stnCrnt.UsrDefExprs = new UserDefinedExpression[MaxUserDefExp];
                #region User defined expressions
                if (stnCrnt.UsrDefExprs != null)
                {
                    for (int indUDE = 0; indUDE < stnCrnt.NumUserDef; indUDE++ )
                    {
                        UserDefinedExpression udExp = stnCrnt.UsrDefExprs[indUDE];
                        AddUserDefinedToMenu(udExp);
                    }
                }
                #endregion
                stnCrnt.Save();
                #region control placement and size and other features
                abtnWindows[0] = btn_Win1;
                abtnWindows[1] = btn_Win2;
                abtnWindows[2] = btn_Win3;
                abtnWindows[3] = btn_Win4;
                PlaceButtons();
                abtnWindows[stnCrnt.WindowCurrent].BackColor = Color.FromKnownColor(System.Drawing.KnownColor.GradientActiveCaption);
                showNumberPadToolStripMenuItem_Click(sender, e);
                SizeTextDisplay();
                if (stnCrnt.clrThemEnum == ColorThemeEnum.BlackOnWhite)
                {
                    clrthActive.SetThemBlackOnWhite();
                    clrthInactive.SetThemWhiteOnBlack();
                }
                else
                {
                    clrthActive.SetThemWhiteOnBlack();
                    clrthInactive.SetThemBlackOnWhite();
                }
                AdjustColors();
                #endregion
                #endregion
            }
            catch (Exception)
            {
                stnCrnt = new Properties.Settings();
                Calculator_FormClosing(sender, null);
                Calculator_Load(sender,e);
                return;
            }
            enableWindowKeysToolStripMenuItem_Click(sender, null);
            State = CalculatorState.WaitingForInput;
            frmLoad = null;
            Thread.Sleep(250);
            this.WindowState = l_WindowState;
            Application.OpenForms["Calculator"].BringToFront();
            this.Activate();
            rtb_Display.Focus();
        }
        private void Calculator_FormClosing(object sender, FormClosingEventArgs e)
        {
            stnCrnt.WndState = WindowState;
            WindowState = FormWindowState.Normal;
            stnCrnt.DisplayHex = cb_HexDisplay.Checked;
            stnCrnt.DisplayBool = cb_BoolDisplay.Checked;
            stnCrnt.DisplayBoolDigits = cb_BinDigits.Checked;
            stnCrnt.DisplayFont = (Font)rtb_Display.Font.Clone();
            stnCrnt.CulculatorSize = this.Size;
            stnCrnt.CulculatorLocation = this.Location;
            stnCrnt.SaveText = saveTextForNextRunToolStripMenuItem.Checked;
            stnCrnt.ShowPad = showNumberPadToolStripMenuItem.Checked;
            stnCrnt.RemainderSize = int.Parse(toolStripRemainderDigits.Text);
            stnCrnt.MaxOppTime = MaxOperationTime;
            if (stnCrnt.SaveText)
            {
                stnCrnt.WindowsText[stnCrnt.WindowCurrent] = rtb_Display.Rtf; ;
            }
            else
            {
                stnCrnt.WindowsText = new string[cnstNumWin];
                for (int ind = 0; ind < cnstNumWin; ind++)
                {
                    stnCrnt.WindowsText[ind] = "";
                }
                stnCrnt.WindowCurrent = 0;
            }
            try
            {
                stnCrnt.Save();
            }
            catch (Exception) { }
        }
        private void Calculator_ResizeEnd(object sender, EventArgs e)
        {
            PlaceButtons();
            SizeTextDisplay();
        }
        private void SwitchWindow(int toWindow)
        {
            try
            {
                string btnString = LastLine(rtb_Display.Rtf);
                if (btnString.Length > 2)
                {
                    abtnWindows[stnCrnt.WindowCurrent].Text = btnString;
                }
                else
                {
                    abtnWindows[stnCrnt.WindowCurrent].Text = "Window " + (stnCrnt.WindowCurrent + 1).ToString();
                }
                abtnWindows[stnCrnt.WindowCurrent].BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Control);
                stnCrnt.WindowsText[stnCrnt.WindowCurrent] = rtb_Display.Rtf;
                rtb_Display.Rtf = stnCrnt.WindowsText[toWindow];
                abtnWindows[toWindow].BackColor = Color.FromKnownColor(System.Drawing.KnownColor.GradientActiveCaption);
                stnCrnt.WindowCurrent = toWindow;
                stnCrnt.Save();
            }
            catch (Exception) { }
            AdjustColors();
        }
		private void b_Enter_Click(object sender, System.EventArgs e)
		{
			int pos1, pos2;
            pos1 = rtb_Display.Text.LastIndexOf('\n') + 1;
            pos2 = rtb_Display.Text.Length;
            rtb_Display.AppendText("" + '\n');	// go to the Next Line
            runProcessTrhead(pos1, pos2);
            this.rtb_Display.Focus();
		}
        private void rtb_Display_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == ' ')||(State != CalculatorState.WaitingForInput))	// Ignore any spaces that are entered
            {
                e.Handled = true;
                return;
            }
            if ((e.KeyChar == 13)||(e.KeyChar == '='))
            {
                e.Handled = true;
                int pos1, pos2;
                pos1 = rtb_Display.Text.Substring(0, rtb_Display.Text.Length - 1).LastIndexOf('\n') + 1;
                pos2 = rtb_Display.Text.Length - 1;
                if (e.KeyChar == '=')
                {
                    pos2++;
                    rtb_Display.AppendText("\n");
                }
                runProcessTrhead(pos1, pos2);
            }
            else if((rtb_Display.Text.Length > 0 ) && (rtb_Display.Text[rtb_Display.Text.Length - 1] == '\n') &&
                ((e.KeyChar == '*') || (e.KeyChar == '+') || (e.KeyChar == '^') || (e.KeyChar == '/') || (e.KeyChar == '-') ||
                 (e.KeyChar == '<') || (e.KeyChar == '>') || (e.KeyChar == '%')))
            {
                rtb_Display.AppendText("ANS");
            }
        }
        private void rtb_Display_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) rtb_Display.Select(rtb_Display.Text.Length, 0);
            else if (e.Control == true && e.KeyCode == Keys.V)
            {
                e.Handled = true;
                string st = Clipboard.GetText();
                st = st.Replace("\r", "");
                st = st.Replace("\n", "");
                rtb_Display.AppendText(st);
            }
            else if ((e.Control == true) && ( e.KeyCode == Keys.Tab ))
            {
                int intBtn;
                e.Handled = true;
                if (e.Shift == true)
                {
                    intBtn = (stnCrnt.WindowCurrent + (cnstNumWin - 1)) % cnstNumWin;
                }
                else
                {
                    intBtn = (stnCrnt.WindowCurrent + 1) % cnstNumWin;
                }
                abtnWindows[intBtn].PerformClick();
            }
        }
        private void rtb_Display_KeyUp(object sender, KeyEventArgs e)
        {
            if (CursorToEndOnKeyUp)
            {
                rtb_Display.Select(rtb_Display.Text.Length, 0);
                CursorToEndOnKeyUp = false;
            }
        }
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            rtb_Display.Clear();
            rtb_Display.Focus();
            rtb_Display.Select(rtb_Display.Text.Length, 0);
            rtb_Display.SelectionColor = clrthActive.Input;
        }
        private void cb_HexDisplay_CheckedChanged(object sender, EventArgs e)
        {
            expression.DisplayHex = cb_HexDisplay.Checked;
            rtb_Display.Focus();
        }
        private void cb_BoolDisplay_CheckedChanged(object sender, EventArgs e)
        {
            expression.DisplayBool = cb_BoolDisplay.Checked;
            rtb_Display.Focus();
        }
        private void cb_BinDigits_CheckedChanged(object sender, EventArgs e)
        {
            expression.DisplayBoolDigits = cb_BinDigits.Checked;
            rtb_Display.Focus();
        }
        private void PlaceButtons()
        {
            btn_Win1.Top = 30;
            btn_Win2.Top = 30;
            btn_Win3.Top = 30;
            btn_Win4.Top = 30;
            
            int btnPlace = (this.Width-30)/4;
            btn_Win1.Left = 10;
            btn_Win1.Width = btnPlace - 10;
            btn_Win2.Left = btnPlace+10;
            btn_Win2.Width = btnPlace - 10;
            btn_Win3.Left = (btnPlace * 2) + 10;
            btn_Win3.Width = btnPlace - 10;
            btn_Win4.Left = (btnPlace * 3) + 10;
            btn_Win4.Width = btnPlace - 10;
            try
            {
                for (int ind = 0; ind < cnstNumWin; ind++)
                {
                    string btnString = LastLine(stnCrnt.WindowsText[ind]);
                    if (btnString.Length > 2)
                    {
                        abtnWindows[ind].Text = btnString;
                    }
                    else
                    {
                        abtnWindows[ind].Text = "Window " + (ind + 1).ToString();
                    }
                    abtnWindows[ind].BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Control);
                }
                abtnWindows[stnCrnt.WindowCurrent].BackColor = Color.FromKnownColor(System.Drawing.KnownColor.GradientActiveCaption);
            }
            catch (Exception)
            {
            }
        }
        private string LastLine(string rtf)
        {
            RichTextBox rtb = new RichTextBox();
            rtb.Rtf = rtf;
            string strLastLine = rtb.Text;
            if (rtf == null) return "";
            if (strLastLine.Length < 2) return "";
            while (strLastLine[strLastLine.Length - 1] == '\n')
            {
                strLastLine = strLastLine.Substring(0, strLastLine.Length - 1);
            }
            if(strLastLine.Contains("\n"))
            {
                strLastLine = strLastLine.Substring(strLastLine.LastIndexOf('\n'));
            }
            strLastLine = strLastLine.Replace("\n", "");
            return strLastLine;            
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Message = version.ToString() + " Last build on " + verDate.ToString() + "\nYigaelBen-Natan\n" + strContact + "\nRepo: " + strRepo;
            MessageBox.Show(Message, "EZcalc Version");
        }
        private void CommandMenuItem_Click(object sender, EventArgs e)
        {
            rtb_Display.AppendText(sender.ToString());
        }
        private void fontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fd_Display.Font = (Font)rtb_Display.Font.Clone();
            DialogResult dr = fd_Display.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                rtb_Display.Font = (Font)fd_Display.Font.Clone();
            }
            rtb_Display.Focus();
        }
        private void lastInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lastInput.Length < 2) return;
            rtb_Display.AppendText(lastInput);
        }
        private void showNumberPadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showNumberPadToolStripMenuItem.Checked)
            {
                showButtomPadButtonsVisibility();
            }
            else
            {
                hideButtomPadButtonsVisibility();
            }
            BreakLoop = true;
            cb_showPad.CheckState = showNumberPadToolStripMenuItem.CheckState;
            BreakLoop = false;
            SizeTextDisplay();
        }
        private void ShowPreParsed_Click(object sender, EventArgs e)
        {
            stnCrnt.ShowPreParsed = tsShowPreParsed.Checked;
        }
        private void enableWindowKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stnCrnt.ShowWindows = enableWindowKeysToolStripMenuItem.Checked;
            bool bVisible;
            if (stnCrnt.ShowWindows)
            {
                bVisible = true;
                rtb_Display.Top = 61;
                rtb_Display.Height -= (btn_Win1.Height);
            }
            else
            {
                bVisible = false;
                rtb_Display.Top = btn_Win1.Top;
                rtb_Display.Height += (btn_Win1.Height);
            }
            for (int ind = 0; ind < cnstNumWin; ind++)
            {
                abtnWindows[ind].Visible = bVisible;
            }
            SizeTextDisplay();
        }
        private void SizeTextDisplay()
        {
            int iTop, iButtom;
            iTop = iTop = btn_Win1.Top;
            if (stnCrnt.ShowWindows)
            {
                iTop += (btn_Win1.Height + 10);
            }
            if (showNumberPadToolStripMenuItem.Checked)
            {
                iButtom = btn_7.Top - 10;
            }
            else
            {
                iButtom = btn_0.Top + btn_0.Height;
            }
            rtb_Display.Top = iTop;
            rtb_Display.Height = iButtom - iTop;
            rtb_Display.Width = this.Width - 40;
            rtb_Display.Left = 10;
        }
        private void btn_pad_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            
            if((rtb_Display.Text.Length > 0 ) && (rtb_Display.Text[rtb_Display.Text.Length - 1] == '\n') &&
                ((btn.Text == "*") || (btn.Text == "+") || (btn.Text == "^") || (btn.Text == "/") || (btn.Text == "-") ||
                 (btn.Text == "<<") || (btn.Text == ">>")))
            {
                rtb_Display.AppendText("ANS");
            }
            rtb_Display.AppendText(btn.Text);
            rtb_Display.Focus();
        }
        private void btn_equ_Click(object sender, EventArgs e)
        {
            rtb_Display.AppendText("\n");
            KeyPressEventArgs key = new KeyPressEventArgs((char)13);
            rtb_Display_KeyPress(sender, key);
            rtb_Display.Focus();
        }
        private void btn_lastInput_Click(object sender, EventArgs e)
        {
            lastInputToolStripMenuItem_Click(sender, e);
            rtb_Display.Focus();
        }
        private void btn_ans_Click(object sender, EventArgs e)
        {
            aNSToolStripMenuItem.PerformClick();
            rtb_Display.Focus();
        }
        private void hideButtomPadButtonsVisibility()
        {
            btn_ans.Visible = false;
            btn_lastInput.Visible = false;
            btn_0.Visible = false;
            btn_1.Visible = false;
            btn_2.Visible = false;
            btn_3.Visible = false;
            btn_4.Visible = false;
            btn_5.Visible = false;
            btn_6.Visible = false;
            btn_7.Visible = false;
            btn_8.Visible = false;
            btn_9.Visible = false;
            btn_A.Visible = false;
            btn_B.Visible = false;
            btn_C.Visible = false;
            btn_D.Visible = false;
            btn_E.Visible = false;
            btn_F.Visible = false;
            btn_dot.Visible = false;
            btn_div.Visible = false;
            btn_mul.Visible = false;
            btn_min.Visible = false;
            btn_plus.Visible = false;
            btn_equ.Visible = false;
            btn_mod.Visible = false;
            btn_sftLeft.Visible = false;
            btn_sftRight.Visible = false;
            btn_and.Visible = false;
            btn_or.Visible = false;
            btn_0b.Visible = false;
            btn_0x.Visible = false;
            btn_BackSpace.Visible = false;
            btn_not.Visible = false;
        }
        private void showButtomPadButtonsVisibility()
        {
            btn_ans.Visible = true;
            btn_lastInput.Visible = true;
            btn_0.Visible = true;
            btn_1.Visible = true;
            btn_2.Visible = true;
            btn_3.Visible = true;
            btn_4.Visible = true;
            btn_5.Visible = true;
            btn_6.Visible = true;
            btn_7.Visible = true;
            btn_8.Visible = true;
            btn_9.Visible = true;
            btn_A.Visible = true;
            btn_B.Visible = true;
            btn_C.Visible = true;
            btn_D.Visible = true;
            btn_E.Visible = true;
            btn_F.Visible = true;
            btn_dot.Visible = true;
            btn_div.Visible = true;
            btn_mul.Visible = true;
            btn_min.Visible = true;
            btn_plus.Visible = true;
            btn_equ.Visible = true;
            btn_mod.Visible = true;
            btn_sftLeft.Visible = true;
            btn_sftRight.Visible = true;
            btn_and.Visible = true;
            btn_or.Visible = true;
            btn_0b.Visible = true;
            btn_0x.Visible = true;
            btn_BackSpace.Visible = true;
            btn_not.Visible = true;
        }
        private void cb_showPad_CheckedChanged(object sender, EventArgs e)
        {
            if (BreakLoop == true)
            {
                BreakLoop = false;
                return;
            }
            showNumberPadToolStripMenuItem.PerformClick();
        }
        private void btn_and_Click(object sender, EventArgs e)
        {
            if ((rtb_Display.Text.Length > 0) && (rtb_Display.Text[rtb_Display.Text.Length - 1] == '\n'))
            {
                rtb_Display.AppendText("ANS");
            }
            rtb_Display.AppendText("&");
            rtb_Display.Focus();
        }
        private void btn_or_Click(object sender, EventArgs e)
        {
            if ((rtb_Display.Text.Length > 0) && (rtb_Display.Text[rtb_Display.Text.Length - 1] == '\n'))
            {
                rtb_Display.AppendText("ANS");
            }
            rtb_Display.AppendText("|");
            rtb_Display.Focus();
        }
        private void toolStripRemainderDigits_DropDownClosed(object sender, EventArgs e)
        {
            expression.RemainderDigits = int.Parse(toolStripRemainderDigits.Text);
            settingsToolStripMenuItem.HideDropDown();
        }
        private void toolStripCmbTimeOut_DropDownClosed(object sender, EventArgs e)
        {
            MaxOperationTime = int.Parse(toolStripCmbTimeOut.SelectedItem.ToString());
            toolStripCmbTimeOut.Text = MaxOperationTime.ToString();
            settingsToolStripMenuItem.HideDropDown();
        }
        private void btn_BackSpace_Click(object sender, EventArgs e)
        {
            rtb_Display.Text = rtb_Display.Text.Substring(0, rtb_Display.Text.Length - 1);
            rtb_Display.Select(rtb_Display.Text.Length, 0);
            rtb_Display.Focus();
        }
        private void btn_Win1_Click(object sender, EventArgs e)
        {
            SwitchWindow(0);
        }
        private void btn_Win2_Click(object sender, EventArgs e)
        {
            SwitchWindow(1);
        }
        private void btn_Win3_Click(object sender, EventArgs e)
        {
            SwitchWindow(2);
        }
        private void btn_Win4_Click(object sender, EventArgs e)
        {
            SwitchWindow(3);
        }
        private void bkw_DoWork(object sender, DoWorkEventArgs e)
        {

        }
        private void AdjustColors( )
        {
            rtb_Display.BackColor = clrthActive.Background;
            for (int i = 0; i < rtb_Display.Text.Length; i++)
            {
                rtb_Display.Select(i, 1);
                if (rtb_Display.SelectionColor == clrthInactive.Input) rtb_Display.SelectionColor = clrthActive.Input;
                if (rtb_Display.SelectionColor == clrthInactive.Answer) rtb_Display.SelectionColor = clrthActive.Answer;
                if (rtb_Display.SelectionColor == clrthInactive.BitIndex) rtb_Display.SelectionColor = clrthActive.BitIndex;
            }
            rtb_Display.Select(rtb_Display.Text.Length, 0);
            rtb_Display.SelectionColor = clrthActive.Input;
            rtb_Display.Focus();
            /* 
            rtb_Display.BackColor = clrthActive.Background;
            rtb_Display.SelectAll();
            rtb_Display.SelectionColor = clrthActive.Input;
            rtb_Display.Select(rtb_Display.Text.Length, 0);
            rtb_Display.SelectionColor = clrthActive.Input; */
        }
        private void UserDefMenuItem_Click(object sender, EventArgs e)
        {
            UserDefinedExpression udExp;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            frmUsrDefinedExpr frmUsrDef = new frmUsrDefinedExpr();
            frmUsrDef.UsrDefExp = new UserDefinedExpression();
            frmUsrDef.UsrDefExp.Name = "Exp" + stnCrnt.NumUserDef;
            frmUsrDef.ShowDialog();
            udExp = frmUsrDef.UsrDefExp;
            if (udExp.InputRegEx == null) return;
            for( int indx = 0; indx < stnCrnt.NumUserDef; indx++ )
            {
                if (stnCrnt.UsrDefExprs[indx].Name == udExp.Name)
                {
                    MessageBox.Show(udExp.Name + " Already exists.");
                    return;
                }
            }
            stnCrnt.UsrDefExprs[stnCrnt.NumUserDef] = udExp;
            stnCrnt.NumUserDef++;
            AddUserDefinedToMenu(udExp);
        }
        private void UserDefinedItemEdit(object sender, EventArgs e)
        {
            ToolStripMenuItem itemSender = (ToolStripMenuItem)sender;
            UserDefinedExpression udeEdit = (UserDefinedExpression)itemSender.Tag;
            frmUsrDefinedExpr frmUsrDef = new frmUsrDefinedExpr();
            frmUsrDef.UsrDefExp = udeEdit;
            DialogResult dr = frmUsrDef.ShowDialog();

            if (dr == DialogResult.Cancel) return;

            for (int indx = 0; indx < stnCrnt.NumUserDef; indx++)
            {
                UserDefinedExpression udExp = (UserDefinedExpression)stnCrnt.UsrDefExprs[indx];
                if (udExp.Name == udeEdit.Name)
                {

                    ToolStripItem tsiUsrDefEdit = tlItmUD.DropDownItems[indx + NumUsrDefManuItemsBeforeExprssions];
                    tsiUsrDefEdit.Text = udeEdit.Name;
                    return;
                }
            }
        }
        private void AddUserDefinedToMenu(UserDefinedExpression udExp)
        {
            tlItmUD.DropDownItems.Add(udExp.Name);
            ToolStripMenuItem newTlStrItm = (ToolStripMenuItem)tlItmUD.DropDownItems[tlItmUD.DropDownItems.Count - 1];
            newTlStrItm.DropDownItems.Add("Use");
            newTlStrItm.DropDownItems[0].Tag = udExp;
            newTlStrItm.DropDownItems[0].Click += new System.EventHandler(this.UserDefinedItemUse);
            newTlStrItm.DropDownItems.Add("Edit");
            newTlStrItm.DropDownItems[1].Tag = udExp;
            newTlStrItm.DropDownItems[1].Click += new System.EventHandler(this.UserDefinedItemEdit);
            newTlStrItm.DropDownItems.Add("Delete");
            newTlStrItm.DropDownItems[2].Tag = udExp;
            newTlStrItm.DropDownItems[2].Click += new System.EventHandler(this.UserDefinedItemDelete);
            if(stnCrnt.NumUserDef >= MaxUserDefExp )
            {
                newToolStripMenuItem.Enabled = false;
            }
        }
        private void UserDefinedItemDelete(object sender, EventArgs e)
        {
            ToolStripMenuItem itemSender = (ToolStripMenuItem)sender;
            UserDefinedExpression udeDelete = (UserDefinedExpression)itemSender.Tag;

            for( int indx = 0; indx < stnCrnt.NumUserDef; indx++ )
            {
                UserDefinedExpression udExp = (UserDefinedExpression)stnCrnt.UsrDefExprs[indx];
                if( udExp.Name == udeDelete.Name )
                {
                    for( int indexRemove = ( indx + 1 ); indexRemove < stnCrnt.NumUserDef; indexRemove++ )
                    {
                        stnCrnt.UsrDefExprs[indexRemove - 1] = stnCrnt.UsrDefExprs[indexRemove];
                    }
                    tlItmUD.DropDownItems.RemoveAt(indx + NumUsrDefManuItemsBeforeExprssions);
                    stnCrnt.NumUserDef--;
                    stnCrnt.UsrDefExprs[stnCrnt.NumUserDef] = null;
                    newToolStripMenuItem.Enabled = true;
                    return;
                }
            }
        }
        private void UserDefinedItemUse(object sender, EventArgs e)
        {
            ToolStripMenuItem itemSender = (ToolStripMenuItem)sender;
            UserDefinedExpression udeUse = (UserDefinedExpression)itemSender.Tag;
            rtb_Display.AppendText(udeUse.Name+"()");
        }
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofdUserDefined.ShowDialog();
            if (dr == DialogResult.Cancel) return;
            StreamReader sr = new StreamReader(ofdUserDefined.FileName);
            bool bFoundSameName;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                UserDefinedExpression udExp = new UserDefinedExpression(line);
                if ((udExp.Name != null) && (udExp.InputRegEx != null) && (udExp.Function != null))
                {
                    bFoundSameName = false;
                    for (int indx = 0; indx < stnCrnt.NumUserDef; indx++)
                    {
                        if (stnCrnt.UsrDefExprs[indx].Name == udExp.Name)
                        {
                            bFoundSameName = true;
                            break; ;
                        }
                    }
                    if (bFoundSameName) continue;
                    stnCrnt.UsrDefExprs[stnCrnt.NumUserDef] = udExp;
                    stnCrnt.NumUserDef++;
                    AddUserDefinedToMenu(udExp);
                }
            }
        }
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stnCrnt.NumUserDef == 0) return;
            DialogResult dr = sfdUserDefined.ShowDialog();
            if (dr == DialogResult.Cancel) return;
            StreamWriter sw = new StreamWriter(sfdUserDefined.FileName);
            for(int indUD = 0; indUD < stnCrnt.NumUserDef; indUD++)
            {
                UserDefinedExpression udExp = stnCrnt.UsrDefExprs[indUD];
                sw.WriteLine(udExp);
            }
            sw.Close();
        }

        private void tlTpCalc_Popup(object sender, PopupEventArgs e)
        {

        }

        private void colorThemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorThemToolStripMenuItem.Checked)
            {
                stnCrnt.clrThemEnum = ColorThemeEnum.WhiteOnBlack;
                clrthActive.SetThemWhiteOnBlack();
                clrthInactive.SetThemBlackOnWhite();
            }
            else
            {
                stnCrnt.clrThemEnum = ColorThemeEnum.BlackOnWhite;
                clrthActive.SetThemBlackOnWhite();
                clrthInactive.SetThemWhiteOnBlack();
            }
            AdjustColors();
        }

        private void Calculator_Activated(object sender, EventArgs e)
        {
            if(!IsOnScreen(this)) this.CenterToScreen();
        }

        private void rtb_Display_TextChanged(object sender, EventArgs e)
        {

        }
        private void btn_xor_Click(object sender, EventArgs e)
        {
            if ((rtb_Display.Text.Length > 0) && (rtb_Display.Text[rtb_Display.Text.Length - 1] == '\n'))
            {
                rtb_Display.AppendText("ANS");
            }
            rtb_Display.AppendText("XOR");
            rtb_Display.Focus();
        }
        private void btn_not_Click(object sender, EventArgs e)
        {
            if ((rtb_Display.Text.Length > 0) && (rtb_Display.Text[rtb_Display.Text.Length - 1] == '\n'))
            {
                rtb_Display.AppendText("ANS");
            }
            rtb_Display.AppendText("~");
            rtb_Display.Focus();
        }
        public bool IsOnScreen(Form form)
        {
            Screen[] screens = Screen.AllScreens;
            foreach (Screen screen in screens)
            {
                Rectangle formRectangle = new Rectangle(form.Left, form.Top, form.Width, form.Height);

                if (screen.WorkingArea.Contains(formRectangle))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public enum CalculatorState
    {
        Initializing,
        WaitingForInput,
        CulculatingResult,
        CulculatingFormats
    }

    public enum ColorThemeEnum
    {
        BlackOnWhite,
        WhiteOnBlack
    }

    public class ColorTheme
    {
        public Color Background;
        public Color Input;
        public Color Answer;
        public Color BitIndex;

        public ColorTheme()
        {
            SetThemBlackOnWhite();
        }
        public void SetThemBlackOnWhite()
        {
            Background = Color.White;
            Input = Color.Black;
            Answer = Color.Blue;
            BitIndex = Color.Red;
        }
        public void SetThemWhiteOnBlack()
        {
            Background = Color.Black;
            Input = Color.White;
            Answer = Color.Yellow;
            BitIndex = Color.LightGreen;
        }
    }

    public class UserDefinedExpression
    {
        public string Name;
        public string Input;
        public string InputRegEx;
        public string Function;
        static Regex rxRecognize = new Regex(@"(?<Name>.+): RegEx: (?<InputRegEx>.+)   Function: (?<Function>.+)");
        public UserDefinedExpression()
        {
            return;
        }
        public UserDefinedExpression( String strIn )
        {
            Match mtch = rxRecognize.Match(strIn);
            if (!mtch.Success) return;
            if (mtch.Groups["Name"].Length > 1) Name = mtch.Groups["Name"].ToString();
            if (mtch.Groups["InputRegEx"].Length > 1) InputRegEx = mtch.Groups["InputRegEx"].ToString();
            if (mtch.Groups["Function"].Length > 1) Function = mtch.Groups["Function"].ToString();
        }
        public override string ToString()
        {
            return Name + ": RegEx: " + InputRegEx + "   Function: " + Function;
        }
    }
}