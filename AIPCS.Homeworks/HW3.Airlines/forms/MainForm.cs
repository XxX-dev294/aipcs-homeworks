/// <summary>
/// Главное окно приложения с навигацией по разделам
/// </summary>
public class MainForm : Form
{
    /// <summary>
    /// Инициализирует главную форму
    /// </summary>
    public MainForm()
    {
        Text = "Управление авиарейсами";
        Width = 300;
        Height = 220;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var btnAirlines = new Button { Text = "Авиакомпании", Left = 50, Top = 25, Width = 180, Height = 40 };
        var btnFlights  = new Button { Text = "Рейсы",        Left = 50, Top = 75, Width = 180, Height = 40 };
        var btnReport   = new Button { Text = "Отчёты",       Left = 50, Top = 125, Width = 180, Height = 40 };

        btnAirlines.Click += (s, e) => new AirlinesForm().ShowDialog();
        btnFlights.Click  += (s, e) => new FlightsForm().ShowDialog();
        btnReport.Click   += (s, e) => new ReportForm().ShowDialog();

        Controls.AddRange(new Control[] { btnAirlines, btnFlights, btnReport });
    }
}
