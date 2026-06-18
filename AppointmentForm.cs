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
    public partial class AppointmentForm : Form
    {
        public AppointmentForm()
        {
            InitializeComponent();
        }

        private void AppointmentForm_Load(object sender, EventArgs e)
        {
            LoadAppointments();

        }
        private void LoadAppointments()
        {
            string sql = @"
        SELECT appointment.appointmentId,
               customer.customerName,
               user.userName,
               appointment.type,
               appointment.start,
               appointment.end
        FROM appointment
        JOIN customer ON appointment.customerId = customer.customerId
        JOIN user ON appointment.userId = user.userId";

            DataTable dt = Database.Query(sql);

            foreach (DataRow row in dt.Rows)
            {
                DateTime startUtc = Convert.ToDateTime(row["start"]);
                DateTime endUtc = Convert.ToDateTime(row["end"]);

                row["start"] = startUtc.ToLocalTime();
                row["end"] = endUtc.ToLocalTime();
            }

            dgvAppointments.DataSource = dt;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddAppointmentForm add = new AddAppointmentForm();
            add.FormClosed += (s, args) => LoadAppointments();
            add.Show();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null)
            {
                MessageBox.Show("Please select an appointment to update.");
                return;
            }

            int id = Convert.ToInt32(dgvAppointments.CurrentRow.Cells["appointmentId"].Value);

            UpdateAppointmentForm update = new UpdateAppointmentForm(id);
            update.FormClosed += (s, args) => LoadAppointments();
            update.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null)
            {
                MessageBox.Show("Please select an appointment to delete.");
                return;
            }

            int id = Convert.ToInt32(dgvAppointments.CurrentRow.Cells["appointmentId"].Value);

            if (MessageBox.Show("Are you sure you want to delete this appointment?",
                                "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    Database.Execute("DELETE FROM appointment WHERE appointmentId = @id",
                        new MySqlParameter("@id", id));

                    LoadAppointments();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting appointment:\n" + ex.Message);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
