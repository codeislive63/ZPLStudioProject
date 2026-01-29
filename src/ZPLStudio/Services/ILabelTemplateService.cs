using System.Windows;
using ZPLStudio.Models;

namespace ZPLStudio.Services;

public interface ILabelTemplateService
{
    FrameworkElement BuildLabel(LabelRecord record);
    string LoadTemplateText();
    void SaveTemplateText(string templateText);
    string TemplatePath { get; }
}
