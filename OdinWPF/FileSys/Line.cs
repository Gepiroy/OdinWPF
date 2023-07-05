using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF.FileSys {
    public class Line {
        public string st;
        public List<Line> subs = new List<Line>();
        public Line parent;
        public Line(string st) {
            this.st = st;
        }
        public Line add(Line l) {
            subs.Add(l);
            l.parent = this;
            return l;
        }
        public Line add(string st) {
            return add(new Line(st));
        }
        public Line lastSub() {
            return subs.Last();
        }
        public Line randSub() {
            return subs[App.r.Next(subs.Count)];
        }

        public string value() {//error-checks not exist.
            int ddi = 1;
            while (ddi<st.Length) {
                if (st[ddi] == ':') break;
                ddi++;
            }
            return st.Substring(ddi+1);
        }

        public Line get(string st) {
            foreach (Line l in subs) {
                if (l.st.StartsWith(st)) return l;
            }
            return null;
        }

        public List<Line> getLineList(string st) {
            Line from = get(st);
            if (from == null) return new List<Line>();
            return from.subs;
        }

        public void toSave(int lvl, ref List<string> sts) {
            string ss = "";
            for (int i = 0; i < lvl; i++) ss += "  ";
            sts.Add(ss+st);
            foreach (Line l in subs) {
                l.toSave(lvl+1, ref sts);
            }
        }
        public Line totallyRandomLine() {
            if (subs.Count == 0) return this;
            return randSub().totallyRandomLine();
        }
        public string totallyFullName(string ret, string sep) {
            ret = st + ret;
            if (parent.parent != null) return parent.totallyFullName(sep+ret, sep);
            return ret;
        }
        public override string ToString() {
            return st;
        }
    }
}
