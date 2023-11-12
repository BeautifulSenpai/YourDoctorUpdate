using YourDoctor.Pages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Office.Interop.Excel;

namespace YourDoctor
{
    public partial class Home : System.Windows.Window
    {
        private List<string> userRoles;
        public Home(List<string> roles)
        {
            InitializeComponent();
            userRoles = roles;
            btnAddEmployee.Visibility = Visibility.Collapsed;

            if (userRoles.Contains("Директор"))
            {
                btnAddEmployee.Visibility = Visibility.Visible;
                btnSetSalary.Visibility= Visibility.Visible;
            }

            if (userRoles.Contains("Фармацевт"))
            {
                rdCustomers.Visibility = Visibility.Collapsed;
                rdOrders.Visibility = Visibility.Collapsed;
                rdOrderItems.Visibility = Visibility.Collapsed;
                rdSuppliers.Visibility = Visibility.Collapsed;
                rdSupply.Visibility = Visibility.Collapsed;
                rdReviews.Visibility = Visibility.Collapsed;
                btnSetSalary.Visibility = Visibility.Collapsed;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void rdProducts_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new Pages.Products(userRoles));
        }

        private void rdCustomers_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new Customers(userRoles));
        }

        private void rdOrders_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new Orders(userRoles));
        }

        private void rdOrderItems_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new OrderItems(userRoles));
        }

        private void rdSuppliers_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new Suppliers(userRoles));
        }

        private void rdSupply_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new Supply(userRoles));
        }

        private void rdPrescriptionMedications_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new PrescriptionMedications(userRoles));
        }

        private void rdReviews_Click(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new Reviews(userRoles));
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnLogout_Click(object sender, MouseButtonEventArgs e)
        {
            Login w1 = new Login();
            this.Close();
            w1.Show();
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeDialog dialog = new AddEmployeeDialog();

            if (dialog.ShowDialog() == true)
            {
                if (dialog.txtUsername != null && dialog.txtPassword != null)
                {
                    string username = dialog.txtUsername.Text;
                    string password = dialog.txtPassword.Password;

                    string connectionString = ConfigurationManager.ConnectionStrings["yourDoctor"].ConnectionString;
                    MySqlConnection conn = new MySqlConnection(connectionString);

                    string query = "INSERT INTO users (Username, Password) VALUES (@username, @password)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    conn.Open();
                    conn.Close();
                }
                else{}
            }
        }

        private void btnSetSalary_Click(object sender, RoutedEventArgs e)
        {
            SetSalaryDialog setSalaryDialog = new SetSalaryDialog();

            if (setSalaryDialog.ShowDialog() == true)
            {
                setSalaryDialog.Show();
            }
        }
    }
}
