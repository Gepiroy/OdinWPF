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

namespace OdinWPF {
    /// <summary>
    /// Логика взаимодействия для MoneyDialog.xaml
    /// </summary>
    public partial class MoneyDialog : Window {
        public MoneyDialog(string label1, string label2) {
            InitializeComponent();
            this.label1.Text = label1;
            this.label2.Text = label2;
        }

        private void bclick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
