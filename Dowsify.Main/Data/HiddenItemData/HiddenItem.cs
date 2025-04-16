using Dowsify.Main.Enums;
using Newtonsoft.Json;
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

            // HGSS Constructor
            public HiddenItem(int tableIndex, ushort itemId, ushort quantity, ushort index)
            {
                GameFamily = GameFamily.HeartGoldSoulSilver;
                TableIndex = tableIndex;
                ItemId = itemId;
                Quantity = quantity;
                Index = index;
            }

            // Platinum Constructor
            public HiddenItem(int tableIndex, ushort itemId, byte quantity, byte range, ushort index)
            {
                GameFamily = GameFamily.Platinum;
                TableIndex = tableIndex;
                ItemId = itemId;
                QuantityPlat = quantity;
                RangePlat = range;
                Index = index;
            }

            [JsonProperty("tableIndex")]
            public int TableIndex { get; set; }

            [JsonProperty("itemId")]
            public ushort ItemId { get; set; }

            [JsonProperty("index")]
            public ushort Index { get; set; }

            [JsonIgnore] // Exclude from JSON (computed property)
            public string CommonScript => (Index + 8000).ToString();

            // HGSS Properties
            [JsonProperty("quantity", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public ushort Quantity { get; set; }

            // Platinum Properties
            [JsonProperty("quantityPlat", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public byte QuantityPlat { get; set; }

            [JsonProperty("rangePlat", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public byte RangePlat { get; set; }

            [JsonProperty("gameFamily")]
            public GameFamily GameFamily { get; set; }
        }
    }
