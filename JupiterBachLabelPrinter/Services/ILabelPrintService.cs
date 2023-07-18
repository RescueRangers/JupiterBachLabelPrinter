using JupiterBachLabelPrinter.Model;

namespace JupiterBachLabelPrinter.Services
{
	public interface ILabelPrintService
	{
		void PrintLabels(Material material, string setNumber, int printQuantity, string printerIP);
	}
}