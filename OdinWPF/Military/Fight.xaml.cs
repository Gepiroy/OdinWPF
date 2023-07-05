using OdinWPF.Military;
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
using System.Windows.Threading;

namespace OdinWPF {
    /// <summary>
    /// Логика взаимодействия для Fight.xaml
    /// </summary>
    public partial class Fight : Window {
        static Random r = App.r;
        DispatcherTimer timer;
        Army[] armies;
        List<ArmyGuy> guys = new List<ArmyGuy>();
        public int loses;
        public bool victory;
        private int timeToEnd = 0;
        private int fightOption;
        public Fight(params Army[] armies) {
            InitializeComponent();
            this.armies = armies;
            armies[0].oppositeIndex = 1;
            armies[0].direction = 1;
            armies[1].oppositeIndex = 0;
            armies[1].direction = -1;
            foreach (Army army in armies) {
                guys.AddRange(army.guys);
                foreach (ArmyGuy g in army.guys) {
                    canv.Children.Add(g.display);
                    Canvas.SetTop(g.display, r.Next(450));
                }
            }
            foreach (ArmyGuy g in armies[0].guys) {
                g.display.Fill = new SolidColorBrush(Color.FromRgb(0, 100, 200));
                Canvas.SetLeft(g.display, r.Next(300));
            }
            foreach (ArmyGuy g in armies[1].guys) {
                g.display.Fill = new SolidColorBrush(Color.FromRgb(200, 0, 0));
                Canvas.SetLeft(g.display, r.Next(300) + 500);
            }
            army1Counter.Content = armies[0].info();
            army2Counter.Content = armies[1].info();
            timer = new DispatcherTimer {
                Interval = TimeSpan.FromMilliseconds(1000/30)
            };
            timer.Tick += tick;
            timer.Start();
        }

        protected override void OnContentRendered(EventArgs e) {
            base.OnContentRendered(e);
            
        }
        bool selected = false;
        void tick(object sender, EventArgs e) {
            if (!selected) {
                PreBattle pre = new PreBattle();
                pre.ShowDialog();
                if (pre.selectedOption == 1) {//run
                    armies[0].direction = -3;
                    timeToEnd = 60;
                } else if (pre.selectedOption == 3) {//rage
                    armies[0].direction = 2;
                }
                fightOption = pre.selectedOption;
                selected = true;
            }
            if (fightOption==1) {
                if (--timeToEnd==0) {
                    timer.Stop();
                    countLoses(0.05, 0.15);
                    victory = false;
                    MessageBox.Show($"Вы сбежали.\nВ ходе бегства вы потеряли {loses} чел.");
                    Close();
                    return;
                }
            }
            guys=guys.OrderBy(x => r.Next()).ToList();//Стреляют они явно не по очереди.
            try {
                foreach (ArmyGuy g in guys.ToArray()) {
                    if (r.NextDouble() < 0.5 && g.army.mode == 1) Canvas.SetLeft(g.display, Canvas.GetLeft(g.display) + g.army.direction);
                    if (fightOption == 1 && g.army.oppositeIndex == 1) continue;
                    if (g.alive && r.NextDouble() < 0.01*(fightOption==3&&g.army.oppositeIndex==1?1.5:1)) {
                        ArmyGuy target = armies[g.army.oppositeIndex].guys[r.Next(armies[g.army.oppositeIndex].guys.Count)];
                        double killCoef=1.0;
                        if (target.army.mode == 0 || fightOption == 1 && target.army == armies[0]) killCoef *= 0.7;

                        if (r.NextDouble() < (0.5+g.army.spirit*0.5) * killCoef) {//Попадёт ли
                            killed(target);
                        }
                    }
                }
            } catch {
                timer.Stop();
                army1Counter.Content = armies[0].info();
                army2Counter.Content = armies[1].info();
                if(fightOption==3) countLoses(0.4, 0.6);
                else countLoses(0.1,0.15);
                victory = armies[0].guys.Count != 0;
                MessageBox.Show($"Битва была {(victory?"выиграна":"проиграна")}.\nВ ходе битвы вы потеряли {loses} чел.");
                Close();
            }
            army1Counter.Content = armies[0].info();
            army2Counter.Content = armies[1].info();
        }

        void countLoses(double min, double max) {
            loses = (int)((armies[0].amStart - armies[0].guys.Count) * (r.NextDouble() * (max-min) + min));
        }

        void killed(ArmyGuy g) {
            guys.Remove(g);
            g.army.guys.Remove(g);
            g.alive = false;
            canv.Children.Remove(g.display);
        }
    }
}
