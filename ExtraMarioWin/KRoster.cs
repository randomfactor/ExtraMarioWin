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

        public bool Remove(KSinger singer)
        {
            if (singer == null) return false;
            return _singers.Remove(singer);
        }

        public bool Bump()
        {
            if (_singers.Count < 2) return false;
            (_singers[0], _singers[1]) = (_singers[1], _singers[0]);
            return true;
        }

        public bool NextSinger()
        {
            if (_singers.Count == 0) return false;
            if (_singers.Count == 1) return true; // nothing changes but considered successful
            var first = _singers[0];
            _singers.RemoveAt(0);
            _singers.Add(first);
            return true;
        }

        // Backwards-compatible name kept for existing code/tests
        public bool Next() => NextSinger();

        public bool Move(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= _singers.Count) return false;
            if (newIndex < 0 || newIndex >= _singers.Count) return false;
            if (oldIndex == newIndex) return false;
            var item = _singers[oldIndex];
            _singers.RemoveAt(oldIndex);
            _singers.Insert(newIndex, item);
            return true;
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