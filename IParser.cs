using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrvYhteistulokset
{
    internal interface IParser
    {
        void Parse(string sCopyPastedFromWebPage, int iDay, Dictionary<string, Record> results);
    }
}
