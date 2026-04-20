using Microsoft.Data.Sqlite;

/// <summary>
/// Работа с базой данных SQLite
/// </summary>
class DatabaseManager
{
    private string _dbPath;

    public DatabaseManager(string dbPath)
    {
        _dbPath = dbPath;
        CreateTables();
    }

    // Вспомогательный метод для соединения1
    
    private SqliteConnection Connect()
    {
        var conn = new SqliteConnection($"Data Source={_dbPath}");
        conn.Open();
        return conn;
    }

    private void CreateTables()
    {
        using var conn = Connect();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS airline (
                airline_id   INTEGER PRIMARY KEY AUTOINCREMENT,
                airline_name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS flight (
                flight_id   INTEGER PRIMARY KEY AUTOINCREMENT,
                airline_id  INTEGER NOT NULL,
                flight_name TEXT NOT NULL,
                distance_km INTEGER NOT NULL,
                FOREIGN KEY (airline_id) REFERENCES airline(airline_id)
            );";
        cmd.ExecuteNonQuery();
    }

    /// <summary>Загрузить данные из CSV, если таблицы пустые</summary>
    public void LoadFromCsv(string airlinePath, string flightPath)
    {
        if (GetAllAirlines().Count == 0 && File.Exists(airlinePath))
        {
            using var conn = Connect();
            var lines = File.ReadAllLines(airlinePath);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');
                if (parts.Length < 2) continue;
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO airline (airline_id, airline_name) VALUES (@id, @name)";
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                cmd.Parameters.AddWithValue("@name", parts[1].Trim());
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Авиакомпании загружены из CSV");
        }

        if (GetAllFlights().Count == 0 && File.Exists(flightPath))
        {
            using var conn = Connect();
            var lines = File.ReadAllLines(flightPath);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');
                if (parts.Length < 4) continue;
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO flight (flight_id, airline_id, flight_name, distance_km)
                                    VALUES (@id, @airlineId, @name, @distance)";
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                cmd.Parameters.AddWithValue("@airlineId", int.Parse(parts[1]));
                cmd.Parameters.AddWithValue("@name", parts[2].Trim());
                cmd.Parameters.AddWithValue("@distance", int.Parse(parts[3]));
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Рейсы загружены из CSV");
        }
    }

    public List<Airline> GetAllAirlines()
    {
        var list = new List<Airline>();
        using var conn = Connect();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT airline_id, airline_name FROM airline ORDER BY airline_id";
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new Airline(r.GetInt32(0), r.GetString(1)));
        return list;
    }

    public List<Flight> GetAllFlights()
    {
        var list = new List<Flight>();
        using var conn = Connect();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT flight_id, airline_id, flight_name, distance_km FROM flight ORDER BY flight_id";
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new Flight(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetInt32(3)));
        return list;
    }

    public Flight? GetFlightById(int id)
    {
        using var conn = Connect();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT flight_id, airline_id, flight_name, distance_km FROM flight WHERE flight_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var r = cmd.ExecuteReader();
        if (r.Read())
            return new Flight(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetInt32(3));
        return null;
    }

    public void AddFlight(Flight f)
    {
        using var conn = Connect();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO flight (airline_id, flight_name, distance_km) VALUES (@airlineId, @name, @distance)";
        cmd.Parameters.AddWithValue("@airlineId", f.AirlineId);
        cmd.Parameters.AddWithValue("@name", f.Name);
        cmd.Parameters.AddWithValue("@distance", f.DistanceKm);
        cmd.ExecuteNonQuery();
    }

    public void UpdateFlight(Flight f)
    {
        using var conn = Connect();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"UPDATE flight
                            SET airline_id = @airlineId, flight_name = @name, distance_km = @distance
                            WHERE flight_id = @id";
        cmd.Parameters.AddWithValue("@id", f.Id);
        cmd.Parameters.AddWithValue("@airlineId", f.AirlineId);
        cmd.Parameters.AddWithValue("@name", f.Name);
        cmd.Parameters.AddWithValue("@distance", f.DistanceKm);
        cmd.ExecuteNonQuery();
    }

    public void DeleteFlight(int id)
    {
        using var conn = Connect();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM flight WHERE flight_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Выполнить произвольный SQL и вернуть результат для отчётов</summary>
    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = Connect();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var r = cmd.ExecuteReader();

        var columns = new string[r.FieldCount];
        for (int i = 0; i < r.FieldCount; i++)
            columns[i] = r.GetName(i);

        var rows = new List<string[]>();
        while (r.Read())
        {
            var row = new string[r.FieldCount];
            for (int i = 0; i < r.FieldCount; i++)
                row[i] = r.GetValue(i)?.ToString() ?? "";
            rows.Add(row);
        }

        return (columns, rows);
    }
}
