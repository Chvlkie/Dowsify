using Dowsify.Main.Data;
using Dowsify.Main.DSUtils;
using Dowsify.Main.Enums;
using Dowsify.MessageEncrypt;
using System.Diagnostics;
using System.Text;
using static Dowsify.Main.Database.Database;
using static Dowsify.Main.Global.GlobalVariables;

namespace Dowsify.Main.Methods
{
    public class RomFileMethods : IRomFileMethods
    {
        #region Write

        public (bool Success, string ErrorMessage) SaveChanges(List<HiddenItem> hiddenItems)
        {
            try
            {
                using var writer = new Arm9.Arm9Writer(RomData.HiddenTableOffset);

                switch (RomData.GameFamily)
                {
                    
                 
                    case GameFamily.Platinum:
                        foreach (var item in hiddenItems)
                        {
                            writer.Write(item.ItemId);
                            writer.Write(item.QuantityPlat);
                            writer.Write(item.RangePlat);
                            writer.Write((ushort)0);
                            writer.Write(item.Index);
                        }
                        break;
                    case GameFamily.HeartGoldSoulSilver:
                    case GameFamily.HgEngine:
                        foreach (var item in hiddenItems)
                        {
                            writer.Write(item.ItemId);
                            writer.Write(item.Quantity);
                            writer.Write((ushort)0);
                            writer.Write(item.Index);
                        }
                        break;
                }
               
               

                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        #endregion Write

        #region Extract

        public (bool Success, string ErrorMessage) LoadInitialRomData(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return (false, "Invalid ROM file path provided.");
            }

            try
            {
                using EasyReader reader = new(filePath, 0xC);

                RomData.GameCode = Encoding.ASCII.GetString(reader.ReadBytes(4));

                reader.BaseStream.Position = 0x1E;
                RomData.EuropeByte = reader.ReadByte();
                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, $"Error loading ROM data: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ExceptionMessage)> ExtractRomContentsAsync(string workingDirectory, string fileName)
        {
            var arguments = $"-x \"{fileName}\" -9 \"{RomData.Arm9Path}\" -7 \"{Path.Combine(workingDirectory, Arm7FilePath)}\" " +
                            $"-y9 \"{Path.Combine(workingDirectory, Y9FilePath)}\" -y7 \"{Path.Combine(workingDirectory, Y7FilePath)}\" " +
                            $"-d \"{Path.Combine(workingDirectory, DataFilePath)}\" -y \"{Path.Combine(workingDirectory, OverlayFilePath)}\" " +
                            $"-t \"{Path.Combine(workingDirectory, BannerFilePath)}\" -h \"{Path.Combine(workingDirectory, HeaderFilePath)}\"";

            using var unpack = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = NdsToolsFilePath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            try
            {
                unpack.Start();

                var outputTask = unpack.StandardOutput.ReadToEndAsync();
                var errorTask = unpack.StandardError.ReadToEndAsync();

                await unpack.WaitForExitAsync();

                string output = await outputTask;
                string error = await errorTask;

                if (unpack.ExitCode == 0)
                {
                    return (true, string.Empty);
                }
                else
                {
                    return (false, $"Process failed with exit code: {unpack.ExitCode}. Error: {error}. Output: {output}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Exception occurred: {ex.Message}");
            }
        }

        #endregion Extract

        #region Get

        public int GetHiddenItemsTableSize()
        {
            var items = new List<HiddenItem>();
            using var reader = new Arm9.Arm9Reader(RomData.HiddenTableSizeOffset);
            try
            {
             return RomData.GameFamily == GameFamily.HeartGoldSoulSilver ? reader.ReadByte() : reader.ReadUInt16();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading hidden items size: {ex.Message}");
                throw;
            }
        }

        public uint GetHiddenItemTableOffset()
        {
            using var reader = new Arm9.Arm9Reader(RomData.HiddenTablePointerOffset);
            try
            {
                var offset = reader.ReadUInt32();
                offset -= Arm9Offset;
                return offset;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting hidden item table offset: {ex.Message}");
                throw;
            }
        }
        public List<HiddenItem> GetHiddenItems()
        {
            var items = new List<HiddenItem>();
            using var reader = new Arm9.Arm9Reader(RomData.HiddenTableOffset);
            try
            {
                switch (RomData.GameFamily)
                {
                    case GameFamily.Platinum:
                        for (int i = 0; i < RomData.HiddenTableSize; i++)
                        {
                            ushort itemId = reader.ReadUInt16();
                            byte quantity = reader.ReadByte();
                            byte range = reader.ReadByte();
                            ushort padding = reader.ReadUInt16();
                            ushort index = reader.ReadUInt16();
                            items.Add(new HiddenItem(i, itemId, quantity, range, index));
                        }
                        break;
                    case GameFamily.HeartGoldSoulSilver:
                    case GameFamily.HgEngine:
                        for (int i = 0; i < RomData.HiddenTableSize; i++)
                        {
                            ushort itemId = reader.ReadUInt16();
                            ushort quantity = reader.ReadUInt16();
                            ushort padding = reader.ReadUInt16();
                            ushort index = reader.ReadUInt16();
                            items.Add(new HiddenItem(i, itemId, quantity, index));
                        }
                        break;
                }
                
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading hidden items: {ex.Message}");
                throw;
            }
        }

        public List<string> GetItemNames()
        {
            var messageArchives = GetMessageArchiveContents(RomData.ItemNamesTextNumber, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.ConvertAll(item => item.MessageText);
        }

        public List<MessageArchive> GetMessageArchiveContents(int messageArchiveId, bool discardLines = false)
        {
            var messageArchives = new List<MessageArchive>();

            try
            {
                string directory = $"{GameDirectories[NarcDirectory.textArchives].unpackedDirectory}\\{messageArchiveId:D4}";

                if (!File.Exists(directory))
                {
                    Console.WriteLine($"File not found: {directory}");
                    return messageArchives;
                }

                using var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                var messages = EncryptText.ReadMessageArchive(fileStream, discardLines);

                messageArchives = new List<MessageArchive>(messages.Count);

                for (int i = 0; i < messages.Count; i++)
                {
                    messageArchives.Add(new MessageArchive(i, messages[i]));
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found exception: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO exception occurred while reading the message archive: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the message archive: {ex.Message}");
                throw;
            }

            return messageArchives;
        }

        public int GetMessageInitialKey(int messageArchive)
        {
            string directory = $"{GameDirectories[NarcDirectory.textArchives].unpackedDirectory}\\{messageArchive:D4}";

            try
            {
                if (!File.Exists(directory))
                {
                    Console.WriteLine($"File not found: {directory}");
                    return -1;
                }

                using var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);
                using var reader = new BinaryReader(fileStream);
                reader.BaseStream.Position = 2;
                int initialKey = reader.ReadUInt16();

                return initialKey;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
                return -1;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error while reading file: {ex.Message}");
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the message initial key: {ex.Message}");
                return -1;
            }
        }

        public List<string> GetMoveNames(int moveTextArchive)
        {
            var messageArchives = GetMessageArchiveContents(moveTextArchive, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.ConvertAll(item => item.MessageText);
        }

        public int GetTotalNumberOfItemsInArchive(int archiveId)
        {
            var messageArchives = GetMessageArchiveContents(archiveId, false);

            return messageArchives?.Count ?? 0;
        }

        #endregion Get

        #region Set

        public void SetNarcDirectories()
        {
            Console.WriteLine("Setting up NARC directories");

            Dictionary<NarcDirectory, string> packedDirectories = RomData.GameFamily switch
            {
                GameFamily.DiamondPearl => new Dictionary<NarcDirectory, string>
                {
                    [NarcDirectory.synthOverlay] = @"data\data\weather_sys.narc",
                    [NarcDirectory.textArchives] = @"data\msgdata\msg.narc",
                },
                GameFamily.Platinum => new Dictionary<NarcDirectory, string>
                {
                    [NarcDirectory.synthOverlay] = @"data\data\weather_sys.narc",
                    [NarcDirectory.textArchives] = @"data\msgdata\" + RomData.GameVersion.ToString()[..2].ToLower() + '_' + "msg.narc"
                },
                GameFamily.HeartGoldSoulSilver or GameFamily.HgEngine => new Dictionary<NarcDirectory, string>
                {
                    [NarcDirectory.synthOverlay] = @"data\a\0\2\8",
                    [NarcDirectory.textArchives] = @"data\a\0\2\7"
                },
                _ => throw new ArgumentException($"Unrecognized GameFamily: {RomData.GameFamily}")
            };

            var directories = new Dictionary<NarcDirectory, (string packedDirectory, string unpackedDirectory)>(packedDirectories.Count);
            foreach (var kvp in packedDirectories)
            {
                directories.Add(kvp.Key, (
                    Path.Combine(RomData.WorkingDirectory, kvp.Value),
                    Path.Combine(RomData.WorkingDirectory, "unpacked", kvp.Key.ToString())
                ));
            }

            GameDirectories = directories;
            Console.WriteLine("Setting up NARC directories | Success");
        }

        #endregion Set

        #region Unpack

        public async Task<(bool Success, string ExceptionMessage)> UnpackNarcsAsync(List<NarcDirectory> narcs, IProgress<int> progress)
        {
            if (narcs.Count == 0)
            {
                progress?.Report(100);
                return (true, string.Empty);
            }

            double progressStep = 100.0 / narcs.Count;
            double currentProgress = 0;

            foreach (var narc in narcs)
            {
                var (success, exceptionMessage) = await UnpackNarcAsync(narc);
                if (!success)
                {
                    progress?.Report(100);
                    return (false, exceptionMessage);
                }

                currentProgress += progressStep;
                progress?.Report((int)currentProgress);
            }

            progress?.Report(100);
            return (true, string.Empty);
        }

        private static async Task<(bool Success, string ExceptionMessage)> UnpackNarcAsync(NarcDirectory narcPath)
        {
            try
            {
                if (!GameDirectories.TryGetValue(narcPath, out (string packedPath, string unpackedPath) paths))
                {
                    return (false, $"NARC directory not found in dictionary: {narcPath}");
                }

                DirectoryInfo directoryInfo = new(paths.unpackedPath);

                if (!directoryInfo.Exists || directoryInfo.GetFiles().Length == 0)
                {
                    Narc openedNarc = await Narc.OpenAsync(paths.packedPath);
                    if (openedNarc == null)
                    {
                        return (false, $"Failed to open NARC at path: {paths.packedPath}");
                    }

                    await openedNarc.ExtractToFolderAsync(paths.unpackedPath);

                    if (Directory.GetFiles(paths.unpackedPath).Length == 0)
                    {
                        return (false, $"Extraction failed for NARC at path: {paths.packedPath}");
                    }
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Exception occurred: {ex.Message}");
            }
        }

        #endregion Unpack
    }
}