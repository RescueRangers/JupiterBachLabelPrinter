using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JupiterBachLabelPrinter.Model;
using Newtonsoft.Json;

namespace JupiterBachLabelPrinter.ViewModels
{
	public class MainWindowViewModel : ObservableObject
	{
		private MasterItem selectedMasterItem;
		private Material selectedMaterial;
		private readonly string _zpl = @"^XA
^PQ<<Quantity>>
^CF0,80
^FO50,50^FD<<Master>>^FS
^CF0,60
^FO1150,50,1^FD<<Number>>^FS
^CF0,80
^FO50,190^FD<<Material>>^FS
^CF0,200
^FO100,290^FD<<Item>>^FS
^XZ";
		private int printQuantity = 4;
		private string printerIp = "172.25.194.77";

		public ObservableCollection<MasterItem> MasterItems { get; set; }
		public MasterItem SelectedMasterItem
		{
			get => selectedMasterItem;
			set
			{
				selectedMasterItem = value;
				OnPropertyChanged(nameof(SelectedMasterItem));
			}
		}

		public Material SelectedMaterial
		{
			get => selectedMaterial;
			set
			{
				if (SetProperty(ref selectedMaterial, value))
				{
					PrintLabelsCommand.NotifyCanExecuteChanged();
				}
			}
		}

		public string PrinterIp
		{
			get => printerIp; 
			set
			{
				if (SetProperty(ref printerIp, value))
				{
					PrintLabelsCommand.NotifyCanExecuteChanged();
				}
			}
		}
		public IRelayCommand PrintLabelsCommand { get; set; }

		public int PrintQuantity
		{
			get => printQuantity;
			set
			{
				if (SetProperty(ref printQuantity, value))
				{
					PrintLabelsCommand.NotifyCanExecuteChanged();
				}
			}
		}

		public MainWindowViewModel()
		{
			PrintLabelsCommand = new RelayCommand(PrintLabels, CanPrintLabels);
			CreateItems();
		}

		private bool CanPrintLabels()
		{
			return SelectedMaterial != null && PrintQuantity >= 1 && IPAddress.TryParse(PrinterIp, out _);
		}

		private void CreateItems()
		{
			using (var reader = new StreamReader("Jupiter-Bach-Items.json"))
			{
				var root = JsonConvert.DeserializeObject<Root>(reader.ReadToEnd());
				MasterItems = new ObservableCollection<MasterItem>(root.MasterItems);
			}
		}

		private void PrintLabels()
		{
			var tcpClient = new System.Net.Sockets.TcpClient();
			tcpClient.Connect(PrinterIp, 9100);
			using (var writer = new StreamWriter(tcpClient.GetStream()))
			{
				if (SelectedMaterial.ComplexMaterial)
				{
					foreach (var item in SelectedMaterial.Items.Reverse())
					{
						var zplData = _zpl.Replace("<<Master>>", item.MasterItem);
						zplData = zplData.Replace("<<Quantity>>", $"{PrintQuantity}");
						zplData = zplData.Replace("<<Number>>", item.SetNumber);
						zplData = zplData.Replace("<<Material>>", SelectedMaterial.Name);
						zplData = zplData.Replace("<<Item>>", item.Name);

						writer.Write(zplData);
						writer.Flush();
					}
				}
				else
				{
					foreach (var item in SelectedMaterial.Items.Reverse())
					{
						var zplData = _zpl.Replace("<<Master>>", SelectedMasterItem.Name);
						zplData = zplData.Replace("<<Quantity>>", $"{PrintQuantity}");
						zplData = zplData.Replace("<<Number>>", SelectedMasterItem.SetNumber);
						zplData = zplData.Replace("<<Material>>", SelectedMaterial.Name);
						zplData = zplData.Replace("<<Item>>", item.Name);

						writer.Write(zplData);
						writer.Flush();
					}
				}
			}
		}

	}
}
