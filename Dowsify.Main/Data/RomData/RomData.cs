using Dowsify.Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dowsify.Main.Global.GlobalVariables;
using static Dowsify.Main.Database.Database;

namespace Dowsify.Main.Data
{
    public static class RomData
    {
        public static string Arm9Path => WorkingDirectory + Arm9FilePath;
        public static byte EuropeByte { get; set; }
        public static string FileName { get; set; }
        public static string GameCode { get; set; }
        public static GameFamily GameFamily => GameVersion switch
        {
            GameVersion.Diamond or GameVersion.Pearl => GameFamily.DiamondPearl,
            GameVersion.Platinum => GameFamily.Platinum,
            GameVersion.HeartGold or GameVersion.SoulSilver => GameFamily.HeartGoldSoulSilver,
            GameVersion.HgEngine => GameFamily.HeartGoldSoulSilver,
            _ => GameFamily.Unknown,
        };

        public static GameLanguage GameLanguage => GameCode switch
        {
            "ADAE" or "APAE" or "CPUE" or "IPKE" or "IPGE" => GameLanguage.English,
            "ADAS" or "APAS" or "CPUS" or "IPKS" or "IPGS" or "LATA" => GameLanguage.Spanish,
            "ADAI" or "APAI" or "CPUI" or "IPKI" or "IPGI" => GameLanguage.Italian,
            "ADAF" or "APAF" or "CPUF" or "IPKF" or "IPGF" => GameLanguage.French,
            "ADAD" or "APAD" or "CPUD" or "IPKD" or "IPGD" => GameLanguage.German,
            _ => GameLanguage.Japanese,
        };

        public static GameVersion GameVersion { get; set; }
        public static List<HiddenItem> HiddenItems { get; set; }

        public static uint HiddenTableOffset => GameVersion switch
        {
            GameVersion.HeartGold => 0xFA558,
            GameVersion.HgEngine => 0xFA558,
            _ => 0x0,
        };

        public static int HiddenTableSize => GameVersion switch
        {
            GameVersion.HeartGold => 231,
            GameVersion.HgEngine => 231,
            _ => 0,
        };

        public static List<string> ItemNames { get; set; }
        public static string OverlayPath => WorkingDirectory + OverlayFilePath;
        public static string OverlayTablePath => WorkingDirectory + Y9FilePath;
        public static string WorkingDirectory { get; set; }

        public static int ItemNamesTextNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => 344,
            GameFamily.Platinum => 392,
            GameFamily.HeartGoldSoulSilver => GameLanguage == GameLanguage.Japanese ? 219 : 222,
            GameFamily.HgEngine => GameLanguage == GameLanguage.Japanese ? 219 : 222,
            _ => 0
        };

        public static void SetupRomFile(string romName, string workingDirectory, bool suffix = true)
        {
            Console.WriteLine("Setup Rom File");
            WorkingDirectory = workingDirectory;

            try
            {
                GameVersion = GameVersions[GameCode];
            }
            catch (KeyNotFoundException)
            {
                GameVersion = GameVersion.Unknown;
                Console.WriteLine("Rom not supported");
            }

            FileName = romName;
            Console.WriteLine("Setup Rom File | Success");
        }
    }
}