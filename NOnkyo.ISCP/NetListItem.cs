using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOnkyo.ISCP
{
    public class NetListItem
    {

        public NetListItem(int pnLine, string psName)
        {
            this.Line = pnLine;
            this.Name = psName;
        }

        public int Line { get; private set; }
        public string Name { get; private set; }
    }
}
