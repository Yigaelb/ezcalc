using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EZCalc
{
    public partial class frmUsrDefinedExpr : Form
    {
        public UserDefinedExpression UsrDefExp;
        public frmUsrDefinedExpr()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmUsrDefinedExpr_Load(object sender, EventArgs e)
        {
            txbName.Text = UsrDefExp.Name;
            if (UsrDefExp.InputRegEx != null) rtbExpIn.Text = UsrDefExp.InputRegEx;
            if (UsrDefExp.Function != null) rtbExpOut.Text = UsrDefExp.Function;

            toolTip1.SetToolTip(rtbExpIn, "This box is used to help the calculator understand the input, allowing user to copy/paste from files.\nBy default there is an example in the text box. \\d - number, \\w - alphanumeric (useful for hex).");
            toolTip1.SetToolTip(lblInput, "This box is used to help the calculator understand the input, allowing user to copy/paste from files.\nBy default there is an example in the text box. \\d - number, \\w - alphanumeric (useful for hex).");
            toolTip1.SetToolTip(rtbExpOut, "This box shows how the calculation will be done. \nBy default there is an example. Each Xi will replace what is inside the () in the box above by order.\nThere is no need to use all the indeces");
            toolTip1.SetToolTip(lblExpOut, "This box shows how the calculation will be done. \nBy default there is an example. Each Xi will replace what is inside the () in the box above by order.\nThere is no need to use all the indeces");
            lblInput.Links.Add(6, 18, "https://www.regex101.com/#python");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            UsrDefExp.Name = txbName.Text;
            UsrDefExp.InputRegEx = rtbExpIn.Text;
            UsrDefExp.Function = rtbExpOut.Text;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSimpleExample_Click(object sender, EventArgs e)
        {
            frmSimpleExample SmpExmpl = new frmSimpleExample();
            SmpExmpl.ShowDialog();
        }

        private void txbName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') // Ignore any spaces that are entered
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                rtbExpIn.Focus();
            }
        }

        private void rtbExpIn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                rtbExpOut.Focus();
            }
        }

        private void rtbExpOut_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                btnOK.Focus();
            }
        }

        private void lblInput_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}
