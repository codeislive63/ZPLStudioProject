using ZPLStudio.Models;

namespace ZPLStudio.Services;

public interface IFastReportService
{
    void Print(LabelRecord record, string? printerName);
    void EditTemplate();
}
