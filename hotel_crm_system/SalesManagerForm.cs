using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hotel_crm_system
{
    public partial class SalesManagerForm : Form
    {
        public SalesManagerForm()
        {
            InitializeComponent();
            LoadRoomData();

            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            dataGridView2.CellFormatting += dataGridView2_CellFormatting;
            dataGridView3.CellFormatting += dataGridView3_CellFormatting;

            comboBox2.Items.AddRange(new object[] { 1, 2, 3 });
            comboBox3.Items.AddRange(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            comboBox1.Items.AddRange(GetSalesManagers().ToArray());
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void LoadRoomData()
        {
            DataGridViewButtonColumn buttonColumn1 = new DataGridViewButtonColumn();
            buttonColumn1.Name = "Cleaning";
            buttonColumn1.Text = "Clean";
            buttonColumn1.UseColumnTextForButtonValue = true;
            buttonColumn1.Width = 50;
            dataGridView1.Columns.Add(buttonColumn1);

            DataGridViewButtonColumn buttonColumn2 = new DataGridViewButtonColumn();
            buttonColumn2.Name = "Cleaning";
            buttonColumn2.Text = "Clean";
            buttonColumn2.UseColumnTextForButtonValue = true;
            buttonColumn2.Width = 50;
            dataGridView2.Columns.Add(buttonColumn2);

            DataGridViewButtonColumn buttonColumn3 = new DataGridViewButtonColumn();
            buttonColumn3.Name = "Cleaning";
            buttonColumn3.Text = "Clean";
            buttonColumn3.UseColumnTextForButtonValue = true;
            buttonColumn3.Width = 50;
            dataGridView3.Columns.Add(buttonColumn3);

            string connectionString = "Data Source=C:\\Users\\Enjoy\\source\\repos\\hotel_crm_system\\hotel_crm_system\\hotel_database.db;Version=3;";
            string query1 = "SELECT RoomNumber, Status FROM Rooms LIMIT 10 OFFSET 0";
            string query2 = "SELECT RoomNumber, Status FROM Rooms LIMIT 10 OFFSET 10";
            string query3 = "SELECT RoomNumber, Status FROM Rooms LIMIT 10 OFFSET 20";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                FillDataGridView(dataGridView1, query1, connection);
                FillDataGridView(dataGridView2, query2, connection);
                FillDataGridView(dataGridView3, query3, connection);
            }

        }
        private void UpdateRoomData()
        {
            string connectionString = "Data Source=C:\\Users\\Enjoy\\source\\repos\\hotel_crm_system\\hotel_crm_system\\hotel_database.db;Version=3;";
            string query1 = "SELECT RoomNumber, Status FROM Rooms LIMIT 10 OFFSET 0";
            string query2 = "SELECT RoomNumber, Status FROM Rooms LIMIT 10 OFFSET 10";
            string query3 = "SELECT RoomNumber, Status FROM Rooms LIMIT 10 OFFSET 20";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                FillDataGridView(dataGridView1, query1, connection);
                FillDataGridView(dataGridView2, query2, connection);
                FillDataGridView(dataGridView3, query3, connection);
            }
        }

        public static List<string> GetSalesManagers()
        {
            List<string> salesManagers = new List<string>();
            string connectionString = "Data Source=C:\\Users\\Enjoy\\source\\repos\\hotel_crm_system\\hotel_crm_system\\hotel_database.db;Version=3;";
            string query = "SELECT Name FROM Employees WHERE Position = 'Sales manager';";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string salesManagerName = reader["name"].ToString();
                            salesManagers.Add(salesManagerName);
                        }
                    }
                }
            }

            return salesManagers;
        }

        private void FillDataGridView(DataGridView dataGridView, string query, SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    dataGridView.Rows.Clear();

                    while (reader.Read())
                    {
                        int rowIndex = dataGridView.Rows.Add(reader["RoomNumber"], reader["Status"]);
                        string statusColumnName = dataGridView == dataGridView1 ? "room_status" :
                                                  dataGridView == dataGridView2 ? "room_status_2fl" :
                                                  "room_status_3fl";

                        if (reader["Status"].ToString() != "Available")
                        {
                            dataGridView.Rows[rowIndex].Cells["Cleaning"].Value = "";
                            dataGridView.Rows[rowIndex].Cells["Cleaning"].ReadOnly = true;
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Cleaning"].Index && e.RowIndex >= 0)
            {
                string roomStatus = dataGridView1.Rows[e.RowIndex].Cells["room_status"].Value.ToString();
                if (roomStatus == "Available")
                {
                    int roomNumber = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["room_number"].Value);
                    Console.WriteLine($"Назначена уборка на комнату {roomNumber} на 1 этаже");
                    SQLiteConnection LocalConnection = db_processing.GetConnection();
                    db_processing.AssignCleaning(LocalConnection, roomNumber);
                    dataGridView1.Rows[e.RowIndex].Cells["room_status"].Value = "Cleaning";
                }
                else
                {
                    MessageBox.Show("Room is not Available. Unable to assign Cleaning.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView2.Columns["Cleaning"].Index && e.RowIndex >= 0)
            {
                string roomStatus = dataGridView2.Rows[e.RowIndex].Cells["room_status_2fl"].Value.ToString();
                if (roomStatus == "Available")
                {
                    int roomNumber = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells["room_number_2fl"].Value);
                    Console.WriteLine($"Назначена уборка на комнату {roomNumber} на 2 этаже");
                    SQLiteConnection LocalConnection = db_processing.GetConnection();
                    db_processing.AssignCleaning(LocalConnection, 10 + roomNumber);
                    dataGridView2.Rows[e.RowIndex].Cells["room_status_2fl"].Value = "Cleaning";
                }
                else
                {
                    MessageBox.Show("Room is not Available. Unable to assign Cleaning.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView3_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView3.Columns["Cleaning"].Index && e.RowIndex >= 0)
            {
                string roomStatus = dataGridView3.Rows[e.RowIndex].Cells["room_status_3fl"].Value.ToString();
                if (roomStatus == "Available")
                {
                    int roomNumber = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["room_number_3fl"].Value);
                    Console.WriteLine($"Назначена уборка на комнату {roomNumber} на 3 этаже");
                    SQLiteConnection LocalConnection = db_processing.GetConnection();
                    db_processing.AssignCleaning(LocalConnection, 20 + roomNumber);
                    dataGridView3.Rows[e.RowIndex].Cells["room_status_3fl"].Value = "Cleaning";
                }
                else
                {
                    MessageBox.Show("Room is not Available. Unable to assign Cleaning.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Cleaning" && e.RowIndex >= 0)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["room_status"]?.Value != null)
                {
                    string roomStatus = dataGridView1.Rows[e.RowIndex].Cells["room_status"].Value.ToString();
                    if (roomStatus != "Available")
                    {
                        e.Value = "";
                    }
                }
            }
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name == "Cleaning" && e.RowIndex >= 0)
            {
                if (dataGridView2.Rows[e.RowIndex].Cells["room_status_2fl"]?.Value != null)
                {
                    string roomStatus = dataGridView2.Rows[e.RowIndex].Cells["room_status_2fl"].Value.ToString();
                    if (roomStatus != "Available")
                    {
                        e.Value = "";
                    }
                }
            }
        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView3.Columns[e.ColumnIndex].Name == "Cleaning" && e.RowIndex >= 0)
            {
                if (dataGridView3.Rows[e.RowIndex].Cells["room_status_3fl"]?.Value != null)
                {
                    string roomStatus = dataGridView3.Rows[e.RowIndex].Cells["room_status_3fl"].Value.ToString();
                    if (roomStatus != "Available")
                    {
                        e.Value = "";
                    }
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null || comboBox3.SelectedItem == null || comboBox1.SelectedItem == null || textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Complete filling the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string selectedFloorString = comboBox2.SelectedItem.ToString();
                int selectedFloor = int.Parse(selectedFloorString);

                string selectedRoomString = comboBox3.SelectedItem.ToString();
                int selectedRoom = int.Parse(selectedRoomString);

                int roomID = (selectedFloor - 1) * 10 + selectedRoom;

                string selectedSalesManager = comboBox1.SelectedItem.ToString();
                string textName = textBox1.Text;
                string textNumber = textBox2.Text;
                string connectionString = "Data Source=C:\\Users\\Enjoy\\source\\repos\\hotel_crm_system\\hotel_crm_system\\hotel_database.db;Version=3;";
                SQLiteConnection connection = new SQLiteConnection(connectionString);

                string roomStatus = db_processing.GetRoomStatus(connection, roomID);
                if (roomStatus == "Cleaning" || roomStatus == "Occupied")
                {
                    MessageBox.Show("Cannot sell room. The room is currently being cleaned or already occupied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Console.WriteLine($"Выбранный этаж: {selectedFloor}, Выбранная комната: {selectedRoom}, Выбранный менеджер: {selectedSalesManager}, Имя клиента: {textName}, Номер телефона клиента: {textNumber}");
                    db_processing.CheckInClient(connection, roomID, textName, textNumber, selectedSalesManager);
                    UpdateRoomData();
                }
            }
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
