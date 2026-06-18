using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScheduleApp;

namespace C969schedule
{
    public partial class AddCustomerForm : Form
    {
        public AddCustomerForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string address = txtAddress.Text.Trim();
            string city = txtCity.Text.Trim();
            string country = txtCountry.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (name == "" || address == "" || city == "" || country == "" || phone == "")
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9\-]+$"))
            {
                MessageBox.Show("Phone number can contain only digits and dashes.");
                return;
            }

            try
            {
                Database.Execute(
                    "INSERT INTO country (country, createDate, createdBy, lastUpdateBy) VALUES (@c, UTC_TIMESTAMP(), 'user', 'user')",
                    new MySql.Data.MySqlClient.MySqlParameter("@c", country)
                );
                int countryId = Convert.ToInt32(Database.Query("SELECT LAST_INSERT_ID()").Rows[0][0]);

                Database.Execute(
                    "INSERT INTO city (city, countryId, createDate, createdBy, lastUpdateBy) VALUES (@city, @cid, UTC_TIMESTAMP(), 'user', 'user')",
                    new MySql.Data.MySqlClient.MySqlParameter("@city", city),
                    new MySql.Data.MySqlClient.MySqlParameter("@cid", countryId)
                );
                int cityId = Convert.ToInt32(Database.Query("SELECT LAST_INSERT_ID()").Rows[0][0]);

                Database.Execute(
      "INSERT INTO address (address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy) " +
      "VALUES (@a, 'N/A', @cid, '00000', @p, NOW(), 'test', NOW(), 'test')",
      new MySql.Data.MySqlClient.MySqlParameter("@a", address),
      new MySql.Data.MySqlClient.MySqlParameter("@p", phone),
      new MySql.Data.MySqlClient.MySqlParameter("@cid", cityId)
  );

                int addressId = Convert.ToInt32(Database.Query("SELECT LAST_INSERT_ID()").Rows[0][0]);

                Database.Execute(
     "INSERT INTO customer (customerName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy) " +
     "VALUES (@n, @aid, 1, NOW(), 'test', NOW(), 'test')",
     new MySql.Data.MySqlClient.MySqlParameter("@n", name),
     new MySql.Data.MySqlClient.MySqlParameter("@aid", addressId)
 );


                MessageBox.Show("Customer added successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding customer:\n" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
