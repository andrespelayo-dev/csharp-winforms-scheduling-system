using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ScheduleApp;
using System.Xml.Linq;

namespace C969schedule
{
    public partial class UpdateCustomerForm : Form
    {
        private int _customerId;
        private int _addressId;
        private int _cityId;
        private int _countryId;
        public UpdateCustomerForm()
        {
            InitializeComponent();
        }
        public UpdateCustomerForm(int customerId) : this()
        {
            _customerId = customerId;
            LoadCustomerData();
        }
        private void LoadCustomerData()
        {
            string sql = @"
        SELECT
            customer.customerName,
            customer.addressId,
            address.address,
            address.phone,
            address.cityId,
            city.city,
            city.countryId,
            country.country
        FROM customer
        JOIN address ON customer.addressId = address.addressId
        JOIN city ON address.cityId = city.cityId
        JOIN country ON city.countryId = country.countryId
        WHERE customer.customerId = @id
    ";

            DataTable dt = Database.Query(sql,
                new MySqlParameter("@id", _customerId));

            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];

                txtName.Text = row["customerName"].ToString();
                txtAddress.Text = row["address"].ToString();
                txtPhone.Text = row["phone"].ToString();
                txtCity.Text = row["city"].ToString();
                txtCountry.Text = row["country"].ToString();

                _addressId = Convert.ToInt32(row["addressId"]);
                _cityId = Convert.ToInt32(row["cityId"]);
                _countryId = Convert.ToInt32(row["countryId"]);
            }
            else
            {
                MessageBox.Show("Unable to load customer data.");
                this.Close();
            }
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
                    "UPDATE country SET country = @country, lastUpdate = UTC_TIMESTAMP(), lastUpdateBy = 'user' WHERE countryId = @id",
                    new MySqlParameter("@country", country),
                    new MySqlParameter("@id", _countryId)
                );

                Database.Execute(
                    "UPDATE city SET city = @city, lastUpdate = UTC_TIMESTAMP(), lastUpdateBy = 'user' WHERE cityId = @id",
                    new MySqlParameter("@city", city),
                    new MySqlParameter("@id", _cityId)
                );

                Database.Execute(
                    "UPDATE address SET address = @address, phone = @phone, lastUpdate = UTC_TIMESTAMP(), lastUpdateBy = 'user' WHERE addressId = @id",
                    new MySqlParameter("@address", address),
                    new MySqlParameter("@phone", phone),
                    new MySqlParameter("@id", _addressId)
                );

                Database.Execute(
                    "UPDATE customer SET customerName = @name, lastUpdate = UTC_TIMESTAMP(), lastUpdateBy = 'user' WHERE customerId = @id",
                    new MySqlParameter("@name", name),
                    new MySqlParameter("@id", _customerId)
                );

                MessageBox.Show("Customer updated successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating customer:\n" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
