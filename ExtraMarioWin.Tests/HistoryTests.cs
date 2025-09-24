using System;
using System.IO;
using ExtraMarioWin;
using ExtraMarioWin.History;
using Xunit;

namespace ExtraMarioWin.Tests
{
    public class HistoryTests
    {
        [Fact]
        public void FilePerformerHistory_WritesToProvidedDirectory()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "ExtraMarioWin_Test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var history = new FilePerformerHistory(tempDir);
                var singer = new KSinger(Guid.NewGuid(), "Tester");
                history.SaveSinger(singer);

                var expected = Path.Combine(tempDir, $"history_{DateTime.Now:yyyy-MM-dd}.csv");
                Assert.True(File.Exists(expected));
                var content = File.ReadAllText(expected);
                Assert.Contains("\"Tester\"", content);
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
            }
        }

        [Fact]
        public void FilePerformerRosterStorage_SaveAndRestore_RoundTrips()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "ExtraMarioWin_Roster_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var storage = new FilePerformerRosterStorage(tempDir);
                var s1 = new KSinger(Guid.NewGuid(), "Alpha");
                var s2 = new KSinger(Guid.NewGuid(), "Beta");
                storage.SaveRoster(new[] { s1, s2 });

                var restored = storage.RestoreRoster();
                Assert.Equal(2, restored.Count);
                Assert.Equal(s1.id, restored[0].id);
                Assert.Equal(s1.stageName, restored[0].stageName);
                Assert.Equal(s2.id, restored[1].id);
                Assert.Equal(s2.stageName, restored[1].stageName);
            }
            finally
            {
                try { Directory.Delete(tempDir, true); } catch { }
            }
        }
    }
}