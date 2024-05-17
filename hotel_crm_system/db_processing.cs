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
            // Создаём таблицы
            string createRoomsTableQuery = "CREATE TABLE Rooms (RoomID INTEGER PRIMARY KEY, Floor INTEGER, RoomNumber INTEGER, capacity INTEGER, Status TEXT, ClientID INTEGER, ManagerID INTEGER)";
            string createEmployeesTableQuery = "CREATE TABLE Employees (EmployeeID INTEGER PRIMARY KEY, Name TEXT, Position TEXT, WorkPlan TEXT)";
            string createClientsTableQuery = "CREATE TABLE Clients (ClientID INTEGER PRIMARY KEY, Name TEXT, Phone_number TEXT)";

            ExecuteQuery(connection, createRoomsTableQuery);
            ExecuteQuery(connection, createEmployeesTableQuery);
            ExecuteQuery(connection, createClientsTableQuery);
            //Заполняем таблицу комнат
            FillRoomsTable(connection);
            //Дабавляем в список работников директора и менеджера по персоналу и 2-х менеджеров по продажам
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
            for (int roomNumber = 1; roomNumber <= 10; roomNumber++) // 10 комнат на каждом этаже
            {
                totalRooms++;
                string status = "available";
                int clientID = -1; // null
                int managerID = -1; // null

                string insertQuery = $"INSERT INTO Rooms (Floor, RoomNumber, Capacity, Status, ClientID, ManagerID) VALUES ({floor}, {roomNumber}, {capacity}, '{status}', {clientID}, {managerID})";
                ExecuteQuery(connection, insertQuery);
            }
        }

        Console.WriteLine($"Вставлено {totalRooms} записей в таблицу Rooms.");
    }

    static void CheckInClient(SQLiteConnection connection, int roomID, string clientName, string clientPhone, string managerName)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            connection.Open();

            // Проверяем, есть ли клиент уже в базе
            command.CommandText = $"SELECT ClientID FROM Clients WHERE Name = '{clientName}'";
            object clientIDObj = command.ExecuteScalar();
            int clientID;
            if (clientIDObj == null) // Если клиента нет в базе, добавляем его
            {
                command.CommandText = $"INSERT INTO Clients (Name, Phone_number) VALUES ('{clientName}', '{clientPhone}')";
                command.ExecuteNonQuery();
                command.CommandText = "SELECT last_insert_rowid()"; // Получаем последний вставленный идентификатор
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

    static void CheckOutClient(SQLiteConnection connection, int roomID)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            connection.Open();
            command.CommandText = $"UPDATE Rooms SET Status = 'Available', ClientID = NULL, ManagerID = NULL WHERE RoomID = {roomID}";
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    static void EmploySalesMan(SQLiteConnection connection, string name, string position, string workPlan)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            connection.Open();
            command.CommandText = $"INSERT INTO Employees (Name, Position, WorkPlan) VALUES ('{name}', '{position}', '{workPlan}')";
            command.ExecuteNonQuery();

            connection.Close();
        }
    }

    static void DismissEmployee(SQLiteConnection connection, int employeeID)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            connection.Open();

            // Проверяем должность сотрудника
            command.CommandText = $"SELECT Position FROM Employees WHERE EmployeeID = {employeeID}";
            string position = command.ExecuteScalar()?.ToString();

            if (position == "Sales manager")
            {
                // Удаляем сотрудника
                command.CommandText = $"DELETE FROM Employees WHERE EmployeeID = {employeeID}";
                command.ExecuteNonQuery();
                Console.WriteLine($"Сотрудник с ID {employeeID} уволен.");
            }
            else
            {
                Console.WriteLine($"Сотрудник с ID {employeeID} не является менеджером по продажам и не может быть уволен.");
            }

            connection.Close();
        }
    }


    static void UpdateWorkPlan(SQLiteConnection connection, int employeeID, string newWorkPlan)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
            connection.Open();

            // Проверяем должность сотрудника
            command.CommandText = $"SELECT Position FROM Employees WHERE EmployeeID = {employeeID}";
            string position = command.ExecuteScalar()?.ToString();

            if (position == "Sales manager")
            {
                // Обновляем рабочий план сотрудника
                command.CommandText = $"UPDATE Employees SET WorkPlan = '{newWorkPlan}' WHERE EmployeeID = {employeeID}";
                command.ExecuteNonQuery();
                Console.WriteLine($"Рабочий план для сотрудника с ID {employeeID} изменен на {newWorkPlan}.");
            }
            else
            {
                Console.WriteLine($"Сотрудник с ID {employeeID} не является менеджером по продажам, поэтому его рабочий план не может быть изменен.");
            }

            connection.Close();
        }
    }



    public static void AssignCleaning(SQLiteConnection connection, int roomID)
    {
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {

            // Проверяем статус комнаты
            command.CommandText = $"SELECT Status FROM Rooms WHERE RoomID = {roomID}";
            string status = command.ExecuteScalar().ToString();

            if (status == "available")
            {
                // Изменяем статус комнаты на "Cleaning"
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