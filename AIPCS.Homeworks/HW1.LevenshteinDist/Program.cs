using System;

while (true)
{
    Console.WriteLine("Первая строка (или 'exit'):");
    string? s1 = Console.ReadLine();

    if (s1 != null && s1.ToLower() == "exit") break;

    Console.WriteLine("Вторая строка:");
    string? s2 = Console.ReadLine();

    int result = CalcDist(s1, s2);
    Console.WriteLine($"Расстояние: {result} \n");
}

static int CalcDist(string? s1, string? s2)
{
    if (s1 == null || s2 == null) return -1;
    if (s1.Length == 0) return s2.Length;
    if (s2.Length == 0) return s1.Length;

    s1 = s1.ToLower();
    s2 = s2.ToLower();

    int n = s1.Length;
    int m = s2.Length;
    
    // Объявление, инициализация матрицы
    int[,] matrix = new int[n + 1, m + 1];

    for (int i = 0; i <= n; i++) matrix[i, 0] = i;
    for (int j = 0; j <= m; j++) matrix[0, j] = j;

    // Вычисление расстояния
    for (int i = 1; i <= n; i++)
    {
        for (int j = 1; j <= m; j++)
        {
            int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
            int deletion = matrix[i - 1, j] + 1;
            int insertion = matrix[i, j - 1] + 1;
            int substitution = matrix[i - 1, j - 1] + cost;

            matrix[i, j] = Math.Min(deletion, Math.Min(insertion, substitution));

            // Дамерау (транспозиция)
            if (i > 1 && j > 1 && s1[i - 1] == s2[j - 2] && s1[i - 2] == s2[j - 1])
            {
                int transposition = matrix[i - 2, j - 2] + cost;
                matrix[i, j] = Math.Min(matrix[i, j], transposition);
            }
        }
    }
    return matrix[n, m];
}
