using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ScheduleApp;

namespace C969schedule
{
    public partial class CustomerForm : Form
    {
        public CustomerForm()
        {
            InitializeComponent();
        }

        private void CustomerForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();

        }
        private void LoadCustomers()
        {
            string sql = @"SELECT customer.customerId, customer.customerName, 
                          address.address, address.phone, 
                          city.city, country.country
                   FROM customer
                   JOIN address ON customer.addressId = address.addressId
                   JOIN city ON address.cityId = city.cityId
                   JOIN country ON city.countryId = country.countryId";

            dgvCustomers.DataSource = Database.Query(sql);
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            AddCustomerForm add = new AddCustomerForm();
            add.FormClosed += (s, args) => LoadCustomers();
            add.Show();
        }

        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow == null)
            {
                MessageBox.Show("Please select a customer to update.");
                return;
            }

            int id = Convert.ToInt32(dgvCustomers.CurrentRow.Cells["customerId"].Value);

            UpdateCustomerForm update = new UpdateCustomerForm(id);
            update.FormClosed += (s, args) => LoadCustomers();
            update.Show();
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow == null)
            {
                MessageBox.Show("Please select a customer to delete.");
                return;
            }

            int id = Convert.ToInt32(dgvCustomers.CurrentRow.Cells["customerId"].Value);

            if (MessageBox.Show("Are you sure you want to delete this customer?",
                                "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    string sql = "DELETE FROM customer WHERE customerId = @id";
                    Database.Execute(sql, new MySqlParameter("@id", id));
                    LoadCustomers();
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        "This customer cannot be deleted because they have existing appointments.\n" +
                        "Please delete their appointments first.",
                        "Delete Not Allowed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
