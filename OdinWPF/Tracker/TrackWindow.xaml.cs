using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OdinWPF.Tracker {
    /// <summary>
    /// Так. Ну короче это окно, в котором трекнутый день отображается. Оно расшифровывает все события и превращает их в дорожку временную, при наводке на каждый кусок
    /// которой будет выводиться инфа, что это было. Но сначала надо зашифровать названия программ... Или не надо? Не, лучше не надо.
    /// </summary>
    public partial class TrackWindow : Window {
        TrackedDay day;
        long secFrom, secTo, dist;
        public TrackWindow(TrackedDay day) {
            InitializeComponent();
            this.day = day;
            secFrom = day.log[0].secStart;
            secTo = Global.getNowSec();
            dist = secTo - secFrom;
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            draw();
        }

        double secToX(long sec) {
            return ActualWidth * (1.0 * (sec - secFrom) / dist);
        }
        double secsToX(long sec) {
            return ActualWidth * (1.0 * sec / dist);
        }

        void update() {
            secTo= Global.getNowSec();
        }

        void draw() {
            if (ActualWidth <= 0 || ActualHeight <= 0||double.IsNaN(ActualHeight) || double.IsNaN(ActualWidth)) return;
            canv.Children.Clear();
            update();
            canv.Children.Add(namesHolder);
            double x = 0;
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            int jump = 60;
            while (secsToX(jump) < 50) jump += 60;
            date = date.AddSeconds(secFrom - secFrom % jump);
            for (long t=secFrom-secFrom%jump + jump; t<secTo;t+=jump) {
                date = date.AddSeconds(jump);
                TextBlock tm = new TextBlock { Text = date.ToString("HH:mm")};
                x = secToX(t);
                Elements.setPosAtCanvas(tm, x, 100);
                Line ln = new Line {
                    X1 = x, X2 = x, Y1 = 20, Y2 = 100, Stroke = Brushes.Black, StrokeThickness = 1
                };
                canv.Children.Add(tm);
                canv.Children.Add(ln);
            }
            x = 0;
            int l = 0;
            for (int i = 0; i < day.log.Count; i++) {
                TrackedThing t = day.log[i];
                if (day.log.Count == i + 1) l = (int)(secTo - t.secStart);
                else l = (int)(day.log[i + 1].secStart - t.secStart);
                Rectangle rect = new Rectangle();
                rect.Height = 20;
                rect.Width = 1.0 * l / dist * ActualWidth;
                rect.Fill = new SolidColorBrush(t.pinf.color);
                rect.MouseEnter += (s, e) => {
                    procName.Text = t.pinf.name;
                    screenName.Text = t.subName;
                };
                Elements.setPosAtCanvas(rect, x, 50);
                canv.Children.Add(rect);
                x += rect.Width;
            }
            day.reparseWorkTime();
            x = 0;
            for (int i = 0; i < day.parsedWorkTime.Count; i+=2) {
                x = ActualWidth * (1.0 * (day.parsedWorkTime[i]-secFrom) / dist);
                l = (int)(day.parsedWorkTime[i+1]-day.parsedWorkTime[i]);
                Rectangle rect = new Rectangle();
                rect.Height = 10;
                rect.Width = 1.0 * l / dist * ActualWidth;
                rect.Fill = new SolidColorBrush(Color.FromRgb(255,225,0));
                Elements.setPosAtCanvas(rect, x, 72);
                canv.Children.Add(rect);
            }
            TextBlock twt = new TextBlock { Text="Worked: "+new DateTime(TimeSpan.FromSeconds(day.workedSecondsSince(0)).Ticks).ToString("HH:mm:ss")};
            Elements.setPosAtCanvas(twt, 0, 120);
            canv.Children.Add(twt);
        }
    }
}
