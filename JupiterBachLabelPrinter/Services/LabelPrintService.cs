using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using JupiterBachLabelPrinter.Messages;
using JupiterBachLabelPrinter.Model;
using Microsoft.Extensions.Logging;

namespace JupiterBachLabelPrinter.Services
{
	public class LabelPrintService : ILabelPrintService
	{
		private ILogger<LabelPrintService> logger;
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

		public LabelPrintService(ILogger<LabelPrintService> logger)
		{
			this.logger = logger;
		}

		public void PrintLabels(Material material, string setNumber, int printQuantity, string printerIP)
		{
			try
			{
				var tcpClient = new System.Net.Sockets.TcpClient();
				tcpClient.Connect(printerIP, 9100);
				using (var writer = new StreamWriter(tcpClient.GetStream()))
				{
					if (material.ComplexMaterial)
					{
						PrintComplexLabels(material, printQuantity, writer);
					}
					else
					{
						PrintSimpleLabels(material, setNumber, printQuantity, writer);
					}
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				WeakReferenceMessenger.Default.Send<ErrorMessage>(new ErrorMessage(ex.Message));
			}
			
		}

		private void PrintSimpleLabels(Material material, string setNumber, int printQuantity, StreamWriter writer)
		{
			foreach (var item in material.Items.Reverse())
			{
				var zplData = _zpl.Replace("<<Master>>", material.Name);
				zplData = zplData.Replace("<<Quantity>>", $"{printQuantity}");
				zplData = zplData.Replace("<<Number>>", setNumber);
				zplData = zplData.Replace("<<Material>>", material.Name);
				zplData = zplData.Replace("<<Item>>", item.Name);

				writer.Write(zplData);
				writer.Flush();
			}
		}

		private void PrintComplexLabels(Material material, int printQuantity, StreamWriter writer)
		{
			foreach (var item in material.Items.Reverse())
			{
				var zplData = _zpl.Replace("<<Master>>", item.MasterItem);
				zplData = zplData.Replace("<<Quantity>>", $"{printQuantity}");
				zplData = zplData.Replace("<<Number>>", item.SetNumber);
				zplData = zplData.Replace("<<Material>>", material.Name);
				zplData = zplData.Replace("<<Item>>", item.Name);

				writer.Write(zplData);
				writer.Flush();
			}
		}
	}
}
