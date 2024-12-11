using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dowsify.Main.Enums
{
    public enum GameFamily : byte
    {
        Unknown,
        DiamondPearl,
        Platinum,
        HeartGoldSoulSilver,
        HgEngine
    }

    public enum GameLanguage : byte
    {
        Unknown,
        English,
        Japanese,
        Italian,
        Spanish,
        French,
        German,
        Chinese
    }

    public enum GameVersion : byte
    {
        Unknown,
        Diamond,
        Pearl,
        Platinum,
        HeartGold,
        SoulSilver,
        HgEngine
    }

    public enum NarcDirectory : byte
    {
        Unknown,
        synthOverlay,
        textArchives,
    }

    public enum LoadType
    {
        UnpackRom = 0,
        LoadRomData = 1,
        UnpackNarcs = 2,
        SetupEditor = 3,
    }
}
