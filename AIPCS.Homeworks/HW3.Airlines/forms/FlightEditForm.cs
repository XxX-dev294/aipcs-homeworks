/// <summary>
/// Диалог добавления или редактирования рейса
/// </summary>
public class FlightEditForm : Form
{
    private readonly TextBox  _txtName;
    private readonly ComboBox _cmbAirline;
    private readonly TextBox  _txtDistance;

    /// <summary>
    /// Введённый маршрут рейса
    /// </summary>
    public string FlightName => _txtName.Text.Trim();

    /// <summary>
    /// Выбранный идентификатор авиакомпании
    /// </summary>
    public int SelectedAirlineId => ((Airline)_cmbAirline.SelectedItem!).Id;

    /// <summary>
    /// Введённая дальность маршрута (км)
    /// </summary>
    public int DistanceKm { get; private set; }

    /// <summary>
    /// Инициализирует диалог; flight = null при добавлении
    /// </summary>
    public FlightEditForm(Flight? flight)
    {
        Text = flight == null ? "Добавить рейс" : "Редактировать рейс";
        Width = 420;
        Height = 220;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        var lblName     = new Label { Text = "Маршрут:",       Left = 10, Top = 22,  Width = 120 };
        _txtName        = new TextBox { Left = 140, Top = 18,  Width = 240, Text = flight?.Name ?? "" };

        var lblAirline  = new Label { Text = "Авиакомпания:",  Left = 10, Top = 58,  Width = 120 };
        _cmbAirline     = new ComboBox { Left = 140, Top = 55, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };

        var lblDistance = new Label { Text = "Дальность (км):", Left = 10, Top = 95, Width = 120 };
        _txtDistance    = new TextBox { Left = 140, Top = 91,  Width = 240,
                                        Text = flight != null ? flight.DistanceKm.ToString() : "" };

        var btnOk     = new Button { Text = "OK",     Left = 140, Top = 140, Width = 100, Height = 30 };
        var btnCancel = new Button { Text = "Отмена", Left = 250, Top = 140, Width = 100, Height = 30,
                                     DialogResult = DialogResult.Cancel };

        LoadAirlines(flight?.AirlineId);

        btnOk.Click += (s, e) =>
        {
            if (_txtName.Text.Trim().Length == 0)
            {
                MessageBox.Show("Маршрут не может быть пустым.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_cmbAirline.SelectedItem == null)
            {
                MessageBox.Show("Выберите авиакомпанию.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(_txtDistance.Text.Trim(), out int dist) || dist < 0)
            {
                MessageBox.Show("Дальность маршрута должна быть неотрицательным целым числом.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DistanceKm = dist;
            DialogResult = DialogResult.OK;
            Close();
        };

        AcceptButton = btnOk;
        CancelButton = btnCancel;
        Controls.AddRange(new Control[] { lblName, _txtName, lblAirline, _cmbAirline,
                                           lblDistance, _txtDistance, btnOk, btnCancel });
    }

    /// <summary>
    /// Загружает список авиакомпаний в выпадающий список
    /// </summary>
    private void LoadAirlines(int? selectedId)
    {
        using var context = new AppDbContext();
        var airlines = context.Airlines.OrderBy(a => a.Name).ToList();
        _cmbAirline.DataSource    = airlines;
        _cmbAirline.DisplayMember = "Name";
        _cmbAirline.ValueMember   = "Id";

        if (selectedId.HasValue)
            _cmbAirline.SelectedValue = selectedId.Value;
    }
}
