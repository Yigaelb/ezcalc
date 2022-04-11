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
    public partial class frmSimpleExample : Form
    {
        public frmSimpleExample()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void frmSimpleExample_Load(object sender, EventArgs e)
        {

            rtbExample.Text = 
@"Name:                       Example1
Regular Expression:  (\w*),(\w*),(\w*),(\w*)
Function:                   X1^X2+X3*X4
Use:                          Example1(2,3,4,5)
Parsed expression:    2^3+4*5
Result:                       28";
            rtbExample.ReadOnly = true;
        }
    }
}
