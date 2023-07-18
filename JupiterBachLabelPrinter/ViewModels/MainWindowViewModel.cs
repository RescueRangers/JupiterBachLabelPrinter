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

		private int printQuantity = 4;
		private string printerIp = "172.25.194.77";

		private ILabelAccessService _labelAccessService;
		private ILabelPrintService _labelPrintService;
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

		public MainWindowViewModel(ILabelAccessService labelAccessService, ILogger<MainWindowViewModel> logger, ILabelPrintService labelPrintService)
		{
			PrintLabelsCommand = new RelayCommand(PrintLabels, CanPrintLabels);
			_labelAccessService = labelAccessService;
			_logger = logger;

			WeakReferenceMessenger.Default.Register<WindowLoadedMessage>(this, (r, m) =>
			{
				MasterItems = new ObservableCollection<MasterItem>(_labelAccessService.GetAllLabels());
			});
			_labelPrintService = labelPrintService;
		}

		private void PrintLabels()
		{
			_labelPrintService.PrintLabels(SelectedMaterial, SelectedMasterItem.SetNumber, PrintQuantity, PrinterIp);
		}

		private bool CanPrintLabels()
		{
			return SelectedMaterial != null && PrintQuantity >= 1 && IPAddress.TryParse(PrinterIp, out _);
		}
	}
}
