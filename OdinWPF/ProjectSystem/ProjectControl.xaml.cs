using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OdinWPF.ProjectSystem {
    public partial class ProjectControl : UserControl {
        public Project p;
        public ProjectControl() {
            InitializeComponent();
        }
        public ProjectControl(Project p) {
            InitializeComponent();
            this.p = p;
            name.Text = p.name;
            subs.Children.Clear();
            foreach (Step s in p.steps) {
                subs.Children.Add(new StepControl(s));
            }
        }

        private void click(object sender, MouseButtonEventArgs e) {
            ProjectWindow win = new ProjectWindow(p);
            win.Show();
        }
    }
}
