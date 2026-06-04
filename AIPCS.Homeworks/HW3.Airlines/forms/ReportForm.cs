using Microsoft.EntityFrameworkCore;

/// <summary>
/// Форма отчёта с тремя разделами по авиарейсам
/// </summary>
public class ReportForm : Form
{
    /// <summary>
    /// Инициализирует форму отчёта и строит все три раздела
    /// </summary>
    public ReportForm()
    {
        Text = "Отчёты";
        Width = 800;
        Height = 650;
        StartPosition = FormStartPosition.CenterParent;

        int y = 5;

        var lbl1 = new Label { Text = "Раздел 1. Все рейсы с авиакомпанией", Left = 10, Top = y, Width = 400, Height = 20 };
        y += 22;
        var grid1 = BuildGrid1(y);
        y += 155;

        var lbl2 = new Label { Text = "Раздел 2. Количество рейсов по авиакомпаниям", Left = 10, Top = y, Width = 400, Height = 20 };
        y += 22;
        var grid2 = BuildGrid2(y);
        y += 155;

        var lbl3 = new Label { Text = "Раздел 3. Средняя дальность по авиакомпаниям (по убыванию)", Left = 10, Top = y, Width = 500, Height = 20 };
        y += 22;
        var grid3 = BuildGrid3(y);

        Controls.AddRange(new Control[] { lbl1, grid1, lbl2, grid2, lbl3, grid3 });
    }

    /// <summary>
    /// Раздел 1: все рейсы с названием авиакомпании, отсортированные по маршруту
    /// </summary>
    private DataGridView BuildGrid1(int top)
    {
        using var context = new AppDbContext();
        var report1 = context.Flights
            .Include(f => f.Airline)
            .OrderBy(f => f.Name)
            .Select(f => new
            {
                f.Name,
                AirlineName = f.Airline!.Name,
                f.DistanceKm
            })
            .ToList();

        var grid = CreateGrid(top);
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name",        HeaderText = "Маршрут",        Width = 280 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "AirlineName", HeaderText = "Авиакомпания",   Width = 200 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DistanceKm",  HeaderText = "Дальность (км)", Width = 110 });
        grid.DataSource = report1;
        return grid;
    }

    /// <summary>
    /// Раздел 2: количество рейсов по авиакомпаниям
    /// </summary>
    private DataGridView BuildGrid2(int top)
    {
        using var context = new AppDbContext();
        var report2 = context.Flights
            .GroupBy(f => f.Airline!.Name)
            .Select(g => new
            {
                Airline = g.Key,
                Count   = g.Count()
            })
            .OrderBy(r => r.Airline)
            .ToList();

        var grid = CreateGrid(top);
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Airline", HeaderText = "Авиакомпания",  Width = 280 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Count",   HeaderText = "Кол-во рейсов", Width = 130 });
        grid.DataSource = report2;
        return grid;
    }

    /// <summary>
    /// Раздел 3: средняя дальность рейсов по авиакомпаниям, сортировка по убыванию
    /// </summary>
    private DataGridView BuildGrid3(int top)
    {
        using var context = new AppDbContext();
        var report3 = context.Flights
            .GroupBy(f => f.Airline!.Name)
            .Select(g => new
            {
                Airline     = g.Key,
                AvgDistance = g.Average(f => f.DistanceKm)
            })
            .OrderByDescending(r => r.AvgDistance)
            .ToList();

        var grid = CreateGrid(top);
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Airline",     HeaderText = "Авиакомпания",           Width = 280 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "AvgDistance", HeaderText = "Средняя дальность (км)", Width = 180 });
        grid.DataSource = report3;
        return grid;
    }

    /// <summary>
    /// Создаёт стандартный DataGridView для разделов отчёта
    /// </summary>
    private static DataGridView CreateGrid(int top)
    {
        return new DataGridView
        {
            Left = 10, Top = top, Width = 760, Height = 150,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
    }
}
