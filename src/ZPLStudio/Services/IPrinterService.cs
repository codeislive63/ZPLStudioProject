using System.Collections.Generic;

namespace ZPLStudio.Services;

public interface IPrinterService
{
    IReadOnlyList<string> GetInstalledPrinters();
}
