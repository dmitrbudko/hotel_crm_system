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

            string connectionString = "Data Source=C:\\Users\\Valenok\\source\\repos\\hotel_crm_system\\hotel_crm_system\\hotel_database.db;Version=3;";
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
        private void FillDataGridView(DataGridView dataGridView, string query, SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    dataGridView.Rows.Clear();

                    while (reader.Read())
                    {
                        dataGridView.Rows.Add(reader["RoomNumber"], reader["Status"]);
                    }
                }
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Cleaning"].Index && e.RowIndex >= 0)
            {
                string roomStatus = dataGridView1.Rows[e.RowIndex].Cells["room_status"].Value.ToString();
                if (roomStatus == "available")
                {
                    int roomNumber = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["room_number"].Value);
                    Console.WriteLine($"Назначена уборка на комнату {roomNumber} на 1 этаже");
                    SQLiteConnection LocalConnection = db_processing.GetConnection();
                    db_processing.AssignCleaning(LocalConnection, roomNumber);
                    dataGridView1.Rows[e.RowIndex].Cells["room_status"].Value = "cleaning";
                }
                else
                {
                    MessageBox.Show("Нельзя назначить уборку на эту комнату, так как она не доступна.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView2.Columns["Cleaning"].Index && e.RowIndex >= 0)
            {
                string roomStatus = dataGridView2.Rows[e.RowIndex].Cells["room_status_2fl"].Value.ToString();
                if (roomStatus == "available")
                {
                    int roomNumber = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells["room_number_2fl"].Value);
                    Console.WriteLine($"Назначена уборка на комнату {roomNumber} на 2 этаже");
                    SQLiteConnection LocalConnection = db_processing.GetConnection();
                    db_processing.AssignCleaning(LocalConnection, 10 + roomNumber);
                    dataGridView2.Rows[e.RowIndex].Cells["room_status"].Value = "cleaning";
                }
                else
                {
                    MessageBox.Show("Нельзя назначить уборку на эту комнату, так как она не доступна.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView3_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView3.Columns["Cleaning"].Index && e.RowIndex >= 0)
            {
                string roomStatus = dataGridView3.Rows[e.RowIndex].Cells["room_status_3fl"].Value.ToString();
                if (roomStatus == "available")
                {
                    int roomNumber = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["room_number_3fl"].Value);
                    Console.WriteLine($"Назначена уборка на комнату {roomNumber} на 3 этаже");
                    SQLiteConnection LocalConnection = db_processing.GetConnection();
                    db_processing.AssignCleaning(LocalConnection, 20 + roomNumber);
                    dataGridView3.Rows[e.RowIndex].Cells["room_status"].Value = "cleaning";
                }
                else
                {
                    MessageBox.Show("Нельзя назначить уборку на эту комнату, так как она не доступна.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
