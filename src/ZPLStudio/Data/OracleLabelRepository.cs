using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZPLStudio.Models;
using ZPLStudio.Services;

namespace ZPLStudio.Data;

public class OracleLabelRepository : ILabelRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ILogger<OracleLabelRepository> _logger;

    public OracleLabelRepository(
        IDbContextFactory<AppDbContext> contextFactory,
        ILogger<OracleLabelRepository> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<IReadOnlyList<LabelRecord>> GetByTenamAsync(
        string tenam,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var query = context.ListForTekarton
            .AsNoTracking()
            .Where(item => item.Tenam == tenam);

        var data = await query.ToListAsync(cancellationToken);

        _logger.LogInformation("Loaded {Count} records for TENAM {Tenam}", data.Count, tenam);

        // Маппинг в доменную модель нужен, чтобы не тащить EF-сущности в UI.
        return data.Select(item => new LabelRecord
        {
            Tenam = item.Tenam,
            Artnr = item.Artnr,
            Artbez = item.Artbez,
            Bstchgnam5 = item.Bstchgnam5,
            Bstmg = item.Bstmg,
            Aufid = item.Aufid,
            Gpplz = item.Gpplz,
            Gpbez = item.Gpbez,
            Lndnam = item.Lndnam,
            Gport1 = item.Gport1,
            Gpstrasse = item.Gpstrasse,
            Lfakdnr = item.Lfakdnr,
            Adres = item.Adres,
            Brutto = item.Brutto,
            Tesortnr = item.Tesortnr,
            Lfaempfkdnr = item.Lfaempfkdnr,
            Market = item.Market,
            CountBst = item.CountBst,
            SumBst = item.SumBst
        }).ToList();
    }
}
