using ClosedXML.Excel;

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YourDoctor.Pages
{
    public partial class Products : Page
    {
        private List<string> userRoles;
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["yourDoctor"].ConnectionString);
        SqlDataAdapter adapter;
        DataTable dt;

        public Products(List<string> roles)
        {
            InitializeComponent();
            userRoles = roles;

            if (userRoles.Contains("Администратор"))
            {
                btnExportToExcel.Visibility = Visibility.Visible;
            }
            if(!userRoles.Contains("Администратор"))
            {
                btnDelete.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
            }
            if (!userRoles.Contains("Фармацевт"))
            {
                tbSearch.Visibility = Visibility.Collapsed;
                btnSell.Visibility = Visibility.Collapsed;
            }
            if (!userRoles.Contains("Директор"))
            {
                btnViewSalesSummary.Visibility= Visibility.Collapsed;
            }



            con.Open();

            string query = "SELECT ProductID, Name, Description, Price, QuantityInStock, QuantitySold FROM Products";
            MySqlCommand cmd = new MySqlCommand(query, con);

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            gridMe.ItemsSource = dt.DefaultView;

            con.Close();


        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            adapter.Update(dt);
            MessageBox.Show("Данные успешно обновлены!");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEditableCollectionView iecv = CollectionViewSource.GetDefaultView(gridMe.ItemsSource) as IEditableCollectionView;

                while (gridMe.SelectedIndex >= 0)
                {
                    int selectedIndex = gridMe.SelectedIndex;
                    DataGridRow dgr = gridMe.ItemContainerGenerator.ContainerFromIndex(selectedIndex) as DataGridRow;
                    dgr.IsSelected = false;

                    if (iecv.IsEditingItem)
                    {
                        iecv.CommitEdit();
                        iecv.RemoveAt(selectedIndex);
                        adapter.Update(dt);
                    }
                    else
                    {
                        iecv.RemoveAt(selectedIndex);
                        adapter.Update(dt);
                    }
                }
                MessageBox.Show("Данные успешно удалены!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка удаления!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tbSearch.Text.ToLower();

            if (gridMe.ItemsSource is DataView dataView)
            {
                DataView filteredView = new DataView(dataView.Table);
                filteredView.RowFilter = $"Name LIKE '%{searchText}%' OR Description LIKE '%{searchText}%'";

                gridMe.ItemsSource = filteredView;
            }
        }

        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (userRoles.Contains("Администратор"))
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Товары");

                        for (int i = 0; i < gridMe.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = gridMe.Columns[i].Header.ToString();
                        }


                        for (int row = 0; row < gridMe.Items.Count; row++)
                        {
                            for (int col = 0; col < gridMe.Columns.Count; col++)
                            {
                                var cellValue = ((TextBlock)gridMe.Columns[col].GetCellContent(gridMe.Items[row])).Text;
                                worksheet.Cell(row + 2, col + 1).Value = cellValue;
                            }
                        }

                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                        {
                            FileName = "Товары.xlsx",
                            DefaultExt = ".xlsx",
                            Filter = "Excel Files|*.xlsx"
                        };

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            workbook.SaveAs(saveFileDialog.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при экспорте данных в Excel: " + ex.Message);
                }
            }
        }

        private void btnSell_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedItem = (DataRowView)gridMe.SelectedItem;

            if (selectedItem != null)
            {
                int productId = Convert.ToInt32(selectedItem["ProductID"]);
                string productName = selectedItem["Name"].ToString();
                decimal productPrice = Convert.ToDecimal(selectedItem["Price"]);

                int quantityToSell = 1;

                if (quantityToSell <= Convert.ToInt32(selectedItem["QuantityInStock"]))
                {
                    try
                    {
                        con.Open();

                        string updateQuery = $"UPDATE Products SET QuantityInStock = QuantityInStock - {quantityToSell}, QuantitySold = QuantitySold + {quantityToSell} WHERE ProductID = {productId}";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, con);
                        updateCmd.ExecuteNonQuery();

                        selectedItem["QuantityInStock"] = Convert.ToInt32(selectedItem["QuantityInStock"]) - quantityToSell;
                        selectedItem["QuantitySold"] = Convert.ToInt32(selectedItem["QuantitySold"]) + quantityToSell;

                        MessageBox.Show($"Продажа {quantityToSell} {productName} за {productPrice * quantityToSell:N2} ₽ завершена успешно.", "Продажа завершена", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating database: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Недостаточно товара для продажи.", "Недостаточный запас", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("\r\nПожалуйста, выберите продукт для продажи.", "Ошибка выбора", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowSalesSummary()
        {
            try
            {
                con.Open();

                string query = "SELECT SUM(QuantitySold) AS TotalSold, SUM(QuantitySold * Price) AS TotalRevenue FROM Products";
                MySqlCommand cmd = new MySqlCommand(query, con);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int totalSold = reader.GetInt32("TotalSold");
                        decimal totalRevenue = reader.GetDecimal("TotalRevenue");

                        MessageBox.Show($"Общая сводка по продажам:\n\nКоличество проданных товаров: {totalSold}\nОбщая выручка: {totalRevenue:N2} ₽", "Сводка по продажам", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Нет данных о продажах.", "Сводка по продажам", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении сводки по продажам: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnViewSalesSummary_Click(object sender, RoutedEventArgs e)
        {
            if (userRoles.Contains("Директор"))
            {
                ShowSalesSummary();
            }
            else
            {
                MessageBox.Show("У вас нет прав для просмотра сводки по продажам.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
