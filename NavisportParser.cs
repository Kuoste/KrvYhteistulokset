using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KrvYhteistulokset
{
    internal class NavisportParser : IParser
    {
        private const char TimeSeparator = ':';

        public void Parse(string sCopyPastedFromWebPage, int iDay, Dictionary<string, Record> results)
        {
            // People in the copy pasted results are separated by empty line

            // Osallistujia
            // 122
            // 1
            // 
            // LassilaKerttu
            // 48:00
            // 2
            // 
            // KarhuSatu
            // Tampereen Pyrintö
            // 48:07
            // +7
            // 
            // 3
            // 
            // LeinonenJuha
            // Karsikon Koheltajat
            // 50:47
            // +2:47


            // Split by double Enter
            string[] sPeople = sCopyPastedFromWebPage.Split(new string[] { Environment.NewLine + Environment.NewLine },
                               StringSplitOptions.RemoveEmptyEntries);

            foreach (string sPerson in sPeople)
            {
                if (!sPerson.Contains(TimeSeparator))
                {
                    // This is some other entry (header or place row or no result time)
                    Console.WriteLine("Time not found for entry: " + sPerson);
                    Console.WriteLine();
                    continue; 
                }

                string[] sDetails = sPerson.Split(Environment.NewLine);

                if (sDetails.Length < 3)
                {
                    // Some other problem
                    Console.WriteLine("Invalid row count for entry: " + sPerson);
                    Console.WriteLine();
                    continue;
                }

                // First line is always name
                string name = sDetails[0];
                string club = String.Empty;

                // Second second line is club or result time
                if (ParseTime(sDetails[1], out int timeInSeconds) == false)
                {
                    // Couldn't parse time, so the line must contain club
                    club = sDetails[1];

                    // And in this case the 3rd line has the result time
                    if (ParseTime(sDetails[2], out timeInSeconds) == false)
                    {
                        Console.WriteLine("Cannot parse time for entry: " + sPerson);
                        continue;
                    }
                }

                // Update time to results
                if (results.TryGetValue(Record.Guid(name, club), out Record r))
                {
                    if (r.TimesInSeconds[iDay - 1] != 0)
                    {
                        // This happens if there's two people with same name
                        throw new Exception("Error: double entry: " + sPerson);
                    }

                    r.TimesInSeconds[iDay - 1] = timeInSeconds;
                }
                else
                {
                    // Create a new result
                    r = new(name, club);
                    r.TimesInSeconds[iDay - 1] = timeInSeconds;
                    results[r.Guid()] = r;
                }
            }
        }

        private static bool ParseTime(string input, out int timeInSeconds)
        {
            timeInSeconds = 0;

            if (!input.Contains(TimeSeparator))
            {
                return false;
            }

            string[] parts = input.Split(TimeSeparator);
            int coeff = 1;

            foreach (string part in parts.Reverse())
            {
                if (int.TryParse(part, out int secondsMinutesOrHours))
                {
                    timeInSeconds += secondsMinutesOrHours * coeff;
                }
                else
                {
                    return false;
                }

                coeff *= 60;
            }

            return true;
        }
    }
}
