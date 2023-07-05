using OdinWPF.Tracker;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace OdinWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window{
        static TextBlock devModeAlert = new TextBlock {
                Text = "devMode", Background = new SolidColorBrush(Color.FromRgb(255,255,0))
            };
        public static MainWindow instance;
        public MainWindow(){
            InitializeComponent();
            instance = this;
            if (App.devMode) {
                pan.Children.Insert(0, devModeAlert);
            } else {
                pan.Children.Remove(devModeAlert);
            }
            moneyBox.Text = App.money+"Ш";
            App.moneyBox = moneyBox;
            ScrollViewer.SetCanContentScroll(lawList, false);
            ScrollViewer.SetCanContentScroll(cardList, false);
            updateData();
            StackPanel qpan = new StackPanel();
            qpan.Orientation = Orientation.Horizontal;
            foreach (QuestSlot q in App.quests) qpan.Children.Add(q.genComp());
            pan.Children.Add(qpan);
            bankPanel.Children.Insert(1,Elements.makeButton(new Button {Content="+", FontSize=16, Width=20, Background=new SolidColorBrush(Color.FromRgb(0,200,0))}, (s, e) => {
                MoneyDialog d = new MoneyDialog("Сколько шекелей добавить", "Комментарий");
                d.ShowDialog();
                if (d.tb1.Text.Length > 0) {
                    App.addMoney(int.Parse(d.tb1.Text), d.tb2.Text);
                }
                }));
            bankPanel.Children.Insert(2,Elements.makeButton(new Button { Content = "-", FontSize = 16, Width=20, Background = new SolidColorBrush(Color.FromRgb(200, 0, 0)) }, (s, e) => {
                MoneyDialog d = new MoneyDialog("Сколько шекелей убавить", "Комментарий");
                d.ShowDialog();
                if (d.tb1.Text.Length > 0) {
                    App.addMoney(-int.Parse(d.tb1.Text), d.tb2.Text);
                }
            }));
            pan.Children.Add(App.milBase.militaryGrid());
            if (App.devMode) {
                pan.Children.Add(Elements.makeButton(new Button { Content = "Следующий день", Background = new SolidColorBrush(Color.FromRgb(0, 50, 200)) }, (s, e) => { App.day(); }));
            }
            pan.Children.Add(App.logBox);
            pan.Children.Add(Elements.makeButton(new Button { Content = "projects" }, (s, e) => {
                ProjectSystem.Projects.openWindow();
            }));
            pan.Children.Add(Elements.makeButton(new Button { Content="track window"}, (s, e) => {
                TrackWindow tw = new TrackWindow(TrackCore.today);
                tw.Show();
            }));
            pan.Children.Add(Elements.makeButton(new Button { Content = "ДЕНЕГ ДАЙ ЗА РАБОТУ!" }, (s, e) => {
                long lmt = 0;
                if(App.globalSaveFile.get("lastMoneyTaking")!=null)lmt = long.Parse(App.globalSaveFile.get("lastMoneyTaking"));
                int secondsOfWork = 0;
                foreach (string st in Directory.GetFiles(TrackCore.trackPath)) {
                    TrackedDay d = new TrackedDay(st);
                    d.reparseWorkTime();
                    secondsOfWork += d.workedSecondsSince(lmt);
                }
                App.addMoney(secondsOfWork / 300, "Оплата рабочего времени");
                App.globalSaveFile.set("lastMoneyTaking", (DateTime.Now.Ticks/TimeSpan.TicksPerSecond).ToString());
                App.globalSaveFile.save();
            }));
        }
        public void updateData() {
            lawList.Items.Clear();
            foreach (Law l in App.activeLaws) {
                ListViewItem litem = l.genListView();
                litem.ContextMenu = new ContextMenu();
                MenuItem militaryItem = new MenuItem { Header = "Попытаться отменить насилием" };
                militaryItem.Click += (s, e) => {
                    Fight fight = App.milBase.startFight(1, 0, 150+App.r.Next(351));
                    if (fight.victory) {
                        App.logMessage($"Насильственным путём был отменён закон \"{l.name}\"!");
                        App.changeLaw(l);
                    } else {
                        App.logMessage($"Попытка насилием отменить закон \"{l.name}\" провалилась.");
                    }
                    App.milBase.endFight();
                };
                litem.ContextMenu.Items.Add(militaryItem);
                lawList.Items.Add(litem);
            }
            cardList.Items.Clear();
            foreach (Card c in App.activeCards) {
                ListViewItem litem = c.genListView();
                litem.ContextMenu = new ContextMenu();
                MenuItem extItem = new MenuItem { Header = $"Продлить ({15 * (c.extensions + 1)}Ш)" };
                extItem.Click += (s, e) => {
                    App.addMoney(-15 * (c.extensions++ + 1),$"Продление карточки \"{c.type.name}\"");
                    c.daysRemain++;
                    updateData();
                };
                litem.ContextMenu.Items.Add(extItem);
                cardList.Items.Add(litem);
            }
        }
        private void dev(object sender, EventArgs e) {
            App.toggleDevMode();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!App.reallyClose) {
                e.Cancel = true;
                Hide();
            }
        }

        private void close(object sender, RoutedEventArgs e) {
            App.close();
        }
    }
}
