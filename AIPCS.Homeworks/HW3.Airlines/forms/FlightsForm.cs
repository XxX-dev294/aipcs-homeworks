using Microsoft.EntityFrameworkCore;

/// <summary>
/// Вспомогательный класс для отображения рейса в таблице
/// </summary>
public class FlightRow
{
    /// <summary>Идентификатор рейса</summary>
    public int Id { get; set; }

    /// <summary>Маршрут рейса</summary>
    public string Name { get; set; } = "";

    /// <summary>Название авиакомпании</summary>
    public string Airline { get; set; } = "";

    /// <summary>Дальность маршрута (км)</summary>
    public int DistanceKm { get; set; }
}

/// <summary>
/// Форма управления рейсами (CRUD)
/// </summary>
public class FlightsForm : Form
{
    private readonly DataGridView _grid;
    private readonly Button _btnAdd;
    private readonly Button _btnEdit;
    private readonly Button _btnDelete;

    /// <summary>
    /// Инициализирует форму рейсов
    /// </summary>
    public FlightsForm()
    {
        Text = "Рейсы";
        Width = 700;
        Height = 450;
        StartPosition = FormStartPosition.CenterParent;

        _grid = new DataGridView
        {
            Left = 10, Top = 10, Width = 660, Height = 360,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false
        };
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id",          HeaderText = "ID",              Width = 45  });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name",        HeaderText = "Маршрут",         Width = 280 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Airline",     HeaderText = "Авиакомпания",    Width = 200 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DistanceKm",  HeaderText = "Дальность (км)",  Width = 110 });

        _btnAdd    = new Button { Text = "Добавить",       Left = 10,  Top = 380, Width = 130, Height = 30 };
        _btnEdit   = new Button { Text = "Редактировать",  Left = 150, Top = 380, Width = 150, Height = 30 };
        _btnDelete = new Button { Text = "Удалить",        Left = 310, Top = 380, Width = 120, Height = 30 };

        _btnAdd.Click    += BtnAdd_Click;
        _btnEdit.Click   += BtnEdit_Click;
        _btnDelete.Click += BtnDelete_Click;

        Controls.AddRange(new Control[] { _grid, _btnAdd, _btnEdit, _btnDelete });
        LoadData();
    }

    /// <summary>
    /// Загружает рейсы с названием авиакомпании через Include
    /// </summary>
    private void LoadData()
    {
        using var context = new AppDbContext();
        var rows = context.Flights
            .Include(f => f.Airline)
            .OrderBy(f => f.Name)
            .Select(f => new FlightRow
            {
                Id          = f.Id,
                Name        = f.Name,
                Airline     = f.Airline!.Name,
                DistanceKm  = f.DistanceKm
            })
            .ToList();
        _grid.DataSource = rows;
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var form = new FlightEditForm(null);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            using var context = new AppDbContext();
            context.Flights.Add(new Flight
            {
                AirlineId  = form.SelectedAirlineId,
                Name       = form.FlightName,
                DistanceKm = form.DistanceKm
            });
            context.SaveChanges();
            LoadData();
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not FlightRow row)
        {
            MessageBox.Show("Выберите рейс для редактирования.", "Внимание",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var loadCtx = new AppDbContext();
        var flight = loadCtx.Flights.Find(row.Id);
        if (flight == null) return;

        var form = new FlightEditForm(flight);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            using var context = new AppDbContext();
            var entity = context.Flights.Find(row.Id);
            if (entity != null)
            {
                entity.AirlineId  = form.SelectedAirlineId;
                entity.Name       = form.FlightName;
                entity.DistanceKm = form.DistanceKm;
                context.SaveChanges();
            }
            LoadData();
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not FlightRow row)
        {
            MessageBox.Show("Выберите рейс для удаления.", "Внимание",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Удалить рейс «{row.Name}»?",
            "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            using var context = new AppDbContext();
            var entity = context.Flights.Find(row.Id);
            if (entity != null)
            {
                context.Flights.Remove(entity);
                context.SaveChanges();
            }
            LoadData();
        }
    }
}
