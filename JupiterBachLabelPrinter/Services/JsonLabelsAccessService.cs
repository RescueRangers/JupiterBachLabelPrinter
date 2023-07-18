using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
