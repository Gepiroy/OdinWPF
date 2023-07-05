using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace OdinWPF {
    public class LinerFile {
        string path;
        public List<string> sts = new List<string>();
        public LinerFile(string path, bool createIfNotExists) {
            this.path = path;
            if (File.Exists(path)) {
                foreach(string st in File.ReadAllLines(path)) {
                    if (st.Length == 0||st.StartsWith("//")) continue;
                    sts.Add(st);
                }
            } else if (createIfNotExists) File.WriteAllLines(path, sts);
        }
        public void save() {
            File.WriteAllLines(path, sts);
        }
    }
}
