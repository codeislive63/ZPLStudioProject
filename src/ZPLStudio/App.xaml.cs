using System.IO;
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
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configuration =>
            {
                configuration.SetBasePath(Directory.GetCurrentDirectory());
                configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("Oracle");
                services.AddDbContext<LabelDbContext>(options =>
                    options.UseOracle(connectionString));

                services.AddSingleton<LabelTemplateOptions>(sp =>
                {
                    var section = context.Configuration.GetSection("Label");
                    return new LabelTemplateOptions
                    {
                        TemplatePath = section.GetValue<string>("TemplatePath") ?? "Templates/EndLabelTemplate.xaml",
                        DefaultTenam = section.GetValue<string>("DefaultTenam") ?? string.Empty,
                        PageWidthMm = section.GetValue<double>("PageWidthMm", 100),
                        PageHeightMm = section.GetValue<double>("PageHeightMm", 150)
                    };
                });

                services.AddScoped<ILabelRepository, LabelRepository>();
                services.AddSingleton<ILabelTemplateService, LabelTemplateService>();
                services.AddSingleton<ILabelPrinter, LabelPrinter>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        _host.Start();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
