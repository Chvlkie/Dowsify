using Dowsify.Main.Enums;

namespace Dowsify.Main.Database
{
    public partial class Database
    {
        public static Dictionary<NarcDirectory, (string packedDirectory, string unpackedDirectory)> GameDirectories { get; set; } = null!;
    }
}