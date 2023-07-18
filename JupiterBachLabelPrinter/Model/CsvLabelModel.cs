using CsvHelper.Configuration.Attributes;

namespace JupiterBachLabelPrinter.Model
{
	internal class CsvLabelModel
	{
		[Index(0)]
		public string MasterItemName { get; set; }
		[Index(1)]
		public string MasterItemSetNumber { get; set; }
		[Index(2)]
		public string MasterItemMaterialName { get; set; }
		[Index(3)]
		public string MaterialItemName { get; set; }
	}
}
