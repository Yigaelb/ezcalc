namespace EZCalc
{
    partial class frmUsrDefinedExpr
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rtbExpIn = new System.Windows.Forms.RichTextBox();
            this.rtbExpOut = new System.Windows.Forms.RichTextBox();
            this.lblExpOut = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txbName = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblInput = new System.Windows.Forms.LinkLabel();
            this.btnSimpleExample = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 278);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(107, 278);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rtbExpIn
            // 
            this.rtbExpIn.Location = new System.Drawing.Point(12, 93);
            this.rtbExpIn.Name = "rtbExpIn";
            this.rtbExpIn.Size = new System.Drawing.Size(414, 54);
            this.rtbExpIn.TabIndex = 2;
            this.rtbExpIn.Text = "Col: (\\d+) Row: (\\d+) Bank: (\\d+) Offset: (\\d+)";
            this.rtbExpIn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbExpIn_KeyDown);
            // 
            // rtbExpOut
            // 
            this.rtbExpOut.Location = new System.Drawing.Point(12, 197);
            this.rtbExpOut.Name = "rtbExpOut";
            this.rtbExpOut.Size = new System.Drawing.Size(414, 55);
            this.rtbExpOut.TabIndex = 3;
            this.rtbExpOut.Text = "X1*10+X2*20+X3*8+X4";
            this.rtbExpOut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbExpOut_KeyDown);
            // 
            // lblExpOut
            // 
            this.lblExpOut.AutoSize = true;
            this.lblExpOut.Location = new System.Drawing.Point(13, 172);
            this.lblExpOut.Name = "lblExpOut";
            this.lblExpOut.Size = new System.Drawing.Size(356, 13);
            this.lblExpOut.TabIndex = 2;
            this.lblExpOut.Text = "Function - Each token recognized in input will replace an Xi in the function";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 9);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(89, 13);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Expression Name";
            this.lblName.Click += new System.EventHandler(this.label1_Click);
            // 
            // txbName
            // 
            this.txbName.Location = new System.Drawing.Point(12, 25);
            this.txbName.Name = "txbName";
            this.txbName.Size = new System.Drawing.Size(100, 20);
            this.txbName.TabIndex = 1;
            this.txbName.Text = "Exp1";
            this.txbName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbName_KeyPress);
            // 
            // toolTip1
            // 
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(13, 62);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(273, 13);
            this.lblInput.TabIndex = 6;
            this.lblInput.TabStop = true;
            this.lblInput.Text = "Input regular expression - Used to parse expression input";
            this.lblInput.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblInput_LinkClicked);
            // 
            // btnSimpleExample
            // 
            this.btnSimpleExample.Location = new System.Drawing.Point(278, 278);
            this.btnSimpleExample.Name = "btnSimpleExample";
            this.btnSimpleExample.Size = new System.Drawing.Size(138, 23);
            this.btnSimpleExample.TabIndex = 4;
            this.btnSimpleExample.Text = "Show Simple Example";
            this.btnSimpleExample.UseVisualStyleBackColor = true;
            this.btnSimpleExample.Click += new System.EventHandler(this.btnSimpleExample_Click);
            // 
            // frmUsrDefinedExpr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 331);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.txbName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblExpOut);
            this.Controls.Add(this.rtbExpOut);
            this.Controls.Add(this.rtbExpIn);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSimpleExample);
            this.Controls.Add(this.btnOK);
            this.Name = "frmUsrDefinedExpr";
            this.Text = "User Defined Expression Editor";
            this.Load += new System.EventHandler(this.frmUsrDefinedExpr_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RichTextBox rtbExpIn;
        private System.Windows.Forms.RichTextBox rtbExpOut;
        private System.Windows.Forms.Label lblExpOut;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txbName;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel lblInput;
        private System.Windows.Forms.Button btnSimpleExample;
    }
}