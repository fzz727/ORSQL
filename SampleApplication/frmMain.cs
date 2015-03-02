using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SampleApplication
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            models.UserInfo user = new models.UserInfo() { Created = DateTime.Now, UserName = "aaaa" };

            user = OR.DAL.Add<models.UserInfo>(user, true);

        }
    }
}
