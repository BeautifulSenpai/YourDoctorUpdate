using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YourDoctor.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEmployeeDialog.xaml
    /// </summary>
    public partial class AddEmployeeDialog : Window
    {
        public AddEmployeeDialog()
        {
            InitializeComponent();
            UsernameTextBox = new TextBox();
            UsernameTextBox.Name = "txtUsername";

            PasswordBox = new PasswordBox();
            PasswordBox.Name = "txtPassword";
        }

        public TextBox UsernameTextBox { get; private set; }
        public PasswordBox PasswordBox { get; private set; }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Введите имя сотрудника");
                return;
            }

            if (txtPassword.Password.Length == 0)
            {
                MessageBox.Show("Введите пароль");
                return;
            }

            if (cmbRoles.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль сотрудника");
                return;
            }

            string selectedRole = ((ComboBoxItem)cmbRoles.SelectedItem).Content.ToString();

            string connectionString = ConfigurationManager.ConnectionStrings["yourDoctor"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string checkUsernameQuery = "SELECT COUNT(*) FROM users WHERE Username = @Username;";
                using (MySqlCommand checkUsernameCommand = new MySqlCommand(checkUsernameQuery, connection))
                {
                    checkUsernameCommand.Parameters.AddWithValue("@Username", txtUsername.Text);
                    int count = Convert.ToInt32(checkUsernameCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует. Выберите другое имя.");
                        return;
                    }
                }

                string insertUserQuery = "INSERT INTO users (Username, Password) VALUES (@Username, @Password);";
                using (MySqlCommand insertUserCommand = new MySqlCommand(insertUserQuery, connection))
                {
                    insertUserCommand.Parameters.AddWithValue("@Username", txtUsername.Text);
                    insertUserCommand.Parameters.AddWithValue("@Password", txtPassword.Password);
                    insertUserCommand.ExecuteNonQuery();
                }

                string getUserIdQuery = "SELECT LAST_INSERT_ID();";
                int userId;
                using (MySqlCommand getUserIdCommand = new MySqlCommand(getUserIdQuery, connection))
                {
                    userId = Convert.ToInt32(getUserIdCommand.ExecuteScalar());
                }

                string getRoleIdQuery = "SELECT RoleID FROM roles WHERE RoleName = @RoleName;";
                int roleId;
                using (MySqlCommand getRoleIdCommand = new MySqlCommand(getRoleIdQuery, connection))
                {
                    getRoleIdCommand.Parameters.AddWithValue("@RoleName", selectedRole);
                    roleId = Convert.ToInt32(getRoleIdCommand.ExecuteScalar());
                }

                string insertUserRoleQuery = "INSERT INTO userroles (UserID, RoleID) VALUES (@UserID, @RoleID);";
                using (MySqlCommand insertUserRoleCommand = new MySqlCommand(insertUserRoleQuery, connection))
                {
                    insertUserRoleCommand.Parameters.AddWithValue("@UserID", userId);
                    insertUserRoleCommand.Parameters.AddWithValue("@RoleID", roleId);
                    insertUserRoleCommand.ExecuteNonQuery();
                }

                MessageBox.Show("Сотрудник успешно добавлен!");

                DialogResult = true;
                Close();
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
             if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
