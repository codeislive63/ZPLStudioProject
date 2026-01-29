namespace ZPLStudio.Models;

public class LabelRecord
{
    public string Tenam { get; init; } = string.Empty;
    public string Artnr { get; init; } = string.Empty;
    public string Artbez { get; init; } = string.Empty;
    public string Bstchgnam5 { get; init; } = string.Empty;
    public decimal Bstmg { get; init; }
    public string Aufid { get; init; } = string.Empty;
    public string Gpplz { get; init; } = string.Empty;
    public string Gpbez { get; init; } = string.Empty;
    public string Lndnam { get; init; } = string.Empty;
    public string Gport1 { get; init; } = string.Empty;
    public string Gpstrasse { get; init; } = string.Empty;
    public string Lfakdnr { get; init; } = string.Empty;
    public string Adres { get; init; } = string.Empty;
    public decimal Brutto { get; init; }
    public int Tesortnr { get; init; }
    public string Lfaempfkdnr { get; init; } = string.Empty;
    public string? Market { get; init; }
    public int CountBst { get; init; }
    public int SumBst { get; init; }
}
