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
        // Oracle-провайдер иногда выполняет async синхронно, поэтому уводим запрос в фон.
        return await Task.Run(() =>
            _dbContext.LabelRecords
                .AsNoTracking()
                .Where(record => record.Tenam == tenam)
                .ToList(), cancellationToken);
    }
}
