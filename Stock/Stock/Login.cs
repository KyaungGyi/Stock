using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUserName.Text = "";
            txtPassword.Clear();

            txtUserName.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // To-Do: Check Login username and password

            SqlConnection con = Connection.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter(@"SELECT *
                                                    FROM [dbo].[Login] where UserName = '" + txtUserName.Text + "' and [Password]='" + txtPassword.Text + "'", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            if (dt.Rows.Count == 1)
                {
                    this.Hide();
                    StockMain main = new StockMain();
                    main.Show();
                }
            else
            {
                MessageBox.Show("Invalid UserName and Password...!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
