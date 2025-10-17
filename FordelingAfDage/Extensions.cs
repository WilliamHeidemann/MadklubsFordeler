namespace FordelingAfDage;

public static class Extensions
{
    public static DateTime GetFutureDateSkippingWeekends(this DateTime from, int daysToAdd)
    {
        DateTime date = from;

        while (daysToAdd > 0)
        {
            date = date.AddDays(1);

            if (date.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday)
            {
                continue;
            }

            daysToAdd--;
        }

        return date;
    }

    public static string GetNameFromRoomNumber(this string roomNumber)
    {
        const string path = "C:\\Users\\dkWiSkHe\\RiderProjects\\Madklub\\FordelingAfDage\\Navne.tsv";
        var lookup = new Dictionary<string, string>();
        foreach (var line in File.ReadAllLines(path))
        {
            var split = line.Split("\t");
            var room = split[0];
            var name = split[1];
            lookup.Add(room, name);
        }
        
        if (lookup.TryGetValue(roomNumber, out var roomOwner))
        {
            return roomOwner!;
        }

        throw new Exception($"Id {roomNumber} does not exists in Navne.tsv.");
    }
}