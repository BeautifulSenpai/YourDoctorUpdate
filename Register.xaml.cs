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
using MySql.Data.MySqlClient;

namespace YourDoctor
{
    /// <summary>
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
            

        private void reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создайте подключение к MySQL
                MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["yourDoctor"].ConnectionString);
                con.Open();

                // Ваш SQL-запрос для вставки данных в MySQL
                string add_data = "INSERT INTO Users (Username, Password) VALUES(@Username, @Password)";

                // Создайте MySqlCommand для выполнения SQL-запроса
                MySqlCommand cmd = new MySqlCommand(add_data, con);

                // Добавьте параметры для вашего SQL-запроса
                cmd.Parameters.AddWithValue("@Username", username.Text);
                cmd.Parameters.AddWithValue("@Password", password.Password);

                // Выполните SQL-запрос
                cmd.ExecuteNonQuery();

                // Закройте подключение после выполнения операции
                con.Close();
                username.Text = "";
                password.Password = "";
                Login w1 = new Login();
                this.Close();
                w1.Show();

            }
            catch
            {

            }
        }
    }
}
