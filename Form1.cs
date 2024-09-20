using System;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private DatabaseHelper dbHelper;

        public Form1()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
        }

        private void btnRegistration_Click(object sender, EventArgs e)
        {
            pnlRegistration.Visible = true;
            pnlLogin.Visible = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            pnlRegistration.Visible = false;
            pnlLogin.Visible = true;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void label5_Click(object sender, EventArgs e)
        {
            
        }

        private void btnLoginSubmit_Click(object sender, EventArgs e)
        {
            string username = txtLoginUsername.Text;
            string password = txtLoginPassword.Text;

            if (username == "Админ" && password == "админ")
            {
                AdminTabForm adminTabForm = new AdminTabForm();
                adminTabForm.Show();
                this.Hide();
            }
            else if (dbHelper.UserExists(username, password, out int readerID))
            {
                NewTabForm newTabForm = new NewTabForm(readerID);
                newTabForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.");
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtRegUsername.Text;
            string password = txtRegPassword.Text;
            string email = txtRegEmail.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            if (dbHelper.RegisterUser(username, password, email))
            {
                MessageBox.Show("Регистрация прошла успешно.");
                pnlRegistration.Visible = false;
                pnlLogin.Visible = true;
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации.");
            }
        }
    }
}