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
    /// <summary>
    /// Логика взаимодействия для StepFullControl.xaml
    /// </summary>
    public partial class StepFullControl : UserControl {
        Step s;
        public StepFullControl() {
            InitializeComponent();
            load(new Step("Name of the step!", "A very long lore of this step which shall be extended if it needs!", App.r.Next()%2==0));
        }
        public StepFullControl(Step s) {
            InitializeComponent();
            load(s);
        }
        private void load(Step s) {
            this.s = s;
            name.Text = s.name;
            lore.Text = s.lore;
            name.Background = s.simpleBg();
        }

        private void click(object sender, MouseButtonEventArgs e) {
            StepWindow win = new StepWindow(s);
            win.Show();
        }
    }
}
