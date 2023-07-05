using Microsoft.Toolkit.Uwp.Notifications;
using OdinWPF.Military;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace OdinWPF
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application{
        public static bool devMode = false;
        public static Random r = new Random();
        public static int money = 0;
        public static TextBlock moneyBox;
        public static List<Law> lawList = new List<Law>();
        public static List<Law> activeLaws = new List<Law>();
        public static List<CardType> cardList = new List<CardType>();
        public static List<Card> activeCards = new List<Card>();
        public static List<QuestSlot> quests = new List<QuestSlot>();
        public static QuestSlot OdinQuest = new QuestSlot("OdinQuests", 3600*2, 3600*4, 0.7, 8, 18);
        public static QuestSlot DarSieQuest = new QuestSlot("DarSieQuests", 3600*6, 3600*8, 0.5, 1, 5);
        public static QuestSlot RebelQuests = new QuestSlot("RebelQuests", 3600 * 12, 3600 * 16, 0.5, 5, 15);

        public static System.Windows.Forms.NotifyIcon tray = new System.Windows.Forms.NotifyIcon();

        public static string folderpath = "";
        public static string userpath = "";
        public static SaveFile globalSaveFile;
        static SaveFile save;

        public static MilBase milBase;

        private static readonly DispatcherTimer _timer;

        public static List<string> log = new List<string>();
        public static ListBox logBox = new ListBox {
            Height = 300
        };

        static App() {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        }

        public static void day() {
            logMessage("Новый день...");
            foreach (Card c in activeCards.ToArray()) {
                if (--c.daysRemain == -1) {
                    activeCards.Remove(c);
                    logMessage($"removed card \"{c.type.name}\" because of expire.");
                }
            }
            foreach (QuestSlot q in quests) {
                if (q.text != null)q.fail();
                q.repairSeconds = 0;
                if (q.hits > 0) q.hits--;
            }
            milBase.day();
            if(OdinWPF.MainWindow.instance!=null) OdinWPF.MainWindow.instance.updateData();
            changeLaws();
        }

        protected override void OnStartup(StartupEventArgs e){
            Process ourProcess = Process.GetCurrentProcess();
            int ourProcesses = 0;
            foreach (Process p in Process.GetProcessesByName("OdinWPF")) {
                if (p.MainModule.FileName.Equals(ourProcess.MainModule.FileName)&&p!=ourProcess) {
                    ourProcesses++;
                }
            }
            if (ourProcesses>1) {
                MessageBox.Show("Этот Один уже запущен.");
                ourProcess.Kill();
                return;//may its useless?
            }
            folderpath = System.AppDomain.CurrentDomain.BaseDirectory+ "odin_data\\";
            Tracker.TrackCore.init();
            base.OnStartup(e);
            
            tray.Icon = OdinWPF.Properties.Resources.app;
            tray.Click += (o, ee) => { OdinWPF.MainWindow.instance.Show(); };
            tray.Visible = true;
            LinerFile lawListFile = new LinerFile(folderpath+"lawList.txt", true);
            logBox.ItemsSource = log;
            foreach (string st in lawListFile.sts) {
                if (st.StartsWith("   ")) {
                    lawList[lawList.Count - 1].cards[lawList[lawList.Count - 1].cards.Count-1].setLore(st.Substring(3));
                } else if (st.StartsWith("  alert:")) {
                    lawList[lawList.Count - 1].alertHooks.Add(st.Substring(8));
                } else if (st.StartsWith("  ")) {
                    lawList[lawList.Count - 1].addCard(new CardType(st.Substring(2)));
                } else if (st.StartsWith(" ")) {
                    lawList[lawList.Count-1].addLore(st.Substring(1));
                } else {
                    lawList.Add(new Law(st));
                }
            }

            foreach (Law l in lawList) {
                foreach (CardType c in l.cards) {
                    cardList.Add(c);
                }
            }

            globalSaveFile = new SaveFile(folderpath + "global.txt");
            devMode = bool.Parse(globalSaveFile.get("devMode") ?? "False");

            

            if (devMode) userpath = folderpath + @"devsave\";
            else userpath = folderpath + @"save\";
            Directory.CreateDirectory(userpath);

            save = new SaveFile(userpath+"save.txt");
            loadSave();

            _timer.Tick += sec;
            _timer.Start();

            ProjectSystem.Projects.init();
        }
        public static void addCard(CardType type, int days) {
            MessageBox.Show($"Получена карточка \"{type.name}\" на {days} д.!");
            logMessage($"Получена карточка \"{type.name}\" на {days} д.!");
            foreach (Card c in activeCards) {
                if (c.type == type) {
                    c.addDays(days);
                    if (OdinWPF.MainWindow.instance != null) OdinWPF.MainWindow.instance.updateData();
                    return;
                }
            }
            activeCards.Add(new Card(type).addDays(days));
            if(OdinWPF.MainWindow.instance!=null) OdinWPF.MainWindow.instance.updateData();
        }
        /*public static void addMoney(int m) {
            addMoney(m, null);
        }*/
        public static void addMoney(int m, string comment) {
            money += m;
            if (moneyBox != null) moneyBox.Text = money + "Ш";
            logMessage($"{(m>=0?"+":"")}{m}Ш{(string.IsNullOrEmpty(comment)?"":" ("+comment+")")}");
        }

        public static void toggleDevMode() {
            devMode = !devMode;
            Current.Shutdown();
        }

        static int astSet=300, autoSaveTimer = astSet;
        void sec(object o, EventArgs e) {
            foreach(QuestSlot q in quests)q.updateTextBlock();
            milBase.updateGrid();
            Tracker.TrackCore.sec();
            if (--autoSaveTimer == 0) {
                autoSaveTimer = astSet;
                saveAll();
            }
        }
        static void saveAll() {
            globalSaveFile.set("devMode", devMode.ToString());
            globalSaveFile.save();
            save.set("money",money.ToString());
            save.set("lastActivity", TM.today().ToString());
            save.save();
            List<string> alsts = new List<string>();
            foreach (Law l in activeLaws) alsts.Add(l.name);
            File.WriteAllLines(userpath+"activeLaws.txt", alsts);

            foreach (QuestSlot q in quests)q.save();

            List<string> acsts = new List<string>();
            foreach (Card c in activeCards) {
                acsts.Add(c.type.name);
                acsts.Add("  "+c.daysRemain);
                acsts.Add("   " + c.extensions);
            }
            File.WriteAllLines(userpath + "activeCards.txt", acsts);

            milBase.saveSave();

            File.WriteAllLines(userpath+"log.txt",log);

            Tracker.TrackCore.save();

            ProjectSystem.Projects.save();
        }
        static void loadSave() {
            if(File.Exists(userpath+"log.txt"))log.AddRange(File.ReadAllLines(userpath+"log.txt"));
            money = int.Parse(save.get("money") ?? "0");
            foreach (QuestSlot q in quests)q.load(new SaveFile(userpath+q.source+".txt"));
            LinerFile userLaws = new LinerFile(userpath + "activeLaws.txt", true);
            foreach (string st in userLaws.sts) {
                foreach (Law l in lawList) {
                    if (l.name.Equals(st)) activeLaws.Add(l);
                }
            }
            LinerFile userCards = new LinerFile(userpath + "activeCards.txt", true);
            CardType lastType = null;
            foreach (string st in userCards.sts) {
                if (st.StartsWith("   ")) {
                    activeCards[activeCards.Count - 1].setExt(int.Parse(st.Substring(3)));
                    continue;
                }
                else if (st.StartsWith("  ")) {
                    activeCards.Add(new Card(lastType).addDays(int.Parse(st.Substring(2))));
                    continue;
                }
                foreach (CardType c in cardList) {
                    if (c.name.Equals(st)) {
                        lastType = c;
                        break;
                    }
                }
            }
            milBase = new MilBase(userpath+"milBase.txt");
            //MUST be the last.
            int days = (int)(TM.today() - long.Parse(save.get("lastActivity") ?? TM.today().ToString()));
            for (int i = 0; i < days; i++) {
                day();
            }
        }
        private void onExit(object sender, ExitEventArgs e) {
            saveAll();
            tray.Visible = false;
        }
        public static void changeLaws() {
            foreach (Law l in lawList) {
                if (r.NextDouble() <= 0.2) {
                    changeLaw(l);
                }
            }
        }
        public static void changeLaw(Law l) {
            if (activeLaws.Contains(l)) {
                activeLaws.Remove(l);
                logMessage("Removed Law \"" + l.name + "\"!");
                foreach (Card c in activeCards.ToArray()) {
                    if (l.cards.Contains(c.type)) {
                        activeCards.Remove(c);
                        logMessage("  Removed Card \"" + c.type.name + "\"!");
                        if(c.daysRemain>0)addMoney(10 * c.daysRemain,"Компенсация за карточку");
                    }
                }
            } else {
                activeLaws.Add(l);
                logMessage("New Law, \"" + l.name + "\"!");
            }
            if (OdinWPF.MainWindow.instance != null) OdinWPF.MainWindow.instance.updateData();
        }
        public static void logMessage(string st) {
            log.Add($"[{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}] {st}");
            logBox.Items.Refresh();
            logBox.ScrollIntoView(logBox.Items[logBox.Items.Count-1]);
        }
        public static void addCard() {
            List<CardType> avCards = new List<CardType>();
            foreach (Law l in activeLaws) {
                foreach (CardType c in l.cards) {
                    bool already = false;
                    foreach (Card ac in activeCards) {
                        if (ac.type == c) {
                            already = true;
                            break;
                        }
                    }
                    if (!already) avCards.Add(c);
                }
            }
            if (avCards.Count == 0&&activeCards.Count==0) return;
            int red = r.Next(avCards.Count * 2 + activeCards.Count);
            if (red < avCards.Count * 2) addCard(avCards[r.Next(avCards.Count)], 1 + r.Next(3));
            else addCard(activeCards[r.Next(activeCards.Count)].type, 1 + r.Next(3));
        }
        public static bool reallyClose = false;
        public static void close() {
            reallyClose = true;
            Current.Shutdown();
        }

    }
}
