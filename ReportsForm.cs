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
    public partial class ReportsForm : Form
    {
        public ReportsForm()
        {
            InitializeComponent();
        }

        private void btnReport1_Click(object sender, EventArgs e)
        {
            ReportAppointmentTypesByMonth();

        }
        private void ReportAppointmentTypesByMonth()
        {
            string sql = @"
        SELECT MONTH(start) AS Month,
               type AS AppointmentType
        FROM appointment";

            DataTable dt = Database.Query(sql);

            var result = dt.AsEnumerable()
                           .GroupBy(row => new
                           {
                               Month = Convert.ToInt32(row["Month"]),
                               Type = row["AppointmentType"].ToString()
                           })
                           .Select(g => new
                           {
                               g.Key.Month,
                               g.Key.Type,
                               Count = g.Count()
                           })
                           .ToList();

            dgvReports.DataSource = result;
        }

        private void btnReport2_Click(object sender, EventArgs e)
        {
            ReportConsultantSchedule();

        }
        private void ReportConsultantSchedule()
        {
            string sql = @"
        SELECT user.userName,
               appointment.type,
               appointment.start,
               appointment.end,
               customer.customerName
        FROM appointment
        JOIN user ON appointment.userId = user.userId
        JOIN customer ON appointment.customerId = customer.customerId";

            DataTable dt = Database.Query(sql);

            // Convert UTC → Local
            foreach (DataRow row in dt.Rows)
            {
                row["start"] = Convert.ToDateTime(row["start"]).ToLocalTime();
                row["end"] = Convert.ToDateTime(row["end"]).ToLocalTime();
            }

            dgvReports.DataSource = dt;
        }

        private void btnReport3_Click(object sender, EventArgs e)
        {
            ReportAppointmentsPerCustomer();

        }
        private void ReportAppointmentsPerCustomer()
        {
            string sql = @"
        SELECT customer.customerName,
               appointment.appointmentId
        FROM appointment
        JOIN customer ON appointment.customerId = customer.customerId";

            DataTable dt = Database.Query(sql);

            // ⭐ Another lambda expression
            var result = dt.AsEnumerable()
                           .GroupBy(row => row.Field<string>("customerName"))
                           .Select(g => new
                           {
                               Customer = g.Key,
                               Count = g.Count()
                           })
                           .ToList();

            dgvReports.DataSource = result;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
