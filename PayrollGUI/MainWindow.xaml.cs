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
            // Create OpenFileDialog
            var openFileDialog = new OpenFileDialog
            {
                Title = "Open Department From JSON",
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "JSON files (*.json)|*.json"
            };

            var result = openFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                textBoxDepartmentFilename.Text = openFileDialog.FileName;
                textBoxDepartmentName.Text = System.IO.File.ReadAllText(openFileDialog.FileName);
            }

            try
            {
                textBoxDepartmentFilename.IsReadOnly = true;

                string fileName = textBoxDepartmentFilename.Text;
                department = DeserializeDepartmentJSON(fileName);

                PopulateDepartmentAndTotalTextBoxes();

                textBoxTargetWorkerId.Clear();

                ClearWorkerTextBoxes(windowGrid, "textBoxWorker");
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
                ClearWorkerTextBoxes(windowGrid, "textBoxWorker");
            }
            else
            {
                PopulateWorkerTextBoxes(worker, targetWorkerId);
            }
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

        public void ClearWorkerTextBoxes(Grid grid, string startsWith)
        {
            foreach (System.Windows.Controls.Control control in windowGrid.Children)
            {
                if (control.Name.StartsWith(startsWith))
                {
                    var textbox = (TextBox)control;
                    textbox.Clear();
                }
            }
        }

        public void PopulateTextBox(string textBoxString, TextBox textBox)
        {
            textBox.Text = textBoxString;
            textBox.IsReadOnly = true;
        }

        public void PopulateDepartmentAndTotalTextBoxes()
        {
            PopulateTextBox(department.Name, textBoxDepartmentName);
            PopulateTextBox(department.CalculateTotalHoursWorked().ToString(), textBoxTotalWorkerHours);
            PopulateTextBox(department.CalculateTotalPay().ToString(), textBoxTotalWorkerPay);
        }

        public void PopulateWorkerTextBoxes(Worker worker, int targetWorkerId)
        {
            PopulateTextBox(worker.Name, textBoxWorkerName);
            PopulateTextBox(worker.Id.ToString(), textBoxWorkerId);
            PopulateTextBox(worker.PayRate.ToString(), textBoxWorkerPayRate);
            PopulateTextBox(department.CalculateTotalWorkerHours(targetWorkerId).ToString(), textBoxWorkerHours);
            PopulateTextBox(department.CalculatePay(targetWorkerId).ToString(), textBoxWorkerPay);
        }
    }
}
