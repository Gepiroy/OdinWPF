using OdinWPF.FileSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF.Tracker {
    public class TrackedThing {
        public ProcInfo pinf;
        public string subName;
        public long secStart;

        public TrackedThing(ProcInfo pinf) {
            this.pinf = pinf;
        }

        public TrackedThing(ProcInfo pinf, string subName, long secStart) {
            this.pinf = pinf;
            this.subName = subName;
            this.secStart = secStart;
        }

        public TrackedThing(Line l) {
            pinf = ProcCore.procList[int.Parse(l.st)];
            if(l.get("s:")!=null)subName = l.get("s:").value();
            secStart = long.Parse(l.get("t:").value());
        }

        public void setSub(string subName) {
            this.subName = subName;
        }

        public void setStart(long secStart) {
            this.secStart = secStart;
        }
    }
}
