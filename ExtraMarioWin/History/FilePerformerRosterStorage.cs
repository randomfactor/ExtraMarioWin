using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExtraMarioWin.History
{
    public class FilePerformerRosterStorage : IPerformerRosterStorage
    {
        private readonly string? _baseDir;
        public FilePerformerRosterStorage() { }
        public FilePerformerRosterStorage(string baseDirectory)
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

        private string GetRosterPath() => Path.Combine(GetAppDir(), $"roster_{DateTime.Now:yyyy-MM-dd}.csv");

        public void SaveRoster(IReadOnlyList<KSinger> singers)
        {
            var path = GetRosterPath();
            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            foreach (var s in singers)
            {
                var id = s.Id.ToString();
                var name = s.StageName ?? string.Empty;
                var csvName = "\"" + name.Replace("\"", "\"\"") + "\"";
                writer.WriteLine($"{id},{csvName}");
            }
            writer.Flush();
            fs.Flush(true);
        }

        public List<KSinger> RestoreRoster()
        {
            var path = GetRosterPath();
            var result = new List<KSinger>();
            if (!File.Exists(path)) return result;
            try
            {
                foreach (var line in File.ReadAllLines(path))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    // Parse: GUID,"Name"
                    int comma = line.IndexOf(',');
                    if (comma <= 0) continue;
                    var guidStr = line[..comma].Trim();
                    var rest = line[(comma + 1)..].Trim();
                    if (Guid.TryParse(guidStr, out var guid))
                    {
                        string name = rest;
                        if (name.Length >= 2 && name[0] == '"' && name[^1] == '"')
                        {
                            name = name.Substring(1, name.Length - 2).Replace("\"\"", "\"");
                        }
                        result.Add(new KSinger(guid, name));
                    }
                }
            }
            catch
            {
                // ignore bad file, return empty or partial
            }
            return result;
        }
    }
}