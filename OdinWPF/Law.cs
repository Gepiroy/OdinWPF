using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OdinWPF {
    public class Law {
        public string name;
        public string lore;
        public List<CardType> cards = new List<CardType>();
        public List<string> alertHooks = new List<string>();

        public Law(string name) {
            this.name = name;
        }
        public Law addLore(string lore) {
            this.lore = lore;
            return this;
        }
        public Law addCard(CardType card) {
            cards.Add(card);
            card.law = this;
            return this;
        }

        public override string ToString() {
            return name;
        }

        public ListViewItem genListView() {
            Border border = new Border();
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(0,0,0));
            border.Padding = new Thickness(2);
            StackPanel grid = new StackPanel();
            border.Child = grid;
            grid.Children.Add(new TextBlock {
                Text = name, FontSize = 16

            });
            
            grid.Children.Add(new TextBlock {
                Text = lore
            });
            ListViewItem litem = new ListViewItem();
            litem.Content = border;
            return litem;
        }
    }
}
