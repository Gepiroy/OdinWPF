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

namespace OdinWPF.Military {
    /// <summary>
    /// Логика взаимодействия для PreBattle.xaml
    /// </summary>
    public partial class PreBattle : Window {
        public int selectedOption = 1;
        public PreBattle() {
            InitializeComponent();
        }

        private void clicked(object sender, RoutedEventArgs e) {
            if (sender == b2) selectedOption = 2;
            if (sender == b3) selectedOption = 3;
            Close();
        }
    }
}
