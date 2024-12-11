using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dowsify.Main.Data
{
    public class HiddenItem
    {
        public HiddenItem() { }
        public HiddenItem(int tableIndex, ushort itemId, ushort quantity, ushort index)
        {
            TableIndex = tableIndex;
            ItemId = itemId;
            Quantity = quantity;
            Index = index;
        }

        public int TableIndex { get; set; }
        public ushort ItemId { get; set; }
        public ushort Quantity { get; set; }
        public ushort Index { get; set; }
        public string CommonScript => (Index + 8000).ToString();
    }
}
