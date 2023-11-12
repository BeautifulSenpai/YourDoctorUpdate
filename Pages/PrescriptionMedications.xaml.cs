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
    public partial class PrescriptionMedications : Page
    {
        private List<string> userRoles;
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["yourDoctor"].ConnectionString);
        SqlDataAdapter adapter;
        DataTable dt;

        public PrescriptionMedications(List<string> roles)
        {
            InitializeComponent();

            userRoles = roles;

            if (userRoles.Contains("Администратор"))
            {
                btnExportToExcel.Visibility = Visibility.Visible;
            }
            if (!userRoles.Contains("Администратор"))
            {
                btnDelete.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
            }
            if (!userRoles.Contains("Фармацевт"))
            {
                tbSearch.Visibility = Visibility.Collapsed;
                
            }

            con.Open();

            string query = "SELECT PrescriptionID, CustomerID, MedicationName, PrescriptionDate FROM PrescriptionMedications";
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

        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (userRoles.Contains("Администратор"))
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Лекарства по рецепту");

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
                            FileName = "Лекарства по рецепту.xlsx",
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

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tbSearch.Text.ToLower();

            if (gridMe.ItemsSource is DataView dataView)
            {
                DataView filteredView = new DataView(dataView.Table);
                filteredView.RowFilter = $"MedicationName LIKE '%{searchText}%'";

                gridMe.ItemsSource = filteredView;
            }
        }
    }
}
