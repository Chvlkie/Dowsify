using Dowsify.Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dowsify.Main.Global
{
    public static class GlobalVariables
    {
        public const uint Arm9Offset = 0x02000000;
        public const string NdsToolsFilePath = @"Tools\ndstool.exe";
        public const string BlzFilePath = @"Tools\blz.exe";
        public const string NdsRomFilter = "NDS Rom (*.nds)|*.nds";
        public const string Arm9FilePath = "arm9.bin";
        public const string Arm7FilePath = "arm7.bin";
        public const string Y9FilePath = "y9.bin";
        public const string Y7FilePath = "y7.bin";
        public const string DataFilePath = "data";
        public const string OverlayFilePath = "overlay";
        public const string BannerFilePath = "banner.bin";
        public const string HeaderFilePath = "header.bin";

        public static class GameFamilyNarcs
        {
            public static List<NarcDirectory> DiamondPearl = [
                        NarcDirectory.synthOverlay,
                        NarcDirectory.textArchives,
                            ];

            public static List<NarcDirectory> Platinum = [
                        NarcDirectory.synthOverlay,
                        NarcDirectory.textArchives,
                          ];

            public static List<NarcDirectory> HeartGoldSoulSilver = [
                        NarcDirectory.synthOverlay,
                        NarcDirectory.textArchives,
                          ];

            public static List<NarcDirectory> HgEngine = [
                        NarcDirectory.synthOverlay,
                        NarcDirectory.textArchives,
                          ];

            public static List<NarcDirectory> GetGameFamilyNarcs(GameFamily gameFamily) => gameFamily switch
            {
                GameFamily.DiamondPearl => DiamondPearl,
                GameFamily.Platinum => Platinum,
                GameFamily.HeartGoldSoulSilver => HeartGoldSoulSilver,
                GameFamily.HgEngine => HgEngine,
                _ => [],
            };
        }
    }
}