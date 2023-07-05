using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace OdinWPF {
    public class ArmyGuy {
        public bool alive = true;
        public Army army { get; }
        public Ellipse display;

        public ArmyGuy(Army from) {
            army = from;
            display = new Ellipse {
                Width=5, Height=5
            };
        }
    }
}
