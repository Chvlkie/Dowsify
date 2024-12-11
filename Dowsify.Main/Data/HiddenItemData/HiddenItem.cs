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
        public HiddenItem(int tableIndex, uint itemId, uint quantity, uint index)
        {
            TableIndex = tableIndex;
            ItemId = itemId;
            Quantity = quantity;
            Index = index;
        }

        public int TableIndex { get; set; }
        public uint ItemId { get; set; }
        public uint Quantity { get; set; }
        public uint Index { get; set; }
        public string CommonScript => (Index + 8000).ToString();
    }
}
