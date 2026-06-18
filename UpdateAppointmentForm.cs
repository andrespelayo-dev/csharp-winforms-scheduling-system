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
    public partial class UpdateAppointmentForm : Form
    {
        private int _appointmentId;
        private int _customerId;
        private int _userId;
        public UpdateAppointmentForm()
        {
            InitializeComponent();
        }
        public UpdateAppointmentForm(int appointmentId) : this()
        {
            _appointmentId = appointmentId;
            LoadCustomers();
            LoadAppointmentData();
        }
        private void LoadCustomers()
        {
            string sql = "SELECT customerId, customerName FROM customer";
            DataTable dt = Database.Query(sql);

            cmbCustomer.DataSource = dt;
            cmbCustomer.DisplayMember = "customerName";
            cmbCustomer.ValueMember = "customerId";
        }
        private void LoadAppointmentData()
        {
            string sql = @"
        SELECT appointment.*, customer.customerName
        FROM appointment
        JOIN customer ON appointment.customerId = customer.customerId
        WHERE appointmentId = @id";

            DataTable dt = Database.Query(sql,
                new MySql.Data.MySqlClient.MySqlParameter("@id", _appointmentId));

            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];

                txtType.Text = row["type"].ToString();

                cmbCustomer.SelectedValue = Convert.ToInt32(row["customerId"]);

                DateTime startUTC = Convert.ToDateTime(row["start"]);
                DateTime endUTC = Convert.ToDateTime(row["end"]);

                dtpStart.Value = startUTC.ToLocalTime();
                dtpEnd.Value = endUTC.ToLocalTime();

                _customerId = Convert.ToInt32(row["customerId"]);
                _userId = Convert.ToInt32(row["userId"]);
            }
            else
            {
                MessageBox.Show("Unable to load appointment data.");
                this.Close();
            }
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
    WHERE appointmentId != @id
    AND customerId = @cid
    AND (
          (@start BETWEEN start AND end)
       OR (@end BETWEEN start AND end)
       OR (start BETWEEN @start AND @end)
        )";

            DataTable overlap = Database.Query(sqlOverlap,
                new MySqlParameter("@id", _appointmentId),
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
UPDATE appointment SET 
    customerId = @cid,
    userId = @uid,
    title = 'Not Needed',
    description = 'Not Needed',
    location = 'Online',
    contact = 'Unassigned',
    type = @type,
    url = 'N/A',
    start = @startUTC,
    end = @endUTC,
    lastUpdate = NOW(),
    lastUpdateBy = 'test'
WHERE appointmentId = @id";

                Database.Execute(sql,
                    new MySqlParameter("@cid", customerId),
                    new MySqlParameter("@uid", 1),  // test user
                    new MySqlParameter("@type", type),
                    new MySqlParameter("@startUTC", startUTC),
                    new MySqlParameter("@endUTC", endUTC),
                    new MySqlParameter("@id", _appointmentId)
                );

                MessageBox.Show("Appointment updated successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating appointment:\n" + ex.Message);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
