using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZPLStudio.Data;
using ZPLStudio.Services;
using ZPLStudio.ViewModels;

namespace ZPLStudio;

public partial class App : Application
{
    private IHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddUserSecrets<App>(optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<ReportSettings>(context.Configuration.GetSection("Reports"));
                services.AddDbContextFactory<OracleDbContext>(options =>
                {
                    var connectionString = context.Configuration.GetConnectionString("Oracle");
                    options.UseOracle(connectionString);
                });
                services.AddSingleton<IPrinterService, PrinterService>();
                services.AddSingleton<IReportService, ReportService>();
                services.AddSingleton<INotificationService, NotificationService>();
                services.AddSingleton<ILabelRepository, LabelRepository>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<MainWindow>();
            })
            .Build();

        _host.Start();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host?.Dispose();
        base.OnExit(e);
    }
}
