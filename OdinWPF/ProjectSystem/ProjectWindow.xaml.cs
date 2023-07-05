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
using System.Windows.Shapes;

namespace OdinWPF.ProjectSystem {
    /// <summary>
    /// Логика взаимодействия для ProjectWindow.xaml
    /// </summary>
    public partial class ProjectWindow : Window {
        Project p;
        public ProjectWindow() {
            InitializeComponent();
        }
        public ProjectWindow(Project p) {
            InitializeComponent();
            this.p = p;
            update(p);
        }
        public void update(Project p) {
            name.Text = p.name;
            int i = 0;
            foreach (Step s in p.steps) {
                Elements.insertToGrid(grid, new StepFullControl(s), i % 2, i / 2);
                i++;
            }
        }

        private void saveClick(object sender, RoutedEventArgs e) {
            p.name = name.Text;
        }

        private void newStepClick(object sender, RoutedEventArgs e) {
            Step s = new Step("Имя шага", "Описание шага", false);
            p.addStep(s);
            StepWindow win = new StepWindow(s);
            win.ShowDialog();
            update(p);
        }
    }
}
