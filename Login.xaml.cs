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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YourDoctor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void regist_Click(object sender, RoutedEventArgs e)
        {
            Register r1 = new Register();
            this.Close();
            r1.Show();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["yourDoctor"].ConnectionString);
                con.Open();

                // SQL-запрос для выборки данных
                string select_data = "SELECT * FROM Users WHERE Username=@Username AND Password=@Password";

                // Создайте MySqlCommand для выполнения SQL-запроса
                MySqlCommand cmd = new MySqlCommand(select_data, con);

                cmd.Parameters.AddWithValue("@Username", username.Text);
                cmd.Parameters.AddWithValue("@Password", password.Password);
                cmd.ExecuteNonQuery();
                int Count = Convert.ToInt32(cmd.ExecuteScalar());
                List<string> userRoles = GetUserRoles(username.Text);
                con.Close();

                username.Text = "";
                password.Password = "";
                if(Count > 0)
                {
                    Home w1 = new Home(userRoles);
                    this.Close();
                    w1.Show();
                }
                else
                {
                    MessageBox.Show("Пароль или Имя пользователя не является правильным");
                }



            }
            catch
            {

            }
        }

        private List<string> GetUserRoles(string username)
        {
            List<string> roles = new List<string>();

            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["yourDoctor"].ConnectionString))
            {
                con.Open();
                string query = "SELECT RoleName FROM Roles INNER JOIN UserRoles ON Roles.RoleID = UserRoles.RoleID " +
                               "INNER JOIN users ON users.UserID = UserRoles.UserID WHERE users.Username = @Username";

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(reader["RoleName"].ToString());
                    }
                }
            }

            return roles;
        }
    }
}
