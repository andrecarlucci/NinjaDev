using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NinjaDev {
    public class SnippetManager {

        private Dictionary<int, string> _sourceDic = new Dictionary<int, string>();
        private const string DIRNAME = "snippets";
        public void LoadAll() {
            Directory.CreateDirectory(DIRNAME);
            foreach (var file in Directory.EnumerateFiles(DIRNAME)) {
                string digits = new String(file.Where(Char.IsDigit).ToArray());
                if (digits.Length != 1) {
                    continue;
                }
                var id = Int32.Parse(digits);
                var snippet = File.ReadAllText(file);
                _sourceDic[id] = snippet;
            }
        }

        public string GetSnippet(int id) {
            if (_sourceDic.ContainsKey(id)) {
                return _sourceDic[id];
            }
            return "";
        }
    }
}