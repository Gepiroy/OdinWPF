using OdinWPF.FileSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF.Tracker {
    public static class ProcCore {
        public static Dictionary<int, ProcInfo> procList = new Dictionary<int, ProcInfo>();

        public static void init() {
            StructFile f = new StructFile(App.folderpath+"procList.txt", true);
            foreach (Line l in f.coreLine.subs) {
                procList[int.Parse(l.st)] = new ProcInfo(l);
            }
            if (!procList.ContainsKey(-1)) procList[-1] = new ProcInfo(-1, "$break");
            if (!procList.ContainsKey(-2)) procList[-2] = new ProcInfo(-2, "$offInf");
        }

        public static ProcInfo findOrCreate(string st) {
            foreach (ProcInfo pinf in procList.Values) {
                if (pinf.name.Equals(st)) return pinf;
            }
            ProcInfo ret = new ProcInfo(procList.Count, st);
            procList[procList.Count] = ret;
            return ret;
        }

        public static void save() {
            StructFile f = new StructFile(App.folderpath + "procList.txt", false);
            f.clear();
            foreach (ProcInfo pinf in procList.Values) {
                pinf.save(f.coreLine.add(new Line(pinf.id+"")));
            }
            f.save();
        }
    }
}
