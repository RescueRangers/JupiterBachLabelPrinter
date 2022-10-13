using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JupiterBachLabelPrinter.Model
{
    public class MasterItem
    {
        public string Name { get; set; }
        public string SetNumber { get; set; }
        public ObservableCollection<Material> Materials { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
