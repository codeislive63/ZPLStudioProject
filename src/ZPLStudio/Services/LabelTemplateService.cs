using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheArtOfDev.HtmlRenderer.WPF;
using ZXing;
using ZXing.Common;
using ZPLStudio.Models;

namespace ZPLStudio.Services;

public class LabelTemplateService : ILabelTemplateService
{
    private readonly LabelTemplateOptions _options;
    private readonly Regex _tokenRegex = new("\\{\\{\\s*(\\w+)\\s*\\}\\}", RegexOptions.Compiled);

    public LabelTemplateService(LabelTemplateOptions options)
    {
        _options = options;
    }

    public string TemplatePath => _options.TemplatePath;

    public FrameworkElement BuildLabel(LabelRecord record, string? templateText = null)
    {
        var resolvedTemplate = templateText ?? LoadTemplateText();

        if (_options.TemplateFormat == TemplateFormat.Html)
        {
            var html = ApplyTemplate(resolvedTemplate, record);
            var panel = new HtmlPanel
            {
                Text = html,
                Background = Brushes.White
            };

            return panel;
        }

        using var stringReader = new StringReader(resolvedTemplate);
        using var xmlReader = System.Xml.XmlReader.Create(stringReader);
        var templateElement = (FrameworkElement)XamlReader.Load(xmlReader);
        templateElement.DataContext = record;

        return templateElement;
    }

    public string LoadTemplateText()
    {
        if (!File.Exists(_options.TemplatePath))
        {
            return _options.TemplateFormat == TemplateFormat.Html
                ? "<html><body><div>Template not found.</div></body></html>"
                : "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />";
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

    private string ApplyTemplate(string templateText, LabelRecord record)
    {
        return _tokenRegex.Replace(templateText, match =>
        {
            var token = match.Groups[1].Value;
            if (token.Equals("BarcodeDataUri", StringComparison.OrdinalIgnoreCase))
            {
                return GenerateBarcodeDataUri(record.Tenam);
            }

            var property = typeof(LabelRecord).GetProperty(token, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (property is null)
            {
                return string.Empty;
            }

            var value = property.GetValue(record)?.ToString() ?? string.Empty;
            return WebUtility.HtmlEncode(value);
        });
    }

    private static string GenerateBarcodeDataUri(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 80,
                Width = 420,
                Margin = 1,
                PureBarcode = true
            }
        };

        var pixelData = writer.Write(text);
        var bitmap = BitmapSource.Create(
            pixelData.Width,
            pixelData.Height,
            96,
            96,
            PixelFormats.Gray8,
            null,
            pixelData.Pixels,
            pixelData.Width);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = new MemoryStream();
        encoder.Save(stream);
        var base64 = Convert.ToBase64String(stream.ToArray());
        return $"data:image/png;base64,{base64}";
    }
}
