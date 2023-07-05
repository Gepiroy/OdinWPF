using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Xml.Linq;

namespace OdinWPF {
    public class Card {
        public CardType type;
        public int daysRemain, extensions=0;


        public Card(CardType type) {
            this.type = type;
        }
        public Card addDays(int am) {
            daysRemain += am;
            return this;
        }
        public Card setExt(int am) {
            extensions = am;
            return this;
        }
        public ListViewItem genListView() {
            Border border = new Border();
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            if(daysRemain==0) border.BorderBrush = new SolidColorBrush(Color.FromRgb(175, 175, 175));
            border.Padding = new Thickness(2);
            StackPanel grid = new StackPanel();
            border.Child = grid;
            Grid mgrid = new Grid();
            //mgrid.ColumnDefinitions[0].Width = new GridLength;
            grid.Children.Add(mgrid);
            putToGrid(new TextBlock {
                Text = type.name, FontSize = 16, HorizontalAlignment = HorizontalAlignment.Left
            }, mgrid, 0, 0);
            putToGrid(new TextBlock {
                Text = daysRemain+"d", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Right
            }, mgrid, 1, 0);

            grid.Children.Add(new TextBlock {
                Text = type.lore
            });
            grid.Children.Add(new TextBlock {
                Text = "К закону \""+type.law.name+"\"", FontSize = 12
            });
            ListViewItem litem = new ListViewItem();
            litem.Content = border;
            return litem;
        }
        void putToGrid(UIElement el, Grid grid, int x, int y) {
            grid.Children.Add(el);
            Grid.SetColumn(el, x);
            Grid.SetRow(el, y);
        }
    }
}
