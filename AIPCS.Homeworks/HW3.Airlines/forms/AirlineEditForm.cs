/// <summary>
/// Диалог добавления или редактирования авиакомпании
/// </summary>
public class AirlineEditForm : Form
{
    private readonly TextBox _txtName;

    /// <summary>
    /// Введённое название авиакомпании
    /// </summary>
    public string AirlineName => _txtName.Text.Trim();

    /// <summary>
    /// Инициализирует диалог; currentName = null при добавлении
    /// </summary>
    public AirlineEditForm(string? currentName)
    {
        Text = currentName == null ? "Добавить авиакомпанию" : "Редактировать авиакомпанию";
        Width = 360;
        Height = 150;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        var lblName  = new Label  { Text = "Название:", Left = 10, Top = 22, Width = 80  };
        _txtName     = new TextBox { Left = 100, Top = 18, Width = 220, Text = currentName ?? "" };
        var btnOk     = new Button { Text = "OK",     Left = 100, Top = 65, Width = 100, Height = 30 };
        var btnCancel = new Button { Text = "Отмена", Left = 210, Top = 65, Width = 100, Height = 30,
                                     DialogResult = DialogResult.Cancel };

        btnOk.Click += (s, e) =>
        {
            if (_txtName.Text.Trim().Length == 0)
            {
                MessageBox.Show("Название не может быть пустым.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        };

        AcceptButton = btnOk;
        CancelButton = btnCancel;
        Controls.AddRange(new Control[] { lblName, _txtName, btnOk, btnCancel });
    }
}
