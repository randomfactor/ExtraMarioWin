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