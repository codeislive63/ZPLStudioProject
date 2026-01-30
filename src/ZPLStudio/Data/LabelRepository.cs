using Microsoft.EntityFrameworkCore;
using ZPLStudio.Models;
using ZPLStudio.Services;

namespace ZPLStudio.Data;

public class LabelRepository : ILabelRepository
{
    private readonly LabelDbContext _dbContext;

    public LabelRepository(LabelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<LabelRecord>> GetByTenamAsync(string tenam, CancellationToken cancellationToken)
    {
        // Читаем как есть из view, без трекинга, чтобы не грузить контекст.
        return await _dbContext.LabelRecords
            .AsNoTracking()
            .Where(record => record.Tenam == tenam)
            .ToListAsync(cancellationToken);
    }
}
