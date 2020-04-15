//******************************************************
// File: MainWindow.xaml.cs
//
// Purpose: This application will use a Windows Presentation Foundation (WPF) GUI to display payroll data.
//
// Written By: Natalie Wong
//
// Compiler: Visual Studio 2019
//
//******************************************************

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

        /// <summary>
        /// Open Department JSON File button. When the user presses this button it
        /// should display an open file dialog and let the user select the file to open.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            // If the user chooses cancel, return to the window 
            if (result == false)
                return;

            // If the user chooses to open the file then it should populate the department controls with data from the selected
            // file, clear the worker TextBoxes (the controls used by Find Worker) and the filename should also appear in the 
            // department filename TextBox in the main window
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

                ClearWorkerTextBoxes(windowGrid, "textBoxWorker");

                textBoxTargetWorkerId.Clear();
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

        /// <summary>
        /// Find Worker. When the user presses this button it should get data for the
        /// target worker id and populate the TextBoxes to the right.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFindWorker_Click(object sender, RoutedEventArgs e)
        {
            int targetWorkerId = Int32.Parse(textBoxTargetWorkerId.Text);

            Worker worker = department.FindWorker(targetWorkerId);

            // If the worker id is not found the TextBoxes should just be cleared
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

        /// <summary>
        /// Clears the Worker TextBoxes populated by the Find Worker button.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="startsWith"></param>
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

        /// <summary>
        /// Populates a TextBox with data.
        /// </summary>
        /// <param name="textBoxString"></param>
        /// <param name="textBox"></param>
        public void PopulateTextBox(string textBoxString, TextBox textBox)
        {
            textBox.Text = textBoxString;
            textBox.IsReadOnly = true;
        }

        /// <summary>
        /// Populates the Department Name, Total Worker Hours and the Total Worker Pay TextBoxes with data.
        /// </summary>
        public void PopulateDepartmentAndTotalTextBoxes()
        {
            PopulateTextBox(department.Name, textBoxDepartmentName);
            PopulateTextBox(department.CalculateTotalHoursWorked().ToString(), textBoxTotalWorkerHours);
            PopulateTextBox(department.CalculateTotalPay().ToString(), textBoxTotalWorkerPay);
        }

        /// <summary>
        /// Populates the Worker Name, Worker Id, Worker Pay Rate, Worker Hours and the Worker Pay Textboxes 
        /// with the Target Worker Id's data.
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="targetWorkerId"></param>
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
