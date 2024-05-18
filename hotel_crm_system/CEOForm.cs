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
    public partial class CEOForm : Form
    {
        public CEOForm()
        {
            InitializeComponent();
            LoadRoomData();
            LoadEmployeeData();
            InitializeTimer();

        }


        static void CheckConnection()
        {
            string connectionString = "Data Source=hotel_database.db;Version=3;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Подключение к базе данных успешно установлено.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения к базе данных: {ex.Message}");
            }
        }

        private void LoadRoomData()
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

        private void LoadEmployeeData()
        {
            string connectionString = "Data Source=C:\\Users\\Enjoy\\source\\repos\\hotel_crm_system\\hotel_crm_system\\hotel_database.db;Version=3;";
            string query = "SELECT Name, Position, WorkPlan FROM Employees";

            dataGridView4.Rows.Clear();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView4.Rows.Add(reader["Name"], reader["Position"], reader["WorkPlan"]);
                        }
                    }
                }
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

        private void UpdateEmployeeData()
        {
            string connectionString = "Data Source=C:\\Users\\Enjoy\\source\\repos\\hotel_crm_system\\hotel_crm_system\\hotel_database.db;Version=3;";
            string query = "SELECT Name, Position, WorkPlan FROM Employees";

            dataGridView4.Rows.Clear();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataGridView4.Rows.Add(reader["Name"], reader["Position"], reader["WorkPlan"]);
                        }
                    }
                }
            }
        }

        private void InitializeTimer()
        {
            Timer timer = new Timer();
            timer.Interval = 10000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateRoomData();
            UpdateEmployeeData();
        }

        private void CEOForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
