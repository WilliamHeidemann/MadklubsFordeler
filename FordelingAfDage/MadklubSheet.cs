using System.Globalization;

namespace FordelingAfDage;

public class MadklubSheet(string[] people)
{
    public readonly string[] People = people;
    public readonly List<bool[]> RowsOfWishes = [];

    public void Print()
    {
        Console.WriteLine(string.Join("\t", People));
        foreach (var row in RowsOfWishes)
        {
            Console.WriteLine(string.Join("\t", row.Select(b => b ? "1" : "")));
        }
    }

    public static MadklubSheet GetSampleSheet()
    {
        var people = new[] { "43", "44", "45" };
        var madklubGraph = new MadklubSheet(people);

        var wishesDay1 = new[] { true, false, true };
        var wishesDay2 = new[] { true, true, false };
        var wishesDay3 = new[] { false, true, true };

        var wishes = new[] { wishesDay1, wishesDay2, wishesDay3 };

        foreach (var rowOfWishes in wishes)
        {
            madklubGraph.RowsOfWishes.Add(rowOfWishes);
        }

        return madklubGraph;
    }

    public static MadklubSheet ReadFromConsole()
    {
        string[] people = Console.ReadLine()?.Split("\t")!;
        var madklubSheet = new MadklubSheet(people);

        int n = people.Length;
        var i = 0;
        while (i < n)
        {
            string line = Console.ReadLine()!;
            if (line == "stop")
            {
                return madklubSheet;
            }

            var wishes = line.Split("\t").Select(s => !string.IsNullOrWhiteSpace(s)).ToArray()!;
            madklubSheet.RowsOfWishes.Add(wishes);
            i++;
        }

        return madklubSheet;
    }

    public static MadklubSheet ReadFromTsv(TsvTable tsvTable)
    {
        var allPeople = tsvTable.Rows[0];
        var peopleToSkip = tsvTable.Rows[1];

        var people = allPeople.Where((_, i) => string.IsNullOrWhiteSpace(peopleToSkip[i])).ToArray();
        
        var madklubSheet = new MadklubSheet(people);

        const string firstDayOfMadklubString = "20/11/2025";
        DateTime firstDayOfMadklub =
            DateTime.ParseExact(firstDayOfMadklubString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        for (int i = 2; i < tsvTable.RowCount; i++)
        {
            var day = firstDayOfMadklub.AddDays(i - 2);
            if (day.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday)
            {
                continue;
            }

            var rowOfWishes = new List<bool>();
            for (int j = 0; j < peopleToSkip.Length; j++)
            {
                if (!string.IsNullOrWhiteSpace(peopleToSkip[j]))
                {
                    continue;
                }

                bool wish = !string.IsNullOrWhiteSpace(tsvTable[i, j]);
                rowOfWishes.Add(wish);
            }

            madklubSheet.RowsOfWishes.Add(rowOfWishes.ToArray());
        }

        Console.WriteLine($"People={madklubSheet.People.Length} Days={madklubSheet.RowsOfWishes.Count}");

        if (madklubSheet.People.Length != madklubSheet.RowsOfWishes.Count)
        {
            throw new Exception("Number of people is not equal to the number of days.");
        }

        return madklubSheet;
    }
}