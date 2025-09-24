using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace ExtraMarioWin.Converters
{
    public class GuidToPastelBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush[] Palette = new[]
        {
            FromHex("#FFCCE5FF"), // light blue
            FromHex("#FFFFE0CC"), // peach
            FromHex("#FFE6FFCC"), // light green
            FromHex("#FFFFCCF2"), // pink
            FromHex("#FFE0FFF4"), // mint
            FromHex("#FFFFF5CC"), // light yellow
            FromHex("#FFE0E7FF"), // periwinkle
            FromHex("#FFFDE2E4"), // rose
            FromHex("#FFE3F2FD"), // sky
            FromHex("#FFECFDF5"), // teal mint
            FromHex("#FFF5F3FF"), // lavender
            FromHex("#FFFFF1F2"), // blush
        };

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guid guid)
            {
                // Deterministic index from GUID bytes
                var bytes = guid.ToByteArray();
                int sum = 0;
                for (int i = 0; i < bytes.Length; i++) sum += bytes[i];
                var index = Math.Abs(sum) % Palette.Length;
                return Palette[index];
            }
            return Palette[0];
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static SolidColorBrush FromHex(string hex)
        {
            var brush = (SolidColorBrush)new BrushConverter().ConvertFrom(hex)!;
            // Freeze for perf
            if (brush.CanFreeze) brush.Freeze();
            return brush;
        }
    }
}