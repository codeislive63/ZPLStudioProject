using Microsoft.EntityFrameworkCore;

namespace ZPLStudio.Data;

public class LabelRepository : ILabelRepository
{
    private readonly IDbContextFactory<OracleDbContext> _dbContextFactory;

    public LabelRepository(IDbContextFactory<OracleDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyList<ListForTekartonV>> GetLabelsAsync(string tenam, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.ListForTekarton
            .Where(item => item.Tenam == tenam)
            .ToListAsync(cancellationToken);
    }
}
