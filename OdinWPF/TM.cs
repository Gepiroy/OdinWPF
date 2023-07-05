using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF {
    public static class TM {
        public static long today() {
            return DateTime.Now.Ticks/TimeSpan.TicksPerDay;
        }
        public static long tosec() {
            return DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
        }
    }
}
