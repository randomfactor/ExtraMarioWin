using System;
using System.IO;
using System.Text;

namespace ExtraMarioWin.History
{
    public class FilePerformerHistory : IPerformerHistory
    {
        private readonly string? _baseDir;
        public FilePerformerHistory() { }
        public FilePerformerHistory(string baseDirectory)
        {
            _baseDir = baseDirectory;
        }

        private string GetAppDir()
        {
            if (!string.IsNullOrEmpty(_baseDir))
            {
                Directory.CreateDirectory(_baseDir);
                return _baseDir!;
            }
            var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Extra Mario");
            Directory.CreateDirectory(appDir);
            return appDir;
        }

        public void SaveSinger(KSinger singer)
        {
            try
            {
                var appDir = GetAppDir();
                var filePath = Path.Combine(appDir, $"history_{DateTime.Now:yyyy-MM-dd}.csv");

                string name = singer.StageName ?? string.Empty;
                string csvName = "\"" + name.Replace("\"", "\"\"") + "\"";
                // ISO 8601 without fractional seconds, with local offset
                string isoTime = DateTimeOffset.Now.ToString("yyyy-MM-dd'T'HH:mm:sszzz");
                string csvTime = "\"" + isoTime + "\"";
                string line = $"{csvName},{csvTime}{Environment.NewLine}";

                using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var writer = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                writer.Write(line);
                writer.Flush();
                fs.Flush(true);
            }
            catch
            {
                // swallow by design
            }
        }
    }
}