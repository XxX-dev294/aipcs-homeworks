using Microsoft.EntityFrameworkCore;

/// <summary>
/// Форма управления справочником авиакомпаний (CRUD)
/// </summary>
public class AirlinesForm : Form
{
    private readonly DataGridView _grid;
    private readonly Button _btnAdd;
    private readonly Button _btnEdit;
    private readonly Button _btnDelete;

    /// <summary>
    /// Инициализирует форму авиакомпаний
    /// </summary>
    public AirlinesForm()
    {
        Text = "Авиакомпании";
        Width = 500;
        Height = 400;
        StartPosition = FormStartPosition.CenterParent;

        _grid = new DataGridView
        {
            Left = 10, Top = 10, Width = 460, Height = 300,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false
        };
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id",   HeaderText = "ID",      Width = 50  });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Название", Width = 380 });

        _btnAdd    = new Button { Text = "Добавить",       Left = 10,  Top = 320, Width = 120, Height = 30 };
        _btnEdit   = new Button { Text = "Редактировать",  Left = 140, Top = 320, Width = 150, Height = 30 };
        _btnDelete = new Button { Text = "Удалить",        Left = 300, Top = 320, Width = 120, Height = 30 };

        _btnAdd.Click    += BtnAdd_Click;
        _btnEdit.Click   += BtnEdit_Click;
        _btnDelete.Click += BtnDelete_Click;

        Controls.AddRange(new Control[] { _grid, _btnAdd, _btnEdit, _btnDelete });
        LoadData();
    }

    /// <summary>
    /// Загружает список авиакомпаний из базы данных
    /// </summary>
    private void LoadData()
    {
        using var context = new AppDbContext();
        _grid.DataSource = context.Airlines.OrderBy(a => a.Name).ToList();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var form = new AirlineEditForm(null);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            using var context = new AppDbContext();
            context.Airlines.Add(new Airline { Name = form.AirlineName });
            context.SaveChanges();
            LoadData();
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not Airline airline)
        {
            MessageBox.Show("Выберите авиакомпанию для редактирования.", "Внимание",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var form = new AirlineEditForm(airline.Name);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            using var context = new AppDbContext();
            var entity = context.Airlines.Find(airline.Id);
            if (entity != null)
            {
                entity.Name = form.AirlineName;
                context.SaveChanges();
            }
            LoadData();
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not Airline airline)
        {
            MessageBox.Show("Выберите авиакомпанию для удаления.", "Внимание",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var context = new AppDbContext();
        bool hasFlights = context.Flights.Any(f => f.AirlineId == airline.Id);
        if (hasFlights)
        {
            MessageBox.Show(
                $"Нельзя удалить авиакомпанию «{airline.Name}»: у неё есть связанные рейсы.",
                "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Удалить авиакомпанию «{airline.Name}»?",
            "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            var entity = context.Airlines.Find(airline.Id);
            if (entity != null)
            {
                context.Airlines.Remove(entity);
                context.SaveChanges();
            }
            LoadData();
        }
    }
}
