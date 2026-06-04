/// <summary>
/// Авиакомпания (справочная таблица, сторона «один»)
/// </summary>
public class Airline
{
    /// <summary>
    /// Идентификатор авиакомпании (первичный ключ)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название авиакомпании
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Навигационное свойство: рейсы этой авиакомпании
    /// </summary>
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
