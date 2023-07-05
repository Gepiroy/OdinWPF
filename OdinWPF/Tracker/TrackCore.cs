using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF.Tracker {
    public static class TrackCore {
        static string screenPath;
        public static string trackPath;
        public static TrackedDay today;
        public static void init() {
            screenPath = App.folderpath + @"screenshots\";
            trackPath = App.folderpath + @"track\";
            if (!Directory.Exists(screenPath)) Directory.CreateDirectory(screenPath);
            if (!Directory.Exists(trackPath)) Directory.CreateDirectory(trackPath);
            ProcCore.init();
            today = new TrackedDay(trackPath+$"{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
        }
        public static void screenshot(int l) {
            Bitmap bm = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Bitmap toSave = new Bitmap(bm.Width / l, bm.Height / l);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(0, 0, 0, 0, bm.Size);
            for (int x=0;x<toSave.Width;x++) {
                for (int y = 0; y < toSave.Height; y++) {
                    toSave.SetPixel(x,y, bm.GetPixel(x*l,y*l));
                }
            }
            toSave.Save(screenPath + $"{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.jpg", ImageFormat.Jpeg);
        }
        private static long lastScreen = 0;
        private static int trackUpdate = 1;
        private static string lastTitle = "";
        private static Dictionary<string, long> alerted = new Dictionary<string, long>();
        public static void sec() {
            long now = Global.getNowSec();
            if (lastScreen + 300 - now < 0) {
                screenshot(2);
                lastScreen = now;
            }

            if (--trackUpdate == 0) {
                trackUpdate = 1;//TODO 10!
                if (today.log.Count > 0) lastTitle = today.log[today.log.Count-1].subName;
                string aw = WinConnector.GetActiveWindowTitle();
                if (!(lastTitle ?? "").Equals(aw)) {
                    lawAlert(aw);
                    today.add(new TrackedThing(ProcCore.findOrCreate(WinConnector.GetForegroundProcessName()), aw, Global.getNowSec()));
                }
            }
        }
        static void lawAlert(string aw) {
            string detected = null;
            foreach (Law l in App.activeLaws) {
                foreach (string st in l.alertHooks) {
                    if ((aw ?? "").Contains(st)) {
                        if (alerted.ContainsKey(st)) {
                            if (DateTime.Now.Ticks - alerted[st] < TimeSpan.TicksPerMinute * 10) {
                                continue;
                            }
                        }
                        detected = st;
                        var toast = new ToastContentBuilder();
                        toast.AddText("Напоминаю про закон \"" + l.name + "\"!");
                        toast.Show();
                    }
                }
            }
            if (detected != null) {
                alerted[detected] = DateTime.Now.Ticks;
            }
        }

        public static void save() {
            today.save();
            ProcCore.save();
        }
    }
}
