using FastReport;
using Microsoft.Extensions.Options;
using ZPLStudio.Data;
using ZPLStudio.Views;

namespace ZPLStudio.Services;

public class ReportService : IReportService
{
    private readonly ReportSettings _settings;

    public ReportService(IOptions<ReportSettings> options)
    {
        _settings = options.Value;
    }

    public void Print(IReadOnlyList<ListForTekartonV> labels, string printerName)
    {
        using var report = CreateReport(labels);
        report.Prepare();
        report.PrintSettings.ShowDialog = false;
        report.PrintSettings.Printer = printerName;
        report.Print();
    }

    public void OpenDesigner(IReadOnlyList<ListForTekartonV> labels)
    {
        using var report = CreateReport(labels);
        var window = new TemplateDesignerWindow(report, GetTemplateFullPath());
        window.ShowDialog();
    }

    private Report CreateReport(IReadOnlyList<ListForTekartonV> labels)
    {
        var report = new Report();
        report.Load(GetTemplateFullPath());
        report.RegisterData(labels, "Label");
        report.GetDataSource("Label")?.Enabled = true;
        return report;
    }

    private string GetTemplateFullPath()
    {
        // Шаблон лежит рядом с приложением и может редактироваться в дизайнере.
        return Path.Combine(AppContext.BaseDirectory, _settings.TemplatePath);
    }
}
