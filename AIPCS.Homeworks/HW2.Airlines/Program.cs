using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string dbPath       = "airlines.db";
string airlineCsv   = Path.Combine(AppContext.BaseDirectory, "data/airlines.csv");
string flightCsv    = Path.Combine(AppContext.BaseDirectory, "data/flights.csv");

var db = new DatabaseManager(dbPath);
db.LoadFromCsv(airlineCsv, flightCsv);
Console.WriteLine();

string choice;
do
{
    Console.WriteLine(" Управление авиарейсами");
    Console.WriteLine("1) Показать авиакомпании");
    Console.WriteLine("2) Показать рейсы");
    Console.WriteLine("3) Добавить рейс");
    Console.WriteLine("4) Редактировать рейс");
    Console.WriteLine("5) Удалить рейс");
    Console.WriteLine("6) Отчёты");
    Console.WriteLine("0) Выход");
    Console.WriteLine();
    Console.Write("> ");
    choice = Console.ReadLine()?.Trim() ?? "";
    Console.WriteLine();

    switch (choice)
    {
        case "1": ShowAirlines(db);  break;
        case "2": ShowFlights(db);   break;
        case "3": AddFlight(db);     break;
        case "4": EditFlight(db);    break;
        case "5": DeleteFlight(db);  break;
        case "6": ReportsMenu(db);   break;
        case "0": break;
        default:  Console.WriteLine("Неверный пункт меню"); break;
    }
    Console.WriteLine();
}
while (choice != "0");

static void ShowAirlines(DatabaseManager db)
{
    Console.WriteLine(" Авиакомпании");
    var airlines = db.GetAllAirlines();
    foreach (var a in airlines)
        Console.WriteLine("  " + a);
}

static void ShowFlights(DatabaseManager db)
{
    Console.WriteLine(" Рейсы");
    var flights = db.GetAllFlights();
    foreach (var f in flights)
        Console.WriteLine("  " + f);
}

static void AddFlight(DatabaseManager db)
{
    Console.WriteLine(" Добавление рейса");
    Console.WriteLine("Доступные авиакомпании:");
    foreach (var a in db.GetAllAirlines())
        Console.WriteLine("  " + a);

    Console.Write("ID авиакомпании: ");
    if (!int.TryParse(Console.ReadLine(), out int airlineId))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    Console.Write("Маршрут (например, Москва - Сочи): ");
    string name = Console.ReadLine()?.Trim() ?? "";
    if (name.Length == 0)
    {
        Console.WriteLine("Ошибка: название маршрута не может быть пустым.");
        return;
    }

    Console.Write("Дальность маршрута (км): ");
    if (!int.TryParse(Console.ReadLine(), out int distance))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    try
    {
        var flight = new Flight(0, airlineId, name, distance);
        db.AddFlight(flight);
        Console.WriteLine("Рейс добавлен.");
    }
    catch (ArgumentException ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
}

static void EditFlight(DatabaseManager db)
{
    Console.WriteLine(" Редактирование рейса");
    Console.Write("Введите ID рейса: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    var flight = db.GetFlightById(id);
    if (flight == null)
    {
        Console.WriteLine($"Рейс с ID={id} не найден.");
        return;
    }

    Console.WriteLine($"Текущие данные: {flight}");
    Console.WriteLine("(нажмите Enter, чтобы оставить значение без изменений)");

    // Маршрут
    Console.Write($"Маршрут [{flight.Name}]: ");
    string input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0)
        flight.Name = input;

    // Авиакомпания
    Console.WriteLine("Доступные авиакомпании:");
    foreach (var a in db.GetAllAirlines())
        Console.WriteLine("  " + a);
    Console.Write($"ID авиакомпании [{flight.AirlineId}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && int.TryParse(input, out int newAirlineId))
        flight.AirlineId = newAirlineId;

    // Дальность
    Console.Write($"Дальность км [{flight.DistanceKm}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && int.TryParse(input, out int newDistance))
    {
        try { flight.DistanceKm = newDistance; }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return;
        }
    }
    db.UpdateFlight(flight);
    Console.WriteLine("Данные обновлены.");
}

static void DeleteFlight(DatabaseManager db)
{
    Console.WriteLine("--- Удаление рейса ---");
    Console.Write("Введите ID рейса: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    var flight = db.GetFlightById(id);
    if (flight == null)
    {
        Console.WriteLine($"Рейс с ID={id} не найден.");
        return;
    }

    Console.Write($"Удалить «{flight.Name}»? (да/нет): ");
    string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
    if (confirm == "да")
    {
        db.DeleteFlight(id);
        Console.WriteLine("Рейс удалён.");
    }
    else { Console.WriteLine("Удаление отменено."); }
}

// Подменю отчётов
static void ReportsMenu(DatabaseManager db)
{
    string choice;
    do
    {
        Console.WriteLine(" Отчёты");
        Console.WriteLine("1) Рейсы по авиакомпаниям");
        Console.WriteLine("2) Количество рейсов по авиакомпаниям");
        Console.WriteLine("3) Средняя дальность рейсов по авиакомпаниям");
        Console.WriteLine("0) Назад");
        Console.Write("> ");
        choice = Console.ReadLine()?.Trim() ?? "";

        switch (choice)
        {
            case "1": Report1_FlightsWithAirlines(db); break;
            case "2": Report2_CountByAirline(db);      break;
            case "3": Report3_AvgDistanceByAirline(db); break;
            case "0": break;
            default: Console.WriteLine("Неверный пункт"); break;
        }
        Console.WriteLine();
    }
    while (choice != "0");
}

// Отчёт 1: Все рейсы с названиями авиакомпаний
static void Report1_FlightsWithAirlines(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT f.flight_name, a.airline_name, f.distance_km
                 FROM flight f
                 JOIN airline a ON f.airline_id = a.airline_id
                 ORDER BY f.flight_name")
        .Title("Рейсы по авиакомпаниям")
        .Header("Маршрут", "Авиакомпания", "Дальность (км)")
        .ColumnWidths(30, 22, 15)
        .Numbered()   // Доп задание, нумерация строк
        .Print();
}

// Отчёт 2: Количество рейсов по авиакомпаниям
static void Report2_CountByAirline(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT a.airline_name, COUNT(*) AS cnt
                 FROM flight f
                 JOIN airline a ON f.airline_id = a.airline_id
                 GROUP BY a.airline_name
                 ORDER BY a.airline_name")
        .Title("Количество рейсов по авиакомпаниям")
        .Header("Авиакомпания", "Кол-во рейсов")
        .ColumnWidths(25, 15)
        // .Numbered()
        .Print();
}

// Отчёт 3: Средняя дальность по авиакомпаниям
static void Report3_AvgDistanceByAirline(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"SELECT a.airline_name,
                        ROUND(AVG(f.distance_km), 1) AS avg_distance
                 FROM flight f
                 JOIN airline a ON f.airline_id = a.airline_id
                 GROUP BY a.airline_name
                 ORDER BY avg_distance DESC")
        .Title("Средняя дальность рейсов по авиакомпаниям")
        .Header("Авиакомпания", "Средняя дальность (км)")
        .ColumnWidths(25, 22)
        // .Numbered()
        .Print();
}
