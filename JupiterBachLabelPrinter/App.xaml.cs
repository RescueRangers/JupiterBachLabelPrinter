using System.Windows;
using JupiterBachLabelPrinter.Services;
using JupiterBachLabelPrinter.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Compact;

namespace JupiterBachLabelPrinter
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private ServiceProvider serviceProvider;

		public App()
		{
			ServiceCollection services = new ServiceCollection();
			ConfigureServices(services);
			serviceProvider = services.BuildServiceProvider();
		}

		private void ConfigureServices(ServiceCollection services)
		{
			services
#if DEBUG
			.AddLogging(b =>
			{
				var logger = new LoggerConfiguration()
				.WriteTo.Debug(Serilog.Events.LogEventLevel.Verbose)
				.WriteTo.File(new CompactJsonFormatter(), @".\logs\log.txt", Serilog.Events.LogEventLevel.Verbose, rollingInterval: RollingInterval.Day)
				.CreateLogger();

				b.AddSerilog(logger);
			})
#else
				.AddLogging(b =>
				{
					var logger = new LoggerConfiguration()
					.WriteTo.File(new CompactJsonFormatter(), @".\logs\log.txt", Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day)
					.CreateLogger();

					b.AddSerilog(logger);
				})
#endif
			.AddSingleton<MainWindowViewModel>()
			.AddSingleton<ILabelAccessService, CsvLabelsAccessService>()
			.AddSingleton<ILabelPrintService, LabelPrintService>()
			.AddTransient<MainWindow>();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			var window = serviceProvider.GetRequiredService<MainWindow>();
			window.Show();
		}
	}
}
