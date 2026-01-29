namespace ZPLStudio.Services;

public interface IPrinterService
{
    IReadOnlyList<string> GetInstalledPrinters();
}
