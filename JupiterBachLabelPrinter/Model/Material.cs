using System.Collections.ObjectModel;

namespace JupiterBachLabelPrinter.Model
{
	public class Material
	{
		public string Name { get; set; }
		public ObservableCollection<Item> Items { get; set; }
		public bool ComplexMaterial { get; set; } = false;

		public override string ToString()
		{
			return Name;
		}
	}
}
