using System.Collections.Generic;
using JupiterBachLabelPrinter.Model;

namespace JupiterBachLabelPrinter.Services
{
	public interface ILabelAccessService
	{
		IEnumerable<MasterItem> GetAllLabels();
	}
}