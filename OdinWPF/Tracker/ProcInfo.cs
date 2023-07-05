using OdinWPF.FileSys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OdinWPF.Tracker {
    public class ProcInfo {
        public string name;
        public Color color;
        public List<string> tags = new List<string>();
        public int id;

        public ProcInfo(Line l) {
            load(l);
        }
        public ProcInfo(int id, Process p) {
            this.id = id;
            name = p.ProcessName;
            BitmapSource ims = (BitmapSource)WinConnector.GetIcon(p.MainModule.FileName);
            //TODO!!!
            color = Color.FromRgb((byte)App.r.Next(256), (byte)App.r.Next(256), (byte)App.r.Next(256));
        }
        public ProcInfo(int id, string name) {
            this.id = id;
            this.name = name;
            color = Color.FromRgb((byte)App.r.Next(256), (byte)App.r.Next(256), (byte)App.r.Next(256));
        }
        public void load(Line l) {
            id = int.Parse(l.st);
            name = l.get("name").value();
            color = (Color)ColorConverter.ConvertFromString(l.get("color").value());
            foreach (Line tl in l.getLineList("tags")) {
                tags.Add(tl.st);
            }
        }
        public void save(Line l) {
            l.add("name:"+name);
            l.add("color:" + new ColorConverter().ConvertToString(color));
            if (tags.Count != 0) {
                Line tl = l.add("tags");
                foreach (string st in tags) {
                    tl.add(st);
                }
            }
        }
    }
}
