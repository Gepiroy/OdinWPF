using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OdinWPF.Military {
    public class MilBase {
        static Random r = App.r;
        /*
         * Основной принцип всей системы: ничто не постоянно. Принцип №2 - миром правит рандом.
         * Постройки ломаются каждое утро с шансом в 20%. Также текущие солдаты уходят каждое утро с шансом в 50%-дух*50% шансом.
         * Дух не бывает выше 100%, ниже 0%. Каждый день падает на 20%.
         * Можно платить за содержание:
         * Воинов (1Ш/10в. Если не заплатить, то сл. утром ещё -20% - итого -40% за день)
         * Зданий (1Ш/здание. Шанс обрушения не 20%, а 10%.)
         * Также есть способ быстро поднять боевой дух: 1Ш/10в+10Ш = +10%БД.
         * Здания:
         * - Казарма (+10% к приходящим людям) (20Ш, +5Ш/шт)
         * 
         * Приходят люди тогда, когда ты объявляешь сборы. Их можно провести бесплатно, там будет такой счёт:
         * 50*%прих*(0.5~1)*spirit.
         * Если хочешь - можешь провести их платно вдобавок к бесплатному, но тогда с тебя 20Ш+10Ш/recall.
         */
        public int soldiers=50;
        public double spirit=0.5;
        public SaveFile save;
        int recalls = 0, morals = 0;
        public long lastAction = 0;
        public int repairSeconds = 0;
        public int caserns = 0;



        public MilBase(string savePath) {
            save = new SaveFile(savePath);
            if (!save.exists()) {
                saveSave();
            }
            soldiers = int.Parse(save.get("soldiers"));
            spirit = double.Parse(save.get("spirit"));
            recalls = int.Parse(save.get("recalls")??"0");
            morals = int.Parse(save.get("morals")??"0");
            lastAction = long.Parse(save.get("lastAction")??Global.getNowSec().ToString());
            repairSeconds = int.Parse(save.get("repairSeconds")??"0");
            caserns = int.Parse(save.get("caserns") ?? "0");
        }

        public void day() {
            addSpirit(-spirit*0.25);
            if (morals == 0) addSpirit(-0.15);
            morals = 0;
            recalls = 0;
            if (r.NextDouble() < 0.5) {
                int loses = (int)(soldiers * (0.5 - spirit * 0.3) * (r.NextDouble() * 0.3 + 0.3));
                soldiers -= loses;
                int lb = 0;
                /*foreach (MilBuilding b in buildings.ToArray()) {
                    if (r.NextDouble() < 0.3 * (1 - spirit * 0.5)) {
                        buildings.Remove(b);
                        lb++;
                    }
                }*/
                for (int i=0;i<caserns;i++) {
                    if (r.NextDouble() < 0.3 * (1 - spirit * 0.5)) {
                        lb++;
                    }
                }
                caserns -= lb;
                App.logMessage($"Этой ночью на нас напали. Потеряно {loses} солдат, разрушено {lb} построек.");
            }
            int remc = 0;
            for (int i=0;i<caserns;i++) {
                if (r.NextDouble() < 0.2) remc++;
            }
            caserns -= remc;
            updateGrid();
        }

        private Grid grid;
        private TextBlock
            displSoldiers = new TextBlock { FontSize=14},
            displSpirit = new TextBlock { FontSize=14};
        private Button
            butAdd,
            butMoral,
            butCasern,
            butRaid,
            butAnarchy;

        public Grid militaryGrid() {
            grid = Elements.makeGrid(new int[] { 120, 120, 120 }, new int[] { 20, 30, 30 });
            Elements.insertToGrid(grid, displSoldiers, 0, 0);
            Elements.insertToGrid(grid, displSpirit, 1, 0);
            butRaid = Elements.makeButton(new Button { Content = "Raid", Background = new SolidColorBrush(Color.FromRgb(200, 0, 0)) }, (s, e) => {
                startFight(1,0,80 + App.r.Next(200));
                if (fight.victory) {
                    App.logMessage("Успешный рейд.");
                    if (r.NextDouble() < 0.7) App.addCard();
                    App.addMoney(App.r.Next(16) + 5,"Рейд");
                } else {
                    App.logMessage("Провальный рейд.");
                }
                endFight();
            });
            Elements.insertToGrid(grid, butRaid, 0, 2);
            butAnarchy = Elements.makeButton(new Button { Content = "Anarchy", Foreground = new SolidColorBrush(Color.FromRgb(255,255,0)), Background = new SolidColorBrush(Color.FromRgb(100, 0, 0)) }, (s, e) => {
                startFight(1, 0, 500 + App.r.Next(1000));
                if (fight.victory) {
                    App.logMessage("СЛАВА АНАРХИИ!");
                    foreach (Law l in App.activeLaws.ToArray()) {
                        App.changeLaw(l);
                    }
                } else {
                    App.logMessage("Попытка штурма ЦПР и установление анархии провалилась.");
                }
                endFight();
            });
            Elements.insertToGrid(grid, butAnarchy, 2, 2);
            butAdd = Elements.makeButton(new Button { Content = "Призыв", Background = new SolidColorBrush(Color.FromRgb(0, 100, 200)) }, (s, e) => {
                if (recalls != 0) App.addMoney(-10*recalls,"Призыв");
                int add = (int)Math.Round(50 * (1.0 + caserns * 0.1) * (0.5+spirit*0.5) * (r.NextDouble() * 0.5 + 0.5));
                MessageBox.Show($"На призыв откликнулось {add} чел.");
                soldiers += add;
                recalls++;
                updateGrid();
            });
            Elements.insertToGrid(grid, butAdd, 0, 1);
            butCasern = Elements.makeButton(new Button { Content = "Казарма№", Background = new SolidColorBrush(Color.FromRgb(0, 100, 200)) }, (s, e) => {
                App.addMoney(-20-caserns*5,"Казарма");
                caserns++;
                updateGrid();
            });
            Elements.insertToGrid(grid, butCasern, 2, 1);
            butMoral = Elements.makeButton(new Button { Content = "Мораль", Background = new SolidColorBrush(Color.FromRgb(0, 200, 200)) }, (s, e) => {
                App.addMoney(-(int)Math.Ceiling(soldiers / 15.0)-8*morals,"Деньги войскам");
                if (morals++ != 0) addSpirit(0.1);
                updateGrid();
            });
            Elements.insertToGrid(grid, butMoral, 1, 1);

            updateGrid();
            return grid;
        }
        private Fight fight;
        public Fight startFight(int ourMode, int theirMode, int enemies) {
            fight = new Fight(new Army(soldiers, spirit).setMode(ourMode), new Army(enemies, 0.5).setMode(theirMode));
            fight.ShowDialog();
            return fight;
        }
        public void endFight() {
            if (fight.victory) {
                addSpirit(r.NextDouble() * 0.12);
            } else {
                addSpirit(r.NextDouble() * -0.12);
            }
            soldiers -= fight.loses;
            updateGrid();
            fight = null;
            lastAction = Global.getNowSec();
            repairSeconds = r.Next(3600*2)+3600*4;
        }

        public Army sendArmy() {
            return new Army(soldiers, spirit);
        }

        public void addSpirit(double am) {
            spirit += am;
            if (spirit > 1) spirit = 1;
            else if (spirit < 0) spirit = 0;
            updateGrid();
        }

        public void updateGrid() {
            if (butMoral == null) return;
            displSoldiers.Text = $"{soldiers} чел.";
            displSpirit.Text = $"Дух: {spirit.ToString("0.0%")}";
            if(morals==0)butMoral.Content = $"(!)Зарплата ({(int)Math.Ceiling(soldiers/15.0)}Ш)";
            else butMoral.Content = $"+10 духа ({(int)Math.Ceiling(soldiers / 15.0)+8*morals}Ш)";
            if (recalls == 0) butAdd.Content = $"Призыв";
            else butAdd.Content = $"Призыв ({10*recalls}Ш)";
            int remain = (int)(lastAction+repairSeconds-Global.getNowSec());
            if (remain < 0) remain = 0;
            if (remain != 0) butRaid.Content = $"{remain/3600}:{remain%3600/60}:{remain%60}";
            else butRaid.Content = "Raid";
            butCasern.Content = $"Казарма№{caserns+1} ({20+caserns*5}Ш)";
        }

        public void saveSave() {
            save.set("soldiers", soldiers.ToString());
            save.set("spirit", spirit.ToString());
            save.set("recalls", recalls.ToString());
            save.set("morals", morals.ToString());
            save.set("lastAction", lastAction.ToString());
            save.set("repairSeconds", repairSeconds.ToString());
            save.set("caserns", caserns.ToString());
            save.save();
        }
    }
}
