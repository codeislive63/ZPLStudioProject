using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZPLStudio.Data;

public interface ILabelRepository
{
    Task<IReadOnlyList<ListForTekartonV>> GetLabelsAsync(string tenam, CancellationToken cancellationToken = default);
}
