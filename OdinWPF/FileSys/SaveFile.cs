using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF {
    public class SaveFile {
        string path;
        Dictionary<string, string> data = new Dictionary<string, string>();
        public SaveFile(string path) {
            this.path = path;
            if (File.Exists(path)) {
                foreach (string st in File.ReadAllLines(path)) {
                    if (st.Length < 3) continue;
                    string[] kv = st.Split(':');
                    data.Add(kv[0], kv[1]);
                }
            }
        }
        public void set(string key, string value) {
            if (value == null) data.Remove(key);
            else data[key] = value;
        }
        public string get(string key) {
            return data.ContainsKey(key)?data[key]:null;
        }
        public bool exists() {
            return File.Exists(path);
        }
        public void save() {
            List<string> lines = new List<string>();
            foreach(string key in data.Keys) {
                lines.Add(key + ":" + data[key]);
            }
            File.WriteAllLines(path, lines);
        }
    }
}
