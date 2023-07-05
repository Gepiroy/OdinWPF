using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace OdinWPF {
    public static class Elements {
        public static Button makeButton(Button but, Action<object, EventArgs> click) {
            but.Click += new RoutedEventHandler(click);
            return but;
        }
        public static Button makeButton(Button but, Action<object, EventArgs> click, out Button set) {
            set = makeButton(but, click);
            return but;
        }
        public static Grid makeGrid(int[] widths, int[] heights) {
            Grid grid = new Grid();
            foreach (int w in widths) grid.ColumnDefinitions.Add(new ColumnDefinition() { Width=new GridLength(w)});
            foreach (int h in heights) grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(h) });
            return grid;
        }
        public static void insertToGrid(Grid grid, UIElement el, int x, int y) {
            grid.Children.Add(el);
            if (grid.RowDefinitions.Count <= y) grid.RowDefinitions.Add(new RowDefinition());
            Grid.SetColumn(el, x);
            Grid.SetRow(el, y);
        }
        public static void setPosAtCanvas(UIElement el, double x, double y) {
            Canvas.SetLeft(el, x);
            Canvas.SetTop(el, y);
        }
    }
}
