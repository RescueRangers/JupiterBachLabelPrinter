using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JupiterBachLabelPrinter.Messages;
using JupiterBachLabelPrinter.Model;
using JupiterBachLabelPrinter.Services;
using Microsoft.Extensions.Logging;
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

		private ILabelAccessService _labelAccessService;
		private ILogger<MainWindowViewModel> _logger;
		private int currentProgress;
		private bool progressBarVisibility = true;
		private ObservableCollection<MasterItem> masterItems;

		public ObservableCollection<MasterItem> MasterItems
		{
			get => masterItems;
			set
			{
				SetProperty(ref masterItems, value);
			}
		}
		public MasterItem SelectedMasterItem
		{
			get => selectedMasterItem;
			set
			{
				SetProperty(ref selectedMasterItem, value);
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

		public int CurrentProgress
		{
			get => currentProgress;
			set
			{
				SetProperty(ref currentProgress, value);
			}
		}

		public bool ProgressBarVisibility
		{
			get => progressBarVisibility;
			set
			{
				SetProperty(ref progressBarVisibility, value);
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

		public MainWindowViewModel(ILabelAccessService labelAccessService, ILogger<MainWindowViewModel> logger)
		{
			PrintLabelsCommand = new RelayCommand(PrintLabels, CanPrintLabels);
			_labelAccessService = labelAccessService;
			_logger = logger;

			WeakReferenceMessenger.Default.Register<WindowLoadedMessage>(this, (r, m) =>
			{
				MasterItems = new ObservableCollection<MasterItem>(_labelAccessService.GetAllLabels());
			});
		}

		private bool CanPrintLabels()
		{
			return SelectedMaterial != null && PrintQuantity >= 1 && IPAddress.TryParse(PrinterIp, out _);
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
