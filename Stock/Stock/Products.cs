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
    public partial class Products : Form
    {
        public Products()
        {
            InitializeComponent();
        }

        private void Products_Load(object sender, EventArgs e)
        {
            ResetRecords();
            LoadData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //SqlConnection con = new SqlConnection("Data Source=DESKTOP-BNPDU0F;Initial Catalog=Stock;User ID=sa;Password=hanlintun");
            if (Validation())
            {
                SqlConnection con = Connection.GetConnection();
                con.Open();

                bool status = false;
                if (cbbProductStatus.SelectedIndex == 0)
                {
                    status = true;
                }
                // Insert Logic
                var sqlQuery = "";
                if (IfProductExist(con, txtProductCode.Text))
                {
                    sqlQuery = @"UPDATE [dbo].[Product]  SET [ProductName] = '" + txtProductName.Text + "' ,[ProductStatus] = '" + status + "' WHERE [ProductCode] = '" + txtProductCode.Text + "'";
                }
                else
                {
                    sqlQuery = @"INSERT INTO [dbo].[Product] ([ProductCode],[ProductName],[ProductStatus]) VALUES ('" + txtProductCode.Text + "','" + txtProductName.Text + "','" + status + "')";
                }

                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                cmd.ExecuteNonQuery();
                con.Close();

                LoadData();
                ResetRecords();
            }
            else
            {
                MessageBox.Show("Sorry, your data is invalid.", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }
        private bool IfProductExist(SqlConnection con, string productCode)
        {
            SqlDataAdapter sda = new SqlDataAdapter(@"SELECT 1 FROM [dbo].[Product] where [ProductCode] = '" + productCode +"'", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void LoadData()
        {
            // Reading Data
            SqlConnection con = Connection.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("Select * From [dbo].[Product]", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            dgvProducts.Rows.Clear();
            foreach (DataRow item in dt.Rows)
            {
                int n = dgvProducts.Rows.Add();
                dgvProducts.Rows[n].Cells[0].Value = item["ProductCode"].ToString();
                dgvProducts.Rows[n].Cells[1].Value = item["ProductName"].ToString();
                //string statusResult = ((int)item["ProductStatus"] == 1) ? "Active" : "Deactive";
                //dgvProducts.Rows[n].Cells[2].Value = statusResult.ToString();
                if ((bool)item["ProductStatus"])
                {
                    dgvProducts.Rows[n].Cells[2].Value = "Active";
                }
                else
                {
                    dgvProducts.Rows[n].Cells[2].Value = "Deactive";
                }
            }
        }

        private void dgvProducts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            txtProductCode.Enabled = false;

            btnAdd.Text = "Update";
            btnDelete.Visible = true;

            txtProductCode.Text = dgvProducts.SelectedRows[0].Cells[0].Value.ToString();
            txtProductName.Text = dgvProducts.SelectedRows[0].Cells[1].Value.ToString();
            if ((string)dgvProducts.SelectedRows[0].Cells[2].Value == "Active")
            {
                cbbProductStatus.SelectedIndex = 0;
            }
            else
            {
                cbbProductStatus.SelectedIndex = 1;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure want to Delete?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                SqlConnection con = Connection.GetConnection();
                con.Open();
                if (IfProductExist(con, txtProductCode.Text))
                {
                    var sqlQuery = "DELETE FROM [dbo].[Product] WHERE [ProductCode] = '" + txtProductCode.Text + "'";
                    SqlCommand cmd = new SqlCommand(sqlQuery, con);
                    cmd.ExecuteNonQuery();
                    con.Close();

                    ResetRecords();
                }
                else
                {
                    MessageBox.Show("Sorry! Your product code is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                LoadData(); 
            }
        }
        private void ResetRecords()
        {
            txtProductCode.Enabled = true;

            txtProductCode.Text = "";
            txtProductName.Text = "";
            cbbProductStatus.SelectedIndex = -1;

            btnAdd.Text = "Add";
            btnDelete.Visible = false;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetRecords();

            LoadData();
        }

        private bool Validation()
        {
            bool checkResult = false;
            if (string.IsNullOrEmpty(txtProductCode.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(txtProductCode, "Product Code is required.");
            }
            else if (string.IsNullOrEmpty(txtProductName.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(txtProductName, "Product Name is required.");
            }
            else if (cbbProductStatus.SelectedIndex == -1)
            {
                errorProvider1.Clear();
                errorProvider1.SetError(cbbProductStatus, "Select a stutas");
            }
            else
            {
                errorProvider1.Clear();
                checkResult = true;
            }
            return checkResult;
        }
    }
}
