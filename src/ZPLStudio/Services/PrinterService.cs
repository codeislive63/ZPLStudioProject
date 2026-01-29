using System.Drawing.Printing;
using System.Linq;

namespace ZPLStudio.Services;

public class PrinterService : IPrinterService
{
    public IReadOnlyList<string> GetInstalledPrinters()
    {
        return PrinterSettings.InstalledPrinters
            .Cast<string>()
            .OrderBy(printer => printer)
            .ToList();
    }
}
