using System.IO;
using System.Windows;
using System.Windows.Markup;
using ZPLStudio.Models;

namespace ZPLStudio.Services;

public class LabelTemplateService : ILabelTemplateService
{
    private readonly LabelTemplateOptions _options;

    public LabelTemplateService(LabelTemplateOptions options)
    {
        _options = options;
    }

    public string TemplatePath => _options.TemplatePath;

    public FrameworkElement BuildLabel(LabelRecord record)
    {
        var templateText = LoadTemplateText();

        using var stringReader = new StringReader(templateText);
        using var xmlReader = System.Xml.XmlReader.Create(stringReader);
        var templateElement = (FrameworkElement)XamlReader.Load(xmlReader);
        templateElement.DataContext = record;

        return templateElement;
    }

    public string LoadTemplateText()
    {
        if (!File.Exists(_options.TemplatePath))
        {
            return "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />";
        }

        return File.ReadAllText(_options.TemplatePath);
    }

    public void SaveTemplateText(string templateText)
    {
        var directory = Path.GetDirectoryName(_options.TemplatePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(_options.TemplatePath, templateText);
    }
}
