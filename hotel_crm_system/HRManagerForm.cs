using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hotel_crm_system
{
    public partial class HRManagerForm : Form
    {
        public HRManagerForm()
        {
            InitializeComponent();
            InitializeDataGridView();
            AddDismissButtonColumn(dataGridView1);
        }

        private void LoadEmployeeData()
        {
            string connectionString = "Data Source=C:\\Users\\Enjoy\\source\\repos\\hotel_crm_system\\hotel_crm_system\\hotel_database.db;Version=3;";
            string query = "SELECT Name, Position, WorkPlan FROM Employees";

            dataGridView1.Rows.Clear();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader["Name"], reader["Position"], reader["WorkPlan"]);
                        }
                    }
                }
            }
        }

        private void UpdateEmployeeData()
        {
            string connectionString = "Data Source=C:\\Users\\Enjoy\\source\\repos\\hotel_crm_system\\hotel_crm_system\\hotel_database.db;Version=3;";
            string query = "SELECT Name, Position, WorkPlan FROM Employees";

            dataGridView1.Rows.Clear();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView1.Rows.Add(reader["Name"], reader["Position"], reader["WorkPlan"]);
                        }
                    }
                }
            }
        }

        private void AddDismissButtonColumn(DataGridView dataGridView)
        {
            DataGridViewButtonColumn dismissButtonColumn = new DataGridViewButtonColumn();
            dismissButtonColumn.Name = "Dismiss";
            dismissButtonColumn.Text = "Dismiss";
            dismissButtonColumn.UseColumnTextForButtonValue = true;
            dismissButtonColumn.Width = 70;

            dataGridView.CellContentClick += (sender, e) =>
            {
                if (e.ColumnIndex == dataGridView.Columns["Dismiss"].Index && e.RowIndex >= 0)
                {
                    string position = dataGridView.Rows[e.RowIndex].Cells["emp_position"].Value.ToString();
                    if (position != "HR manager" && position != "Director")
                    {
                        string employeeName = dataGridView.Rows[e.RowIndex].Cells["emp_name"].Value.ToString();
                        DialogResult result = MessageBox.Show($"Are you sure you want to dismiss {employeeName}?", "Confirm Dismissal", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            SQLiteConnection connection = db_processing.GetConnection();
                            db_processing.DismissEmployee(connection, employeeName);
                            UpdateEmployeeData();
                            MessageBox.Show("Dismissed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cannot dismiss this employee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            dataGridView.Columns.Add(dismissButtonColumn);
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["emp_workplan"].Index)
            {
                string employeeName = dataGridView1.Rows[e.RowIndex].Cells["emp_name"].Value.ToString();
                string newWorkPlan = dataGridView1.Rows[e.RowIndex].Cells["emp_workplan"].Value.ToString();

                SQLiteConnection connection = db_processing.GetConnection();
                db_processing.UpdateWorkPlan(connection, employeeName, newWorkPlan);
            }
        }



        private void InitializeDataGridView()
        {
            dataGridView1.CellValueChanged += dataGridView_CellValueChanged;
            LoadEmployeeData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        }
    }