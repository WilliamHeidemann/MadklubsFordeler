using System.Globalization;

namespace FordelingAfDage;

// Next step: 
// Create output that can be pasted into google sheets

public class Runner
{
    public static void Main(string[] args)
    {
        ReadTSV();
    }
    
    public static void ReadConsole()
    {
        var madklubSheet = MadklubSheet.ReadFromConsole();
        var graph = new Graph(madklubSheet);
        // graph.PrintGraph();
        graph.PrintFlow();
    }

    public static void ReadTSV()
    {
        var tsv = TsvTable.Load("C:\\Users\\dkWiSkHe\\RiderProjects\\MadklubsFordeler\\FordelingAfDage\\Madklub.tsv");
        // tsv.PrintTable();
        var madklubSheet = MadklubSheet.ReadFromTsv(tsv);
        // madklubSheet.Print();
        var graph = new Graph(madklubSheet);
        graph.PrintFlow();
    }

    public static void GetLastDayOfMadklub()
    {
        const string firstDayOfMadklubString = "20/11/2025";
        DateTime firstDayOfMadklub = DateTime.ParseExact(firstDayOfMadklubString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        const int numberOfPeople = 19;
        DateTime lastDayOfMadklub = firstDayOfMadklub.GetFutureDateSkippingWeekends(numberOfPeople-1);

        Console.WriteLine(lastDayOfMadklub.ToShortDateString());
    }
}