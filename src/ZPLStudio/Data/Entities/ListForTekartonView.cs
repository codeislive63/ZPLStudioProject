using System.ComponentModel.DataAnnotations.Schema;

namespace ZPLStudio.Data.Entities;

[Table("LIST_FOR_TEKARTON_V", Schema = "MLSOFT")]
public class ListForTekartonView
{
    // Свойства совпадают с колонками Oracle View, при сколдинге этот класс будет обновлен.
    public string Tenam { get; set; } = string.Empty;
    public string Artnr { get; set; } = string.Empty;
    public string Artbez { get; set; } = string.Empty;
    public string Bstchgnam5 { get; set; } = string.Empty;
    public decimal Bstmg { get; set; }
    public string Aufid { get; set; } = string.Empty;
    public string Gpplz { get; set; } = string.Empty;
    public string Gpbez { get; set; } = string.Empty;
    public string Lndnam { get; set; } = string.Empty;
    public string Gport1 { get; set; } = string.Empty;
    public string Gpstrasse { get; set; } = string.Empty;
    public string Lfakdnr { get; set; } = string.Empty;
    public string Adres { get; set; } = string.Empty;
    public decimal Brutto { get; set; }
    public int Tesortnr { get; set; }
    public string Lfaempfkdnr { get; set; } = string.Empty;
    public string? Market { get; set; }
    public int CountBst { get; set; }
    public int SumBst { get; set; }
}
