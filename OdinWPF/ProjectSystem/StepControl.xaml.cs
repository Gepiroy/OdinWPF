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
    /// Логика взаимодействия для StepControl.xaml
    /// </summary>
    public partial class StepControl : UserControl {
        public StepControl() {
            InitializeComponent();
            Background = new Step("","",App.r.Next()%2==0).simpleBg();
        }
        public StepControl(Step s) {
            InitializeComponent();
            name.Text = s.name;
            Background = s.simpleBg();
        }
    }
}
