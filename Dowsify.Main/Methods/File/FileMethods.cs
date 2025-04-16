using Dowsify.Main.Data;
using Dowsify.Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace Dowsify.Main.Methods
{
    public class FileMethods : IFileMethods
    {
        private const string FileFilter = "Hidden Item Table (*.hit)|*.hit|JSON (*.json)|*.json";

        public void Export(List<HiddenItem> items, GameFamily gameFamily, List<string> itemNames)
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = FileFilter,
                DefaultExt = ".hit",
                Title = "Export Hidden Item Table"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                var exportData = new
                {
                    Metadata = new
                    {
                        Game = gameFamily.ToString(),
                        ExportDate = DateTime.Now,
                        ItemCount = items.Count
                    },
                    Items = items.ConvertAll(item => new {
                        ItemName = item.ItemId < itemNames.Count ? itemNames[item.ItemId] : "Unknown",
                        item.ItemId,
                        item.Quantity,
                        item.QuantityPlat,
                        item.RangePlat,
                        item.Index,
                        item.CommonScript
                    })                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(saveDialog.FileName, JsonSerializer.Serialize(exportData, options));
            }
        }
        public List<HiddenItem>? Import(GameFamily targetGameFamily)
        {
            using var openDialog = new OpenFileDialog
            {
                Filter = FileFilter,
                Title = "Import Hidden Item Table"
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string json = File.ReadAllText(openDialog.FileName);
                var document = JsonDocument.Parse(json);

                // Check game compatibility
                var game = document.RootElement.GetProperty("Metadata").GetProperty("Game").GetString();
                if (!Enum.TryParse<GameFamily>(game, out var sourceGameFamily))
                {
                    throw new InvalidDataException("Unknown game version in import file");
                }

                // Convert items if needed
                var items = JsonSerializer.Deserialize<List<HiddenItem>>(
                    document.RootElement.GetProperty("Items").GetRawText());

                return sourceGameFamily == targetGameFamily
                    ? items
                    : ConvertHiddenItems(items!, sourceGameFamily, targetGameFamily);
            }
            return null;
        }

        private static List<HiddenItem> ConvertHiddenItems(List<HiddenItem> items, GameFamily source, GameFamily target)
        {
            if (source == target) return items;

            return source switch
            {
                GameFamily.Platinum when target == GameFamily.HeartGoldSoulSilver =>
                    items.ConvertAll(i => new HiddenItem(i.TableIndex, i.ItemId, i.QuantityPlat, i.Index)),

                GameFamily.HeartGoldSoulSilver when target == GameFamily.Platinum =>
                    items.ConvertAll(i => new HiddenItem(i.TableIndex, i.ItemId, (byte)i.Quantity, 0, i.Index)),

                _ => throw new NotSupportedException($"Conversion from {source} to {target} not supported")
            };
        }
    }
}
