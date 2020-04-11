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
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
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

                string departmentName = department.Name;
                textBoxDepartmentName.Text = departmentName;

                double totalWorkerHours = department.CalculateTotalHoursWorked();
                textBoxTotalWorkerHours.Text = totalWorkerHours.ToString();

                double totalWorkerPay = department.CalculateTotalPay();
                textBoxTotalWorkerPay.Text = totalWorkerPay.ToString();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
            }

            foreach (Worker worker in department.Workers)
            {
                listViewWorkers.Items.Add(worker);
            }

            foreach (Shift shift in department.Shifts)
            {
                listViewShifts.Items.Add(shift);
            }
        }

        private void buttonFindWorker_Click(object sender, RoutedEventArgs e)
        {
            int targetWorkerId = Int32.Parse(textBoxTargetWorkerId.Text);

            Worker worker = department.FindWorker(targetWorkerId);

            if (null == worker)
            {
                MessageBox.Show($"Could not find worker with Id {targetWorkerId}");
            }

            textBoxWorkerName.Text = worker.Name;
            textBoxWorkerId.Text = worker.Id.ToString();
            textBoxWorkerPayRate.Text = worker.PayRate.ToString();
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
