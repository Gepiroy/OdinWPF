using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OdinWPF.FileSys {
    public class StructFile {
        string path;

        public Line coreLine = new Line("core");
        public StructFile(string path, bool createIfNotExists) {
            this.path = path;
            if (File.Exists(path)) {
                foreach (string st in File.ReadAllLines(path)) {
                    if (st.Length == 0 || st.StartsWith("//")) continue;
                    int level = 0, sl=0;
                    for (int i=0;i<st.Length;i+=2) {
                        if (st[i] == ' ') level++;
                        else break;
                    }
                    sl = level;
                    Line line;
                    if (level == 0) {
                        coreLine.add(new Line(st));
                    } else {
                        line = coreLine.lastSub();
                        while (level-- > 0) {
                            if (level == 0) {
                                line.add(new Line(st.Substring(sl * 2)));
                            }
                            line = line.lastSub();
                        }
                    }

                }
            } else if (createIfNotExists) File.WriteAllText(path, "");
        }
        public void clear() {
            coreLine.subs.Clear();
        }
        public void save() {
            List<string> toWrite = new List<string>();
            foreach (Line l in coreLine.subs) {
                l.toSave(0, ref toWrite);
            }
            File.WriteAllLines(path, toWrite);
        }

        public Line get(string st) {
            // "Пупы.Бойня.Час пилить"
            string[] parsed = st.Split('.');
            Line line = coreLine;
            foreach (string s in parsed) {
                foreach (Line l in line.subs) {
                    if (l.st.Equals(s)) {
                        line = l;
                        break;
                    }
                }
            }
            return line;
        }
    }
}
