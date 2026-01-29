using ZPLStudio.Models;

namespace ZPLStudio.Services;

public interface ILabelRepository
{
    Task<IReadOnlyList<LabelRecord>> GetByTenamAsync(string tenam, CancellationToken cancellationToken);
}
