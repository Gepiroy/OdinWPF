using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF {
    public class Army {
        public int oppositeIndex = 0;
        public int direction;
        public int amStart;
        public List<ArmyGuy> guys = new List<ArmyGuy>();
        public double spirit;
        public int mode=1; //0=def,1=rush.
        
        public Army(int am, double spirit) {
            amStart = am;
            for (int i=0;i<am;i++) {
                guys.Add(new ArmyGuy(this));
            }
            this.spirit = spirit;
        }

        public Army setMode(int mode) {
            this.mode = mode;
            return this;
        }

        public string info() {
            return $"{guys.Count}/{amStart} ({guys.Count*100/amStart}%)";
        }
    }
}
