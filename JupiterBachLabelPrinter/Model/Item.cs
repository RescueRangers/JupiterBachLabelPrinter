using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JupiterBachLabelPrinter.Model
{
    public class Item
    {
        public string Name { get; set; }
        public string MasterItem { get; set; }
        public string SetNumber { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
