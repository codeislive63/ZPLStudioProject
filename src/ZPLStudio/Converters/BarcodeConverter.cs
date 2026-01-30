using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace ZPLStudio.Converters;

public class BarcodeConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var text = value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 120,
                Width = 420,
                Margin = 2,
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

        return bitmap;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
