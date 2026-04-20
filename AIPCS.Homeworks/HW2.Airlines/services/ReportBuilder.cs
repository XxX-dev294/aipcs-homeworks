using System.Text;

/// <summary>
/// Строит отчёты по принципу Fluent Interface - методы возвращают this
/// </summary>
class ReportBuilder
{
    private DatabaseManager _db;
    private string _sql = "";
    private string _title = "";
    private string[] _headers = new string[0];
    private int[] _widths = new int[0];
    private bool _numbered = false;

    public ReportBuilder(DatabaseManager db) { _db = db; }

    // Промежуточные методы
    public ReportBuilder Query(string sql)                  { _sql = sql;      return this; }
    public ReportBuilder Title(string title)                { _title = title;  return this; }
    public ReportBuilder Header(params string[] cols)       { _headers = cols; return this; }
    public ReportBuilder ColumnWidths(params int[] widths)  { _widths = widths; return this; }

    // Группа А
    public ReportBuilder Numbered() { _numbered = true; return this; }

    // Терминальный метод собирает строку отчёта
    public string Build()
    {
        var (columns, rows) = _db.ExecuteQuery(_sql);
        var sb = new StringBuilder();

        string[] headers = _headers.Length > 0 ? _headers : columns;
        int n = headers.Length;

        // Ширина каждого столбца
        int[] w = new int[n];
        for (int i = 0; i < n; i++)
            w[i] = i < _widths.Length ? _widths[i] : 20;
        int numW = _numbered ? 5 : 0;
        
        int total = numW;
        for (int i = 0; i < n; i++) total += w[i];
        string sep = new string('-', total);

        if (_title.Length > 0)
            sb.AppendLine($"\n=== {_title} ===");

        // Шапка
        if (_numbered) sb.Append("№".PadRight(numW));
        for (int i = 0; i < n; i++) sb.Append(headers[i].PadRight(w[i]));
        sb.AppendLine();
        sb.AppendLine(sep);

        // Строки данных
        for (int r = 0; r < rows.Count; r++)
        {
            if (_numbered) sb.Append((r + 1).ToString().PadRight(numW));
            for (int c = 0; c < rows[r].Length && c < n; c++)
                sb.Append(rows[r][c].PadRight(w[c]));
            sb.AppendLine();
        }
        return sb.ToString();
    }
    public void Print() => Console.Write(Build());
}
