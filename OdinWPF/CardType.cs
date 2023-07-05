using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinWPF {
    public class CardType {
        public string name, lore;
        public Law law;
        public CardType(string name) {
            this.name = name;
        }
        public CardType setLore(string lore) {
            this.lore = lore;
            return this;
        }
    }
}
