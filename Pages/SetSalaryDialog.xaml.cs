using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public partial class SetSalaryDialog : Window
    {
        public SetSalaryDialog()
        {
            InitializeComponent();
            FillUserComboBox();
        }

        private void FillUserComboBox()
        {
            cboUsers.Items.Clear();

            string connectionString = ConfigurationManager.ConnectionStrings["yourDoctor"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT username FROM users";

                using (MySqlCommand cmd = new MySqlCommand(selectQuery, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string username = reader["username"].ToString();
                            cboUsers.Items.Add(username);
                        }
                    }
                }
            }
        }

        private void btnSetSalary_Click(object sender, RoutedEventArgs e)
        {
            string selectedUsername = cboUsers.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedUsername))
            {
                MessageBox.Show("Пожалуйста, выберите пользователя из списка.");
                return;
            }

            decimal salary;
            if (!decimal.TryParse(txtSalary.Text, out salary))
            {
                MessageBox.Show("Некорректная зарплата.");
                return;
            }

            decimal bonus;
            if (!decimal.TryParse(txtBonus.Text, out bonus))
            {
                MessageBox.Show("Некорректная премия.");
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["yourDoctor"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE users SET ";

                if (!string.IsNullOrEmpty(txtSalary.Text))
                {
                    updateQuery += "salary = @salary";
                }

                if (!string.IsNullOrEmpty(txtBonus.Text))
                {
                    if (updateQuery != "UPDATE users SET ")
                    {
                        updateQuery += ", ";
                    }
                    updateQuery += "bonus = @bonus";
                }

                if (!string.IsNullOrEmpty(selectedUsername))
                {
                    updateQuery += " WHERE username = @username";
                }

                using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                {
                    if (!string.IsNullOrEmpty(txtSalary.Text))
                    {
                        cmd.Parameters.AddWithValue("@salary", salary);
                    }

                    if (!string.IsNullOrEmpty(txtBonus.Text))
                    {
                        cmd.Parameters.AddWithValue("@bonus", bonus);
                    }

                    if (!string.IsNullOrEmpty(selectedUsername))
                    {
                        cmd.Parameters.AddWithValue("@username", selectedUsername);
                    }

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Зарплата и премия успешно установлены для пользователя {selectedUsername}.");
                        }
                        else
                        {
                            MessageBox.Show($"Не удалось обновить данные пользователя {selectedUsername}.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
                    }
                }
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
