using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OdinWPF {
    internal class WinConnector {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static string GetActiveWindowTitle() {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0) {
                return Buff.ToString();
            }
            return null;
        }

        public static Process getForegroundProcess() {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == null) return null;
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            foreach (Process p in Process.GetProcesses()) {
                if (p.Id == pid) {
                    return p;
                }
            }
            return null;
        }
        public static string GetForegroundProcessName() {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == null) return "null";
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            foreach (Process p in Process.GetProcesses()) {
                if (p.Id == pid) {
                    return p.ProcessName;
                }
            }
            return "Unknown";
        }

        public static ImageSource GetIcon(string path) {
            Icon icon = Icon.ExtractAssociatedIcon(path);
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        new Int32Rect(0,0,icon.Width, icon.Height),
                        BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
