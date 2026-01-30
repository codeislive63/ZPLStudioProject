using System.Printing;
using ZPLStudio.Models;

namespace ZPLStudio.Services;

public interface ILabelPrinter
{
    Task PrintAsync(LabelRecord record, PrintQueue queue, CancellationToken cancellationToken);
}
