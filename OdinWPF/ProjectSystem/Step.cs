using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OdinWPF.ProjectSystem {
    public class Step {
        public string name;
        public string lore;
        public bool difficult=false;
        public bool complete = false;
        public Project proj;

        public Step(string name) {
            this.name = name;
        }

        public Step(string name, string lore, bool difficult) {
            this.name = name;
            this.lore = lore;
            this.difficult = difficult;
        }

        private static SolidColorBrush completeBg = new SolidColorBrush(Color.FromRgb(0,255,0));
        private static SolidColorBrush difficultBg = new SolidColorBrush(Color.FromRgb(255, 192, 0));
        private static SolidColorBrush defaultBg = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        public SolidColorBrush simpleBg() {
            if (complete) return completeBg;
            if (difficult) return difficultBg;
            return defaultBg;
        }
    }
}
