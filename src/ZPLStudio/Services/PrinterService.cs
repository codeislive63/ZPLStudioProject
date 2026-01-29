using System.Drawing.Printing;

namespace ZPLStudio.Services;

public class PrinterService : IPrinterService
{
    public IReadOnlyList<string> GetInstalledPrinters()
    {
        var printers = new List<string>();
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            printers.Add(printer);
        }

        return printers;
    }
}
