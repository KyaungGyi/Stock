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
    public partial class Stock : Form
    {
        public Stock()
        {
            InitializeComponent();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void Stock_Load(object sender, EventArgs e)
        {
            this.ActiveControl = dtpStock;
            cbbStock.SelectedIndex = 0;

            btnDelete.Visible = false;
            LoadData();
        }

        private void dtpStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtProductCode.Focus();
            }
        }

        private void txtProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtProductCode.Text.Length > 0)
                {
                    txtProductName.Focus();
                }
                else
                {
                    txtProductCode.Focus();
                }
            }
        }

        private void txtProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtProductName.Text.Length > 0)
                {
                    txtProductQuantity.Focus();
                }
                else
                {
                    txtProductName.Focus();
                }
            }
        }

        private void txtProductQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtProductQuantity.Text.Length > 0)
                {
                    cbbStock.Focus();
                }
                else
                {
                    txtProductQuantity.Focus();
                }
            }
        }

        private void cbbStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cbbStock.SelectedIndex != -1)
                {
                    btnAdd.Focus();
                }
                else
                {
                    cbbStock.Focus();
                }
            }
        }

        private void txtProductCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back & e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtProductQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void ResetRecords()
        {
            errorProvider1.Clear();

            dtpStock.Value = DateTime.Now;
            txtProductCode.Enabled = true;
            txtProductCode.Clear();
            txtProductName.Clear();
            txtProductQuantity.Clear();
            cbbStock.SelectedIndex = -1;


            btnAdd.Text = "Add";
            btnDelete.Visible = false;
            dtpStock.Focus();

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetRecords();
            LoadData();
        }
        private bool Validation()
        {
            bool check = false;
            if (string.IsNullOrEmpty(txtProductCode.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(txtProductCode, "Product code is required.");
            }
            else if (string.IsNullOrEmpty(txtProductName.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(txtProductName, "Product name is required.");
            }
            else if (string.IsNullOrEmpty(txtProductQuantity.Text))
            {
                errorProvider1.Clear();
                errorProvider1.SetError(txtProductQuantity, "Product Quantity is required.");
            }
            else if (cbbStock.SelectedIndex == -1)
            {
                errorProvider1.Clear();
                errorProvider1.SetError(cbbStock, "Status is required.");
            }
            else
            {
                errorProvider1.Clear();
                check = true;
            }
            return check;
        }
        private bool IfProductExist(SqlConnection con, string productCode)
        {
            SqlDataAdapter sda = new SqlDataAdapter("SELECT 1 FROM [dbo].[Stock] Where [ProductCode] = '" + productCode + "'", con);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (Validation())
            {
                bool status = false;
                if (cbbStock.SelectedIndex == 0)
                {
                    status = true;
                }

                SqlConnection con = Connection.GetConnection();
                con.Open();
                var sqlQuery = "";
                if (IfProductExist(con, txtProductCode.Text))
                {
                    sqlQuery = @"UPDATE [dbo].[Stock]
                                        SET [ProductName] = '" + txtProductName.Text + "', [TransDate] =  '" + dtpStock.Value.ToString("MM/dd/yyyy") + "' ,[Quantity] =  '" + txtProductQuantity.Text + "' ,[ProductStatus] =  '" + status + "' WHERE [ProductCode] = '" + txtProductCode.Text + "'";
                }
                else
                {
                    sqlQuery = @"INSERT INTO [dbo].[Stock]
                                       ([ProductCode]
                                       ,[ProductName]
                                       ,[TransDate]
                                       ,[Quantity]
                                       ,[ProductStatus])
                                 VALUES
                                       ('" + txtProductCode.Text + "','" + txtProductName.Text +"' ,'" + dtpStock.Value.ToString("MM/dd/yyyy")+ "' , '" + txtProductQuantity.Text + "', '" + status + "')";
                }
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Saved your record successfully!.","Saved Message");
                ResetRecords();

                LoadData();
            }
            else
            {
                MessageBox.Show("Your data is invalid.Please correpct and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void LoadData()
        {
            SqlConnection con = Connection.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("Select * From [dbo].[Stock]", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            dgvStock.Rows.Clear();
            foreach (DataRow each in dt.Rows)
            {
                int n = dgvStock.Rows.Add();
                dgvStock.Rows[n].Cells["dgSNo"].Value = n + 1;
                dgvStock.Rows[n].Cells["dgCode"].Value = each["ProductCode"].ToString();
                dgvStock.Rows[n].Cells["dgName"].Value = each["ProductName"].ToString();
                dgvStock.Rows[n].Cells["dgDate"].Value = Convert.ToDateTime(each["TransDate"].ToString()).ToString("MM/dd/yyyy");
                dgvStock.Rows[n].Cells["dgQuantity"].Value = each["Quantity"].ToString();
                if ((bool)each["ProductStatus"])
                {
                    dgvStock.Rows[n].Cells["dgStatus"].Value = "Active";
                }
                else
                {
                    dgvStock.Rows[n].Cells["dgStatus"].Value = "Deactive";                    
                }
            }
            if (dt.Rows.Count > 0)
            {
                lblTotalProducts.Text = dgvStock.Rows.Count.ToString();
                float totalQty = 0F;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    totalQty += float.Parse(dgvStock.Rows[i].Cells["dgQuantity"].Value.ToString());
                }
                lblTotalQuantity.Text = totalQty.ToString();
            }
            else
            {
                lblTotalProducts.Text = "0";
                lblTotalQuantity.Text = "0";
            }
        }

        private void dgvStock_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnAdd.Text = "Update";
            btnDelete.Visible = true;

            txtProductCode.Enabled = false;

            txtProductCode.Text = dgvStock.SelectedRows[0].Cells["dgCode"].Value.ToString();
            txtProductName.Text = dgvStock.SelectedRows[0].Cells["dgName"].Value.ToString();
            txtProductQuantity.Text = dgvStock.SelectedRows[0].Cells["dgQuantity"].Value.ToString();
            dtpStock.Value = Convert.ToDateTime(dgvStock.SelectedRows[0].Cells["dgDate"].Value);
            if (dgvStock.SelectedRows[0].Cells["dgStatus"].Value.ToString() == "Active")
            {
                cbbStock.SelectedIndex = 0;
            }
            else
            {
                cbbStock.SelectedIndex = 1;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SqlConnection con = Connection.GetConnection();
            if (!string.IsNullOrEmpty(txtProductCode.Text) && IfProductExist(con, txtProductCode.Text))
            {
                DialogResult dialogResult = MessageBox.Show("Sure to Delete?", "Delete Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    //SqlConnection con = Connection.GetConnection();
                    con.Open();
                    SqlCommand cmd= new SqlCommand("Delete From [dbo].[Stock] Where [ProductCode] = '" + txtProductCode.Text + "'", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    LoadData();
                    ResetRecords();

                    MessageBox.Show("Deleted data successfully!");
                }
            }
            else
            {
                MessageBox.Show("Your record is not valid, please try again by reset.");
            }
        }
    }
}
