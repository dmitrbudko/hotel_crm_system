using System;
using System.Data.SQLite;
using System.IO;

class db_processing
{
    public static SQLiteConnection GetConnection()
    {
        string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string directoryPath = Path.Combine(projectDirectory, "..", "..");
        string dbFilePath = Path.Combine(directoryPath, "hotel_database.db");
        string connectionString = $"Data Source={dbFilePath};Version=3;";

        SQLiteConnection connection = new SQLiteConnection(connectionString);
        connection.Open(); 
        return connection; 
    }

    public static void InitializeTables()
    {
        string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string directoryPath = Path.Combine(projectDirectory, "..", "..");
        string dbFilePath = Path.Combine(directoryPath, "hotel_database.db");

        if (File.Exists(dbFilePath))
        {
            File.Delete(dbFilePath);
        }

        SQLiteConnection.CreateFile(dbFilePath);
        Console.WriteLine("База данных успешно создана.");

        string connectionString = $"Data Source={dbFilePath};Version=3;";
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string createRoomsTableQuery = "CREATE TABLE Rooms (RoomID INTEGER PRIMARY KEY, Floor INTEGER, RoomNumber INTEGER, capacity INTEGER, Status TEXT, ClientID INTEGER, ManagerID INTEGER)";
            string createEmployeesTableQuery = "CREATE TABLE Employees (EmployeeID INTEGER PRIMARY KEY, Name TEXT, Position TEXT, WorkPlan TEXT)";
            string createClientsTableQuery = "CREATE TABLE Clients (ClientID INTEGER PRIMARY KEY, Name TEXT, Phone_number TEXT)";

            ExecuteQuery(connection, createRoomsTableQuery);
            ExecuteQuery(connection, createEmployeesTableQuery);
            ExecuteQuery(connection, createClientsTableQuery);
    
            FillRoomsTable(connection);
  
            string addDirectorQuery = "INSERT INTO Employees (Name, Position, WorkPlan) VALUES ('Ivan Directorov', 'Director', '8:00 - 18:00')";
            ExecuteQuery(connection, addDirectorQuery);
            string addHRmanager = "INSERT INTO Employees (Name, Position, WorkPlan) VALUES ('Anatoliy Personalchenko', 'HR manager', '8:00 - 18:00');";
            ExecuteQuery(connection, addHRmanager);
            string addSalesManager = "INSERT INTO Employees (Name, Position, WorkPlan) VALUES ('Alexandr Pordajnikov', 'Sales manager', '6:00-14:00'), ('Vladimir Prodajko', 'Sales manager', '14:00 - 22:00');";
            ExecuteQuery(connection, addSalesManager);
            connection.Close();
            Console.WriteLine($"Добавлены записи о сотрудниках по умолчаниюю");
        }
    }

    static void FillRoomsTable(SQLiteConnection connection)
    {

        int[] floors = { 1, 2, 3 };
        int[] capacities = { 4, 2, 1 };
        int totalRooms = 0;

        foreach (int floor in floors)
        {
            int capacity = capacities[floor - 1];
            for (int roomNumber = 1; roomNumber <= 10; roomNumber++)
            {
                totalRooms++;
                string status = "Available";
                int clientID = -1;
                int managerID = -1; 

                string insertQuery = $"INSERT INTO Rooms (Floor, RoomNumber, Capacity, Status, ClientID, ManagerID) VALUES ({floor}, {roomNumber}, {capacity}, '{status}', {clientID}, {managerID})";
                ExecuteQuery(connection, insertQuery);
            }
        }

        Console.WriteLine($"Вставлено {totalRooms} записей в таблицу Rooms.");
    }

    public static void CheckInClient(SQLiteConnection connection, int roomID, string clientName, string clientPhone, string managerName)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            connection.Open();
            command.CommandText = $"SELECT ClientID FROM Clients WHERE Name = '{clientName}'";
            object clientIDObj = command.ExecuteScalar();
            int clientID;
            if (clientIDObj == null)
            {
                command.CommandText = $"INSERT INTO Clients (Name, Phone_number) VALUES ('{clientName}', '{clientPhone}')";
                command.ExecuteNonQuery();
                command.CommandText = "SELECT last_insert_rowid()"; //последний вставленный идентификатор
                clientID = Convert.ToInt32(command.ExecuteScalar());
            }
            else
            {
                clientID = Convert.ToInt32(clientIDObj);
            }

            command.CommandText = $"UPDATE Rooms SET Status = 'Occupied', ClientID = {clientID}, ManagerID = (SELECT EmployeeID FROM Employees WHERE Name = '{managerName}') WHERE RoomID = {roomID}";
            command.ExecuteNonQuery();

            connection.Close();
        }
    }

    public static string GetRoomStatus(SQLiteConnection connection, int roomID)
    {
        connection.Open();
        string status = "";
        string query = "SELECT Status FROM Rooms WHERE RoomID = @RoomID";

        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@RoomID", roomID);

            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    status = reader["Status"].ToString();
                }
            }
        }
        connection.Close();
        return status;
    }
    public static void CheckOutClient(SQLiteConnection connection, int roomID)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            connection.Open();
            command.CommandText = $"UPDATE Rooms SET Status = 'Available', ClientID = NULL, ManagerID = NULL WHERE RoomID = {roomID}";
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public static void EmploySomeone(SQLiteConnection connection, string name, string position, string workPlan)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            connection.Open();
            command.CommandText = $"INSERT INTO Employees (Name, Position, WorkPlan) VALUES ('{name}', '{position}', '{workPlan}')";
            command.ExecuteNonQuery();

            connection.Close();
        }
    }

    public static void DismissEmployee(SQLiteConnection connection, string employeeName)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            command.CommandText = $"SELECT Position FROM Employees WHERE Name = @Name";
            command.Parameters.AddWithValue("@Name", employeeName);
            string position = command.ExecuteScalar()?.ToString();

            if (position != "Director" && position != "HR manager")
            {
                command.CommandText = $"DELETE FROM Employees WHERE Name = @Name";
                command.ExecuteNonQuery();
                Console.WriteLine($"Сотрудник с именем {employeeName} уволен.");
            }
            else
            {
                Console.WriteLine($"Сотрудник с именем {employeeName} не может быть уволен.");
            }

            connection.Close();
        }
    }



    public static void UpdateWorkPlan(SQLiteConnection connection, string employeeName, string newWorkPlan)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            command.CommandText = "SELECT Position FROM Employees WHERE Name = @employeeName";
            command.Parameters.AddWithValue("@employeeName", employeeName);
            string position = command.ExecuteScalar()?.ToString();

            if (position != "Director" && position != "HR manager")
            {
                command.CommandText = "UPDATE Employees SET WorkPlan = @newWorkPlan WHERE Name = @employeeName";
                command.Parameters.AddWithValue("@newWorkPlan", newWorkPlan);
                command.ExecuteNonQuery();
                Console.WriteLine($"Рабочий план для сотрудника с именем {employeeName} изменен на {newWorkPlan}.");
            }
            else
            {
                Console.WriteLine($"Сотрудник с именем {employeeName} не является менеджером по продажам, поэтому его рабочий план не может быть изменен.");
            }

            connection.Close();
        }
    }



    public static void AssignCleaning(SQLiteConnection connection, int roomID)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            command.CommandText = $"SELECT Status FROM Rooms WHERE RoomID = {roomID}";
            string status = command.ExecuteScalar().ToString();

            if (status == "Available")
            {
                command.CommandText = $"UPDATE Rooms SET Status = 'Cleaning' WHERE RoomID = {roomID}";
                command.ExecuteNonQuery();
                Console.WriteLine($"Уборка назначена для комнаты с ID {roomID}");
            }
            else
            {
                Console.WriteLine($"Комната с ID {roomID} не доступна для уборки.");
            }

            connection.Close();
        }
    }

    static void ExecuteQuery(SQLiteConnection connection, string query)
    {
        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        {
            command.ExecuteNonQuery();
        }
    }
}