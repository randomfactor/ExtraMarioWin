using System.Collections.Generic;

namespace ExtraMarioWin.History
{
    public interface IPerformerRosterStorage
    {
        void SaveRoster(IReadOnlyList<KSinger> singers);
        List<KSinger> RestoreRoster();
    }
}