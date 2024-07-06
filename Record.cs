using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrvYhteistulokset
{
    internal struct Record(string name, string club)
    {
        public const int CompetitionCount = 4;

        public string Name = name;
        public string Club = club;
        public int[] TimesInSeconds = new int[CompetitionCount];

        public readonly int TotalTimeInSeconds => TimesInSeconds.Contains(0) ? 0 : TimesInSeconds.Sum();

        public static string CreateTimeString(int timeInSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
            return $"{time:hh\\:mm\\:ss}";
        }

        public static string Guid(string sName, string sClub)
        {
            if (string.IsNullOrEmpty(sClub))
                return sName;
             
            return sName + " (" + sClub + ")";
        }

        public readonly string Guid()
        {
            return Guid(Name, Club);
        }
    }
}
