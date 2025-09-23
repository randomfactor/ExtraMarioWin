using System.Collections.Generic;

namespace ExtraMarioWin
{
    public class KRoster
    {
        private readonly List<KSinger> _singers = new();

        public IReadOnlyList<KSinger> Singers => _singers;

        public void Add(KSinger singer)
        {
            if (singer == null) return; // simple guard
            _singers.Add(singer);
        }

        public int Count()
        {
            return _singers.Count;
        }

        public KSinger? Get(int index)
        {
            if (index < 0 || index >= _singers.Count) return null;
            return _singers[index];
        }
    }
}