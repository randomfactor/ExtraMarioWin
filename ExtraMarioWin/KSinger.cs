using System;

namespace ExtraMarioWin
{
    public class KSinger
    {
        // Backing fields (originally required as fields)
        public Guid id;
        public string? stageName;

        // CLR properties for WPF binding
        public Guid Id
        {
            get => id;
            set => id = value;
        }
        public string? StageName
        {
            get => stageName;
            set => stageName = value;
        }

        public KSinger() { }
        public KSinger(Guid id, string? stageName)
        {
            this.id = id;
            this.stageName = stageName;
        }
    }
}