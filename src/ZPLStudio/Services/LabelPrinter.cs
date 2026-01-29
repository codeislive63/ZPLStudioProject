using System.Printing;
using System.Windows;
using System.Windows.Controls;
using ZPLStudio.Models;

namespace ZPLStudio.Services;

public class LabelPrinter : ILabelPrinter
{
    private readonly ILabelTemplateService _templateService;
    private readonly LabelTemplateOptions _options;

    public LabelPrinter(ILabelTemplateService templateService, LabelTemplateOptions options)
    {
        _templateService = templateService;
        _options = options;
    }

    public Task PrintAsync(LabelRecord record, PrintQueue queue, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var label = _templateService.BuildLabel(record);
        var size = new Size(MmToPixels(_options.PageWidthMm), MmToPixels(_options.PageHeightMm));

        label.Measure(size);
        label.Arrange(new Rect(size));
        label.UpdateLayout();

        var printDialog = new PrintDialog
        {
            PrintQueue = queue
        };

        printDialog.PrintVisual(label, $"End label {record.Tenam}");
        return Task.CompletedTask;
    }

    private static double MmToPixels(double millimeters)
    {
        const double dpi = 96;
        return millimeters / 25.4 * dpi;
    }
}
