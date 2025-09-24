namespace ExtraMarioWin.History
{
    public interface IPerformerHistory
    {
        // Persist a record that this singer just finished
        void SaveSinger(KSinger singer);
    }
}