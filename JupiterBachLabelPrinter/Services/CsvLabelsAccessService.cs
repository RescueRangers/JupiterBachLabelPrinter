using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using CsvHelper;
using CsvHelper.Configuration;
using JupiterBachLabelPrinter.Messages;
using JupiterBachLabelPrinter.Model;
using Microsoft.Extensions.Logging;

namespace JupiterBachLabelPrinter.Services
{
	public class CsvLabelsAccessService : ILabelAccessService
	{
		private readonly CsvConfiguration _config;
		private readonly ILogger<CsvLabelsAccessService> _logger;

		public CsvLabelsAccessService(ILogger<CsvLabelsAccessService> logger)
		{
			_config = new CsvConfiguration(CultureInfo.CurrentCulture)
			{
				NewLine = Environment.NewLine,
				HasHeaderRecord = false,
				Delimiter = ";",
				BadDataFound = null
			};
			_logger = logger;
		}

		public IEnumerable<MasterItem> GetAllLabels()
		{
			var direcotry = new DirectoryInfo("CsvData");
			var masterItems = new List<MasterItem>();

			foreach (var file in direcotry.GetFiles("*.csv"))
			{
				try
				{

					var separatorIndex = file.Name.LastIndexOf('-');
					var masterItemName = file.Name.Substring(0, separatorIndex).Trim();
					var masterItemNumber = file.Name.Substring(separatorIndex + 1, file.Name.Length - separatorIndex - 5).Trim();

					var masterItem = new MasterItem
					{
						Name = masterItemName,
						SetNumber = masterItemNumber,
						Materials = new System.Collections.ObjectModel.ObservableCollection<Material>()
					};

					using (var reader = new StreamReader(file.FullName))
					{
						using (var csv = new CsvReader(reader, _config))
						{
							var records = csv.GetRecords<CsvLabelModel>();
							var list = records.ToList();

							var materials = list.GroupBy(g => g.MasterItemMaterialName);

							foreach (var material in materials)
							{
								if (string.IsNullOrWhiteSpace(material.Key)) continue;

								var items = material.ToList().Select(g => new Item { MasterItem = g.MasterItemName, SetNumber = g.MasterItemSetNumber, Name = g.MaterialItemName });
								var mat = new Material { ComplexMaterial = true, Name = material.Key, Items = new System.Collections.ObjectModel.ObservableCollection<Item>(items) };
								masterItem.Materials.Add(mat);
							}

						}
					}
					masterItems.Add(masterItem);
				}
				catch (ArgumentOutOfRangeException ex)
				{
					var message = $@"Wszystkie pliki muszą zawierać numer zestawu poprzedzony znakiem ""-"".
Błędny plik: {file.Name}.";

					_logger.LogError(ex.Message);
					WeakReferenceMessenger.Default.Send(new ErrorMessage(message));
				}
			}


			return masterItems;
		}
	}
}
