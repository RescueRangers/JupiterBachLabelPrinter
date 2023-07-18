using System.Collections.Generic;
using System.IO;
using JupiterBachLabelPrinter.Model;
using Newtonsoft.Json;

namespace JupiterBachLabelPrinter.Services
{
	public class JsonLabelsAccessService : ILabelAccessService
	{
		public IEnumerable<MasterItem> GetAllLabels()
		{
			using (var reader = new StreamReader("Jupiter-Bach-Items.json"))
			{
				var root = JsonConvert.DeserializeObject<Root>(reader.ReadToEnd());
				return new List<MasterItem>(root.MasterItems);
			}
		}
	}
}
