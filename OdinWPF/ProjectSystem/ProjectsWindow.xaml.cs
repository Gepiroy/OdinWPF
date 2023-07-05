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
    /// Логика взаимодействия для ProjectsWindow.xaml
    /// </summary>
    public partial class ProjectsWindow : Window {
        public ProjectsWindow() {
            InitializeComponent();
            update();
        }

        void update() {
            int i = 0;
            foreach (Project p in Projects.projects) {
                Elements.insertToGrid(grid, new ProjectControl(p), i % 2, i / 2);
                i++;
            }
        }

        private void newProjClick(object sender, RoutedEventArgs e) {
            Project p = new Project("Имя проекта");
            Projects.projects.Add(p);
            ProjectWindow win = new ProjectWindow(p);
            win.ShowDialog();
            update();
        }
    }
}
