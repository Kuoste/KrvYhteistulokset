using System.Diagnostics;
using System.Net;

namespace KrvYhteistulokset
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NavisportParser parser = new();
            Dictionary<string, Record> participants = [];

            for (int iDay = 1; iDay <= Record.CompetitionCount; iDay++)
            {
                Console.WriteLine($"Copypasteta {iDay}. paivan tulokset Navisport-sivulta");
                Console.WriteLine("Kun valmis, syota: Enter ja Ctrl+Z ja Enter");

                // Read until Ctrl+Z is entered
                string? sCopyPastedWebPage = Console.In.ReadToEnd();

                parser.Parse(sCopyPastedWebPage, iDay, participants);
            }

            Console.WriteLine();
            Console.WriteLine("Tulokset:");
            Console.WriteLine("");

            var sortedResults = from r in participants orderby r.Value.TotalTimeInSeconds ascending select r.Value;

            // Report incomplete results

            Console.WriteLine();
            Console.WriteLine("Ei aikoja kaikille osakilpailuille: ");
            Console.WriteLine("");


            foreach (Record r in sortedResults)
            {
                if (r.TotalTimeInSeconds == 0)
                {
                    ParseDailyResults(r.TimesInSeconds, out string sDailyResults);

                    Console.WriteLine($"{r.Guid(),-40} (osakilpailut: {sDailyResults})");
                }
            }

            // Report valid results

            Console.WriteLine();
            Console.WriteLine("Yhteisajat osakilpailuille: ");
            Console.WriteLine("");

            int iPlace = 1;
            foreach (Record r in sortedResults)
            {
                if (r.TotalTimeInSeconds > 0)
                {
                    ParseDailyResults(r.TimesInSeconds, out string sDailyResults);

                    Console.WriteLine($"{iPlace}. {r.Guid(),-40} " +
                        $"{Record.CreateTimeString(r.TotalTimeInSeconds)} (osakilpailut: {sDailyResults})");

                    iPlace++;
                }
            }
        }

        static void ParseDailyResults(int[] iTimesInSeconds, out string sDailyResults)
        {
            sDailyResults = string.Empty;

            for (int i = 0; i < iTimesInSeconds.Length; i++)
            {
                sDailyResults += Record.CreateTimeString(iTimesInSeconds[i]);

                if (i < iTimesInSeconds.Length - 1)
                    sDailyResults += ", ";
            }
        }
    }
}
