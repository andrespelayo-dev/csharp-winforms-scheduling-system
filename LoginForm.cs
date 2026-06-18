using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using ScheduleApp;

namespace C969schedule
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            LocalizeText();
        }
        private void LocalizeText()
        {
            string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if (lang == "es")  
            {
                this.Text = "Iniciar Sesión";
                lblUsername.Text = "Usuario:";
                lblPassword.Text = "Contraseña:";
                btnLogin.Text = "Acceder";
            }
            else 
            {
                this.Text = "Login";
                lblUsername.Text = "Username:";
                lblPassword.Text = "Password:";
                btnLogin.Text = "Login";
            }
        }


        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                ShowError("Please enter username and password.",
                          "Por favor ingrese usuario y contraseña.");
                LogLoginAttempt(user, false);
                return;
            }

            string sql = "SELECT * FROM user WHERE userName = @user AND password = @pass";

            var result = Database.Query(sql,
                new MySql.Data.MySqlClient.MySqlParameter("@user", user),
                new MySql.Data.MySqlClient.MySqlParameter("@pass", pass)
            );

            if (result.Rows.Count == 1)
            {
                LogLoginAttempt(user, true);
                CheckUpcomingAppointments(result.Rows[0]["userId"]);
                OpenMainForm();
            }
            else
            {
                ShowError("The username and password do not match.",
                          "El usuario y la contraseña no coinciden.");
                LogLoginAttempt(user, false);
            }
        }
        private void ShowError(string english, string spanish)
        {
            string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if (lang == "es")
                lblMessage.Text = spanish;
            else
                lblMessage.Text = english;
        }
        private void LogLoginAttempt(string username, bool success)
        {
            string file = "Login_History.txt";

            string status = success ? "SUCCESS" : "FAIL";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string line = $"{timestamp} | {username} | {status}";

            System.IO.File.AppendAllText(file, line + Environment.NewLine);
        }
        private void CheckUpcomingAppointments(object userIdObject)
        {
            int userId = Convert.ToInt32(userIdObject);

            string sql = @"SELECT * FROM appointment
                   WHERE userId = @uid
                   AND start BETWEEN UTC_TIMESTAMP() AND DATE_ADD(UTC_TIMESTAMP(), INTERVAL 15 MINUTE)";

            var result = Database.Query(sql,
                new MySql.Data.MySqlClient.MySqlParameter("@uid", userId)
            );

            if (result.Rows.Count > 0)
            {
                string msg = "You have an appointment within the next 15 minutes.";
                string msgES = "Tiene una cita en los próximos 15 minutos.";

                string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

                if (lang == "es")
                    MessageBox.Show(msgES, "Aviso");
                else
                    MessageBox.Show(msg, "Alert");
            }
        }
        private void OpenMainForm()
        {
            this.Hide();
            MainForm main = new MainForm();
            main.Show();
        }

    }
}
