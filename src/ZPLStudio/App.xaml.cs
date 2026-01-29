using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZPLStudio.Data;
using ZPLStudio.Services;
using ZPLStudio.ViewModels;

namespace ZPLStudio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(AppContext.BaseDirectory);
                    builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    builder.AddUserSecrets<App>(optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<PrintingOptions>(context.Configuration.GetSection("Printing"));

                    var connectionString = context.Configuration.GetConnectionString("Oracle") ?? string.Empty;
                    services.AddDbContextFactory<AppDbContext>(options =>
                        options.UseOracle(connectionString));
                    services.AddSingleton<IPrinterService, PrinterService>();
                    services.AddSingleton<IFastReportService, FastReportService>();
                    services.AddSingleton<ILabelRepository, OracleLabelRepository>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var window = _host.Services.GetRequiredService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }

}
