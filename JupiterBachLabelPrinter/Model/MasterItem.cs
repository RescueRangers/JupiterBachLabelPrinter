using System.Collections.ObjectModel;

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
