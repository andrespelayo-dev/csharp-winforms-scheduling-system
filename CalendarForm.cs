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
    public partial class CalendarForm : Form
    {
        public CalendarForm()
        {
            InitializeComponent();
        }

        private void CalendarForm_Load(object sender, EventArgs e)
        {
            LoadAllAppointments();

        }
        private void LoadAllAppointments()
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

            // Convert UTC → Local for display
            foreach (DataRow row in dt.Rows)
            {
                row["start"] = Convert.ToDateTime(row["start"]).ToLocalTime();
                row["end"] = Convert.ToDateTime(row["end"]).ToLocalTime();
            }

            dgvCalendar.DataSource = dt;
        }

        private void btnWeek_Click(object sender, EventArgs e)
        {
            LoadWeekAppointments();

        }
        private void LoadWeekAppointments()
        {
            DateTime now = DateTime.Now;
            DateTime weekStart = now.AddDays(-(int)now.DayOfWeek);  
            DateTime weekEnd = weekStart.AddDays(7);

            string sql = @"
        SELECT appointment.appointmentId,
               customer.customerName,
               user.userName,
               appointment.type,
               appointment.start,
               appointment.end
        FROM appointment
        JOIN customer ON appointment.customerId = customer.customerId
        JOIN user ON appointment.userId = user.userId
        WHERE appointment.start BETWEEN @start AND @end";

            DataTable dt = Database.Query(sql,
                new MySqlParameter("@start", weekStart.ToUniversalTime()),
                new MySqlParameter("@end", weekEnd.ToUniversalTime())
            );

            foreach (DataRow row in dt.Rows)
            {
                row["start"] = Convert.ToDateTime(row["start"]).ToLocalTime();
                row["end"] = Convert.ToDateTime(row["end"]).ToLocalTime();
            }

            dgvCalendar.DataSource = dt;
        }

        private void btnMonth_Click(object sender, EventArgs e)
        {
            LoadMonthAppointments();

        }
        private void LoadMonthAppointments()
        {
            DateTime now = DateTime.Now;
            DateTime monthStart = new DateTime(now.Year, now.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1);

            string sql = @"
        SELECT appointment.appointmentId,
               customer.customerName,
               user.userName,
               appointment.type,
               appointment.start,
               appointment.end
        FROM appointment
        JOIN customer ON appointment.customerId = customer.customerId
        JOIN user ON appointment.userId = user.userId
        WHERE appointment.start BETWEEN @start AND @end";

            DataTable dt = Database.Query(sql,
                new MySqlParameter("@start", monthStart.ToUniversalTime()),
                new MySqlParameter("@end", monthEnd.ToUniversalTime())
            );

            foreach (DataRow row in dt.Rows)
            {
                row["start"] = Convert.ToDateTime(row["start"]).ToLocalTime();
                row["end"] = Convert.ToDateTime(row["end"]).ToLocalTime();
            }

            dgvCalendar.DataSource = dt;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            LoadAllAppointments();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }


        private void monthCalendar1_DateSelected_1(object sender, DateRangeEventArgs e)
        {
            DateTime selectedDate = e.Start.Date;

            string sql = @"
        SELECT 
            customer.customerName,
            appointment.type,
            appointment.start,
            appointment.end
        FROM appointment
        JOIN customer ON appointment.customerId = customer.customerId
        WHERE DATE(appointment.start) = @selectedDate";

            DataTable dt = Database.Query(sql,
                new MySqlParameter("@selectedDate", selectedDate)
            );

            // Convert UTC → Local for display
            foreach (DataRow row in dt.Rows)
            {
                row["start"] = Convert.ToDateTime(row["start"]).ToLocalTime();
                row["end"] = Convert.ToDateTime(row["end"]).ToLocalTime();
            }

            dgvCalendar.DataSource = dt;
        }
    }
}
