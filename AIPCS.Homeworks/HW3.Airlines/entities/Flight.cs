/// <summary>
/// Рейс (основная таблица, сторона «много»)
/// </summary>
public class Flight
{
    /// <summary>
    /// Идентификатор рейса (первичный ключ)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор авиакомпании (внешний ключ)
    /// </summary>
    public int AirlineId { get; set; }

    /// <summary>
    /// Навигационное свойство: авиакомпания рейса
    /// </summary>
    public Airline? Airline { get; set; }

    /// <summary>
    /// Маршрут рейса
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Дальность маршрута (км)
    /// </summary>
    public int DistanceKm { get; set; }
}
