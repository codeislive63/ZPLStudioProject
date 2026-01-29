using System.Windows;
using FastReport;
using FastReport.Design;

namespace ZPLStudio.Views;

public partial class TemplateDesignerWindow : Window
{
    private readonly Report _report;
    private readonly string _templatePath;

    public TemplateDesignerWindow(Report report, string templatePath)
    {
        InitializeComponent();
        _report = report;
        _templatePath = templatePath;

        var designer = new DesignerControl
        {
            Report = _report,
            Dock = System.Windows.Forms.DockStyle.Fill
        };

        designer.FileSave += (_, args) =>
        {
            _report.Save(_templatePath);
            args.Cancel = false;
        };

        DesignerHost.Child = designer;
    }
}
