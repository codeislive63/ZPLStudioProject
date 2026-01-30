namespace ZPLStudio.Services;

public class LabelTemplateOptions
{
    public string TemplatePath { get; set; } = "Templates/EndLabelTemplate.xaml";
    public string DefaultTenam { get; set; } = string.Empty;
    public double PageWidthMm { get; set; } = 100;
    public double PageHeightMm { get; set; } = 150;
}
