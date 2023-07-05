using OdinWPF.FileSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace OdinWPF {
    public class QuestSlot {
        static Random r = App.r;
        public string text, source;
        public int repairSeconds=0, hits = 0, secMin, secMax, perHit=30*60, minRew, maxRew;
        public double cardChance;
        public long lastAction = 0;
        bool isQuestT = false;

        SaveFile sf;

        TextBlock tb;
        Button regenBut;

        public QuestSlot(string source, int secMin, int secMax, double cardChance, int minRew, int maxRew) {
            lastAction = Global.getNowSec();
            this.source = source;
            App.quests.Add(this);
            this.cardChance = cardChance;
            this.minRew = minRew;
            this.maxRew = maxRew;
            this.secMin = secMin;
            this.secMax = secMax;
        }
        public void load(SaveFile f) {
            sf = f;
            lastAction = long.Parse(f.get("lastAction")?? Global.getNowSec().ToString());
            text = f.get("text");
            repairSeconds = int.Parse(f.get("repairSeconds") ?? "0");
            hits = int.Parse(f.get("hits") ?? "0");
            isQuestT = bool.Parse(f.get("isQuestT") ?? "False");
        }
        public void save() {
            sf.set("lastAction",lastAction.ToString());
            sf.set("text", text);
            sf.set("repairSeconds", repairSeconds.ToString());
            sf.set("hits", hits.ToString());
            sf.set("isQuestT", isQuestT.ToString());

            sf.save();
        }
        public UIElement genComp() {
            Border border = new Border();
            border.Padding = new Thickness(2);
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(0,0,0));
            StackPanel panel = new StackPanel();
            panel.Children.Add(new TextBlock {
                Text = source
            });
            tb = new TextBlock {
                Text = text
            };
            panel.Children.Add(tb);
            StackPanel butSpace = new StackPanel {
                Orientation = Orientation.Horizontal
            };
            butSpace.Children.Add(Elements.makeButton(new Button {
                Content = remain()<0?"Реген":"25Ш"
            }, (s, e) => {
                if (remain() >= 0) App.addMoney(-25,"Ускоренное получение "+source);
                LinerFile tquests = new LinerFile(App.userpath + source + "T.txt", true);
                if (tquests.sts.Count > 0 && r.NextDouble() < 0.5) {
                    text = tquests.sts[r.Next(tquests.sts.Count)];
                    isQuestT = true;
                } else {//Не так уж и дорого каждый раз заново открывать файл. Зато можно не закрывая прогу редачить список квестов!
                    StructFile sf = new StructFile(App.folderpath + source + ".txt", true);
                    Line line = sf.coreLine.totallyRandomLine();
                    text = line.totallyFullName(""," - ");
                }
                acted();
            }, out regenBut));
            butSpace.Children.Add(Elements.makeButton(new Button {
                Content = "Готово"
            }, (s, e) => {
                App.logMessage($"Выполнен квест \"{text}\"!");
                App.addMoney(minRew + App.r.Next(maxRew-minRew+1),"Награда за квест");
                App.milBase.addSpirit(App.r.NextDouble()*maxRew*0.0065);
                if (App.r.NextDouble() < cardChance) {
                    App.addCard();
                }
                if (isQuestT) {
                    LinerFile tquests = new LinerFile(App.userpath + source + "T.txt", true);
                    string real = text;
                    if (text.StartsWith("(")) {
                        int last = text.LastIndexOf(")");
                        int parsed = int.Parse(text.Substring(1, last - 1));
                        real = text.Substring(last + 1);
                        if (--parsed != 0) {
                            tquests.sts.Add($"({parsed}){real}");
                        }
                    }
                    tquests.sts.Remove(text);
                    tquests.save();
                    isQuestT = false;
                }
                text=null;
                acted();
            }));
            butSpace.Children.Add(Elements.makeButton(new Button {
                Content = "Отмена"
            }, (s, e) => {
                fail();
            }));

            panel.Children.Add(butSpace);
            
            border.Child = panel;
            return border;
        }
        public void fail() {
            text = null;
            isQuestT = false;
            hits++;
            acted();
        }
        void acted() {
            lastAction = Global.getNowSec();
            repairSeconds = secMin + App.r.Next(secMax - secMin + 1) + hits * perHit;
            updateTextBlock();
        }
        int remain() {
            return repairSeconds - (int)(Global.getNowSec() - lastAction);
        }
        public void updateTextBlock() {
            if (tb == null) return;
            if (text != null) tb.Text = text;
            else {
                int secRemain = remain();
                if (secRemain < 0) {
                    tb.Text = "Можно новый брать";
                    regenBut.Content = "Реген";
                } else {
                    tb.Text = $"{secRemain / 3600}h:{secRemain % 3600 / 60}m:{secRemain % 60}s";
                    regenBut.Content = "25Ш";
                }
            }
        }
    }
}
