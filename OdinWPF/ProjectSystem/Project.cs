using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OdinWPF.ProjectSystem {
    public class Project {
        public string name;
        public List<Step> steps = new List<Step>();

        public Project(string name) {
            this.name = name;
        }

        public Step addStep(Step s) {
            steps.Add(s);
            s.proj = this;
            return s;
        }

        public UIElement makeDisplay() {
            StackPanel sp = new StackPanel();
            return sp;
        }
    }
}
