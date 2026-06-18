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
    public partial class AddAppointmentForm : Form
    {
        public AddAppointmentForm()
        {
            InitializeComponent();
        }

        private void AddAppointmentForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();

        }
        private void LoadCustomers()
        {
            string sql = "SELECT customerId, customerName FROM customer";
            DataTable dt = Database.Query(sql);

            cmbCustomer.DataSource = dt;
            cmbCustomer.DisplayMember = "customerName";
            cmbCustomer.ValueMember = "customerId";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int customerId = Convert.ToInt32(cmbCustomer.SelectedValue);
            string type = txtType.Text.Trim();

            DateTime startLocal = dtpStart.Value;
            DateTime endLocal = dtpEnd.Value;

            if (type == "")
            {
                MessageBox.Show("Appointment type is required.");
                return;
            }

            if (endLocal <= startLocal)
            {
                MessageBox.Show("End time must be after start time.");
                return;
            }
            
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            DateTime startEST = TimeZoneInfo.ConvertTime(startLocal, TimeZoneInfo.Local, estZone);
            DateTime endEST = TimeZoneInfo.ConvertTime(endLocal, TimeZoneInfo.Local, estZone);

            // Check business hours
            if (startEST.Hour < 9 || startEST.Hour > 17 ||
                endEST.Hour < 9 || endEST.Hour > 17 ||
                startEST.DayOfWeek == DayOfWeek.Saturday ||
                startEST.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Appointment must be within business hours (Mon–Fri, 9am–5pm EST).");
                return;
            }
            DateTime startUTC = startLocal.ToUniversalTime();
            DateTime endUTC = endLocal.ToUniversalTime();

            string sqlOverlap = @"
    SELECT * FROM appointment
    WHERE customerId = @cid
    AND (
          (@start BETWEEN start AND end)
       OR (@end BETWEEN start AND end)
       OR (start BETWEEN @start AND @end)
        )";

            DataTable overlap = Database.Query(sqlOverlap,
                new MySqlParameter("@cid", customerId),
                new MySqlParameter("@start", startUTC),
                new MySqlParameter("@end", endUTC)
            );

            if (overlap.Rows.Count > 0)
            {
                MessageBox.Show("This appointment overlaps with an existing appointment.");
                return;
            }
            try
            {
                string sql = @"
INSERT INTO appointment
(customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdate, lastUpdateBy)
VALUES
(@cid, @uid, 'Not Needed', 'Not Needed', 'Online', 'Unassigned', @type, 'N/A', @startUTC, @endUTC, NOW(), 'test', NOW(), 'test')";

                Database.Execute(sql,
                    new MySqlParameter("@cid", customerId),
                    new MySqlParameter("@uid", 1),
                    new MySqlParameter("@type", type),
                    new MySqlParameter("@startUTC", startUTC),
                    new MySqlParameter("@endUTC", endUTC)
                );


                MessageBox.Show("Appointment added!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding appointment:\n" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
