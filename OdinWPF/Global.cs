using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF {
    public static class Global {
        public static Random r = new Random();//Дерьмово, что такое двоевластие...
        public static long getNowSec() {
            return DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
        }
        public static int randRange(int min, int max) {
            return min + r.Next(max - min + 1);
        }
    }
}
