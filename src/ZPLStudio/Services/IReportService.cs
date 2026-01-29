using System.Collections.Generic;
using ZPLStudio.Data;

namespace ZPLStudio.Services;

public interface IReportService
{
    void Print(IReadOnlyList<ListForTekartonV> labels, string printerName);
    void OpenDesigner(IReadOnlyList<ListForTekartonV> labels);
}
