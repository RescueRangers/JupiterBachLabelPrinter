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
