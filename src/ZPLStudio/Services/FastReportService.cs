using System;
using System.Collections.Generic;
using System.IO;
using FastReport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZPLStudio.Models;

namespace ZPLStudio.Services;

public class FastReportService : IFastReportService
{
    private readonly PrintingOptions _options;
    private readonly ILogger<FastReportService> _logger;

    public FastReportService(IOptions<PrintingOptions> options, ILogger<FastReportService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public void Print(LabelRecord record, string? printerName)
    {
        var templatePath = ResolveTemplatePath();
        using var report = new Report();
        report.Load(templatePath);
        report.RegisterData(new List<LabelRecord> { record }, "Label");
        var dataSource = report.GetDataSource("Label");
        if (dataSource != null)
        {
            dataSource.Enabled = true;
        }

        report.Prepare();

        if (!string.IsNullOrWhiteSpace(printerName))
        {
            report.PrintSettings.Printer = printerName;
        }

        report.PrintSettings.ShowDialog = false;
        report.Print();

        _logger.LogInformation("Printed label for TENAM {Tenam} on printer {Printer}", record.Tenam, printerName);
    }

    public void EditTemplate()
    {
        var templatePath = ResolveTemplatePath();
        using var report = new Report();
        report.Load(templatePath);

        report.Design();
        report.Save(templatePath);

        _logger.LogInformation("Template saved to {TemplatePath}", templatePath);
    }

    private string ResolveTemplatePath()
    {
        return Path.IsPathRooted(_options.TemplatePath)
            ? _options.TemplatePath
            : Path.Combine(AppContext.BaseDirectory, _options.TemplatePath);
    }
}
