namespace FordelingAfDage;

public class TsvTable
{
    public List<string[]> Rows { get; } = new();

    public static TsvTable Load(string path)
    {
        var table = new TsvTable();
        foreach (var line in File.ReadAllLines(path))
        {
            table.Rows.Add(line.Split('\t'));
        }

        return table;
    }

    public string this[int row, int col] =>
        row >= 0 && row < Rows.Count && col >= 0 && col < Rows[row].Length
            ? Rows[row][col]
            : string.Empty;

    public int RowCount => Rows.Count;
    public int ColumnCount => Rows.Count > 0 ? Rows[0].Length : 0;

    public void PrintTable(int maxRows = 10, int maxCols = 10)
    {
        for (int r = 0; r < Math.Min(maxRows, RowCount); r++)
        {
            for (int c = 0; c < Math.Min(maxCols, ColumnCount); c++)
            {
                Console.Write($"{this[r, c]}\t");
            }

            Console.WriteLine();
        }
    }
}