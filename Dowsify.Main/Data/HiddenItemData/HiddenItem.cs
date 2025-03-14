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

        // HGSS Table
        public HiddenItem(int tableIndex, ushort itemId, ushort quantity, ushort index)
        {
            TableIndex = tableIndex;
            ItemId = itemId;
            Quantity = quantity;
            Index = index;
        }

        // Platinum Table
        public HiddenItem(int tableIndex, ushort itemId, byte quantity, byte range, ushort index)
        {
            TableIndex = tableIndex;
            ItemId = itemId;
            QuantityPlat = quantity;
            RangePlat = range;
            Index = index;
        }

        public int TableIndex { get; set; }
        public ushort ItemId { get; set; }
        public ushort Quantity { get; set; }
        public ushort Index { get; set; }
        public string CommonScript => (Index + 8000).ToString();

        #region Platinum
        public byte QuantityPlat { get; set; }
        public byte RangePlat { get; set; }

        #endregion Platinum
    }
}
