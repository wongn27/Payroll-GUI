using Microsoft.Win32;
using Payroll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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

namespace PayrollGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Department department = new Department();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonOpenDepartmentJSONFile_Click(object sender, RoutedEventArgs e)
        {
            textBoxDepartmentFilename.IsReadOnly = true;
            textBoxDepartmentName.IsReadOnly = true;
            textBoxTotalWorkerHours.IsReadOnly = true;
            textBoxTotalWorkerPay.IsReadOnly = true;

            // Create OpenFileDialog
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open Department From JSON";
            openFileDialog.InitialDirectory = @"C:\Users\wongn\source\repos\Payroll\PayrollTesting\bin\Debug";
            openFileDialog.Filter = "JSON files (*.json)|*.json";
            var result = openFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                textBoxDepartmentFilename.Text = openFileDialog.FileName;
                textBoxDepartmentName.Text = System.IO.File.ReadAllText(openFileDialog.FileName);
            }

            try
            {
                string fileName = textBoxDepartmentFilename.Text;
                department = DeserializeDepartmentJSON(fileName);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
            }

            string departmentName = department.Name;
            textBoxDepartmentName.Text = departmentName;

            double totalWorkerHours = department.CalculateTotalHoursWorked();
            textBoxTotalWorkerHours.Text = Convert.ToString(totalWorkerHours);

        }

        /// <summary>
        /// Reads Department data from JSON file <see cref="Department"/>
        /// </summary>
        /// <returns></returns>
        public static Department DeserializeDepartmentJSON(string fileName)
        {
            Department department;

            var reader = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var inputSerializer = new DataContractJsonSerializer(typeof(Department));

            department = (Department)inputSerializer.ReadObject(reader);
            reader.Close();

            return department;
        }
    }
}
