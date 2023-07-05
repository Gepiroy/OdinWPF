using OdinWPF.FileSys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF.Tracker {
    public class TrackedDay {
        public List<TrackedThing> log = new List<TrackedThing>();
        public List<long> parsedWorkTime = new List<long>();
        private string filepath;

        public TrackedDay(string filepath) {
            this.filepath = filepath;
            StructFile f = new StructFile(filepath, true);
            foreach (Line l in f.coreLine.subs) {
                add(new TrackedThing(l));
            }
        }
        public void addBreak() {
            TrackedThing t=null;
            if (log.Count != 0) {
                t = log[log.Count - 1];
            }
            if (t==null||t.pinf.id!=-1) t = new TrackedThing(ProcCore.procList[-1]);
            t.secStart = Global.getNowSec();
            log.Add(t);
        }
        public void add(TrackedThing t) {
            if (log.Count != 0) {
                TrackedThing tlast = log[log.Count - 1];
                if (tlast.pinf.id == -1 && Global.getNowSec()-tlast.secStart<5) {
                    log.RemoveAt(log.Count - 1);
                }
            }
            log.Add(t);
        }
        public void save() {
            StructFile f = new StructFile(filepath, false);
            f.clear();
            addBreak();
            foreach (TrackedThing t in log) {
                Line l = f.coreLine.add(t.pinf.id+"");
                if (t.subName != null) l.add("s:"+t.subName);
                l.add("t:" + t.secStart);
            }
            f.save();
        }
        bool isWorkUnit(TrackedThing t) {
            if (t.subName!=null&&t.subName.Contains("Melancholy")) return false;
            if (t.pinf.tags.Contains("work")) return true;

            return false;
        }
        public void reparseWorkTime() {
            parsedWorkTime.Clear();
            int i = 0;
            foreach (TrackedThing t in log) {
                if (isWorkUnit(t)) {
                    long endT = TM.tosec();
                    if (log.Count >i+1) {
                        endT = log[i + 1].secStart;
                    }
                    addParsed(t.secStart, endT);
                }
                i++;
            }
        }
        void addParsed(long start, long end) {
            if (parsedWorkTime.Count>0) {
                long lastWorkOut = parsedWorkTime[parsedWorkTime.Count - 1];
                if (start - lastWorkOut <= 600) {
                    parsedWorkTime[parsedWorkTime.Count - 1]=end;
                    return;//Пробелы до 10 минут между рабочими прогами считаются работой. Мало ли надо было что найти в файлах или инете - короле прокрастинации?
                }
            }
            parsedWorkTime.Add(start);
            parsedWorkTime.Add(end);
        }
        public int workedSecondsSince(long from) {
            long ret = 0;
            for (int i=0;i<parsedWorkTime.Count;i+=2) {
                ret += Math.Max(0, parsedWorkTime[i + 1] - Math.Max(from, parsedWorkTime[i]));
            }
            return (int)ret;
        }
    }
}
