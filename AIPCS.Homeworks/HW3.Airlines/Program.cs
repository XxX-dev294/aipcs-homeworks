using Microsoft.EntityFrameworkCore;

SeedDatabase();

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
Application.Run(new MainForm());

static void SeedDatabase()
{
    using var context = new AppDbContext();
    context.Database.EnsureCreated();

    if (!context.Airlines.Any())
    {
        context.Airlines.AddRange(
            new Airline { Name = "Аэрофлот" },
            new Airline { Name = "S7 Airlines" },
            new Airline { Name = "Победа" },
            new Airline { Name = "Уральские авиалинии" },
            new Airline { Name = "Utair" }
        );
        context.SaveChanges();
    }

    if (!context.Flights.Any())
    {
        var airlines = context.Airlines.ToList();
        int aeroflotId = airlines.First(a => a.Name == "Аэрофлот").Id;
        int s7Id       = airlines.First(a => a.Name == "S7 Airlines").Id;
        int pobedaId   = airlines.First(a => a.Name == "Победа").Id;
        int uralId     = airlines.First(a => a.Name == "Уральские авиалинии").Id;
        int utairId    = airlines.First(a => a.Name == "Utair").Id;

        context.Flights.AddRange(
            new Flight { AirlineId = aeroflotId, Name = "Москва — Санкт-Петербург",    DistanceKm = 634  },
            new Flight { AirlineId = aeroflotId, Name = "Москва — Сочи",               DistanceKm = 1354 },
            new Flight { AirlineId = aeroflotId, Name = "Москва — Владивосток",        DistanceKm = 6430 },
            new Flight { AirlineId = aeroflotId, Name = "Москва — Нью-Йорк",           DistanceKm = 9754 },
            new Flight { AirlineId = s7Id,       Name = "Новосибирск — Москва",        DistanceKm = 2812 },
            new Flight { AirlineId = s7Id,       Name = "Новосибирск — Санкт-Петербург", DistanceKm = 3194 },
            new Flight { AirlineId = s7Id,       Name = "Москва — Лондон",             DistanceKm = 2504 },
            new Flight { AirlineId = pobedaId,   Name = "Москва — Екатеринбург",       DistanceKm = 1416 },
            new Flight { AirlineId = pobedaId,   Name = "Москва — Казань",             DistanceKm = 822  },
            new Flight { AirlineId = pobedaId,   Name = "Москва — Краснодар",          DistanceKm = 1185 },
            new Flight { AirlineId = uralId,     Name = "Екатеринбург — Москва",       DistanceKm = 1416 },
            new Flight { AirlineId = uralId,     Name = "Екатеринбург — Сочи",         DistanceKm = 2200 },
            new Flight { AirlineId = utairId,    Name = "Тюмень — Москва",             DistanceKm = 2144 },
            new Flight { AirlineId = utairId,    Name = "Ханты-Мансийск — Москва",     DistanceKm = 2410 }
        );
        context.SaveChanges();
    }
}
