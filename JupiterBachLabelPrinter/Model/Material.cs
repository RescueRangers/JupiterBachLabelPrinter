using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JupiterBachLabelPrinter.Model
{
    public class Material
    {
        public string Name { get; set; }
        public ObservableCollection<Item> Items { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
