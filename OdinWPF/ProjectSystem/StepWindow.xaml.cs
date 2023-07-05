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
    /// Логика взаимодействия для StepWindow.xaml
    /// </summary>
    public partial class StepWindow : Window {
        Step s;
        public StepWindow() {
            InitializeComponent();
        }
        public StepWindow(Step s) {
            InitializeComponent();
            this.s = s;
            update(s);
        }
        void update(Step s) {
            name.Text = s.name;
            lore.Text = s.lore;
            difficult.IsChecked = s.difficult;
            done.IsChecked = s.complete;
        }

        private void saveClick(object sender, RoutedEventArgs e) {
            s.name = name.Text;
            s.lore = lore.Text;
            s.difficult = difficult.IsChecked??false;
            s.complete = done.IsChecked??false;
        }

        private void doneClick(object sender, RoutedEventArgs e) {
            App.logMessage($"Выполнен{(s.difficult?" тяжёлый":"")} шаг \"{s.name}\" проекта \"{s.proj.name}\"!");

            App.addMoney(s.difficult?Global.randRange(20,45): Global.randRange(10, 20), "Награда за шаг");
            App.milBase.addSpirit(App.r.NextDouble() * (s.difficult ? 20 : 45) * 0.0065);
            if (s.difficult) {
                App.addCard();
                if(App.r.NextDouble()<0.5)App.addCard();
            }else if (App.r.NextDouble() < 0.7) {
                App.addCard();
            }
            done.IsChecked = true;
            saveClick(null, null);
            Close();
        }
    }
}
