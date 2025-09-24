using System;
using System.ComponentModel;
using System.Windows.Media;

namespace ExtraMarioWin
{
    public class KSinger : INotifyPropertyChanged
    {
        // Backing fields (originally required as fields)
        public Guid id;
        public string? stageName;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // CLR properties for WPF binding
        public Guid Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    _stageBrush = null; // reset cached brush if id changes
                    OnPropertyChanged(nameof(Id));
                    OnPropertyChanged(nameof(StageBrush));
                }
            }
        }
        public string? StageName
        {
            get => stageName;
            set
            {
                if (stageName != value)
                {
                    stageName = value;
                    OnPropertyChanged(nameof(StageName));
                }
            }
        }

        // UI helper: deterministic pastel color per singer
        private SolidColorBrush? _stageBrush;
        public SolidColorBrush StageBrush => _stageBrush ??= GetPastelBrushFromGuid(Id);

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
            FromHex("#FFFFE4E1"), // misty rose
            FromHex("#FFFFF0F5"), // lavender blush
            FromHex("#FFF0FFF0"), // honeydew
            FromHex("#FFF0FFFF"), // azure
            FromHex("#FFFFFAF0"), // floral white
            FromHex("#FFFFF8DC"), // cornsilk
            FromHex("#FFFAFAD2"), // light goldenrod yellow
            FromHex("#FFE6E6FA"), // lavender (pale)
            FromHex("#FFE0FFFF"), // light cyan
            FromHex("#FFFFF5EE"), // seashell
            FromHex("#FFF5F5DC"), // beige
            FromHex("#FFFAF0E6"), // linen
            FromHex("#FFFDF5E6"), // old lace
            FromHex("#FFFFEBCD"), // blanched almond
            FromHex("#FFFFEFD5"), // papaya whip
            FromHex("#FFFFDAB9"), // peach puff
            FromHex("#FFFFDEAD"), // navajo white
            FromHex("#FFE6FFE6"), // pale mint
            FromHex("#FFD5EAF5"), // light periwinkle
            FromHex("#FFF2D4D0"), // light salmon
            FromHex("#FFD4EDDA"), // light jade
            FromHex("#FFFFD3B6"), // light apricot
            FromHex("#FFE1BEE7"), // light lavender
            FromHex("#FFE8CBAF"), // light tan
            FromHex("#FFEAD1DC"), // light thistle
            FromHex("#FFFFE6CC"), // light cream
            FromHex("#FFF0F8FF"), // light icy blue (AliceBlue)
            FromHex("#FFDFD3FF"), // pale lilac
            FromHex("#FFFFFFD1"), // light butter
            FromHex("#FFD1E8FF"), // light ice
            FromHex("#FFE8D8B0"), // light khaki
            FromHex("#FFF2A3B8"), // light rose
            FromHex("#FFD3C6EA"), // light violet
            FromHex("#FFD1F2EB"), // light minty
            FromHex("#FFE3FFE4"), // very pale green
            FromHex("#FFFFD8E6"), // very pale pink
        };

        private static SolidColorBrush GetPastelBrushFromGuid(Guid guid)
        {
            var bytes = guid.ToByteArray();
            int sum = 0;
            for (int i = 0; i < bytes.Length; i++) sum += bytes[i];
            var index = Math.Abs(sum) % Palette.Length;
            return Palette[index];
        }

        private static SolidColorBrush FromHex(string hex)
        {
            var brush = (SolidColorBrush)new BrushConverter().ConvertFrom(hex)!;
            if (brush.CanFreeze) brush.Freeze();
            return brush;
        }

        public KSinger() { }
        public KSinger(Guid id, string? stageName)
        {
            this.id = id;
            this.stageName = stageName;
        }
    }
}