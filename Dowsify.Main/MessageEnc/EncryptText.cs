﻿using Dowsify.Main.Enums;
using System.Text;
using static Dowsify.Main.Database.Database;

namespace Dowsify.MessageEncrypt
{
    public static class EncryptText
    {
        private static readonly Dictionary<int, string> ReadTextDictionary = TextCharacters.ReadTextDictionary;
        private static readonly Dictionary<int, int> WriteTextDictionary = TextCharacters.WriteTextDictionary;

        private static string DecodeCharacter(int textChar)
        {
            if (ReadTextDictionary.TryGetValue(textChar, out var character))
            {
                return character;
            }
            else
            {
                return $"\\x{textChar:X4}";
            }
        }

        public static string DecodeMessage(BinaryReader reader, int key, int offset, int size)
        {
            bool hasSpecialCharacter = false;
            bool isCompressed = false;
            reader.BaseStream.Position = offset;
            StringBuilder decode = new("");
            for (int j = 0; j < size; j++)
            {
                int textChar = (reader.ReadUInt16()) ^ key;
                switch (textChar)
                {
                    case 0xE000:
                        decode.Append("\\n");
                        break;

                    case 0x25BC:
                        decode.Append("\\r");
                        break;

                    case 0x25BD:
                        decode.Append("\\f");
                        break;

                    case 0xF100:
                        isCompressed = true;
                        break;

                    case 0xFFFE:
                        decode.Append("\\v");
                        hasSpecialCharacter = true;
                        break;

                    case 0xFFFF:
                        decode.Append("");
                        break;

                    default:
                        if (hasSpecialCharacter)
                        {
                            decode.Append(textChar.ToString("X4"));
                            hasSpecialCharacter = false;
                        }
                        else if (isCompressed)
                        {
                            int shift = 0;
                            int trans = 0;
                            while (true)
                            {
                                int compChar = textChar >> shift;
                                if (shift >= 0xF)
                                {
                                    shift -= 0xF;
                                    if (shift > 0)
                                    {
                                        compChar = (trans | ((textChar << (9 - shift)) & 0x1FF));
                                        if ((compChar & 0xFF) == 0xFF)
                                        {
                                            break;
                                        }
                                        if (compChar != 0x0 && compChar != 0x1)
                                        {
                                            decode.Append(DecodeCharacter(compChar));
                                        }
                                    }
                                }
                                else
                                {
                                    compChar = (textChar >> shift) & 0x1FF;
                                    if ((compChar & 0xFF) == 0xFF)
                                    {
                                        break;
                                    }
                                    if (compChar != 0x0 && compChar != 0x1)
                                    {
                                        decode.Append(DecodeCharacter(compChar));
                                    }
                                    shift += 9;
                                    if (shift < 0xF)
                                    {
                                        trans = (textChar >> shift) & 0x1FF;
                                        shift += 9;
                                    }
                                    key += 0x493D;
                                    key &= 0xFFFF;
                                    textChar = Convert.ToUInt16(reader.ReadUInt16() ^ key);
                                    j++;
                                }
                            }
                            decode.Append("");
                        }
                        else
                        {
                            decode.Append(DecodeCharacter(textChar));
                        }
                        break;
                }
                key += 0x493D;
                key &= 0xFFFF;
            }
            return decode.ToString();
        }

        public static List<string> ReadMessageArchive(FileStream fileStream, bool discardLines)
        {
            int initialKey = 0;
            int stringCount = 0;
            List<string> messagesString = [];
            using (BinaryReader reader = new(fileStream))
            {
                bool success;
                try
                {
                    stringCount = reader.ReadUInt16();
                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    reader.Close();
                    fileStream.Close();
                    throw;
                }

                if (success)
                {
                    try
                    {
                        initialKey = reader.ReadUInt16();
                        if (!discardLines)
                        {
                            int[] offsets = new int[stringCount];
                            int[] sizes = new int[stringCount];
                            int key = (initialKey * 0x2FD) & 0xFFFF;

                            for (int i = 0; i < stringCount; i++)
                            {
                                int key2 = (key * (i + 1) & 0xFFFF);
                                int actualKey = key2 | (key2 << 16);
                                offsets[i] = ((int)reader.ReadUInt32()) ^ actualKey;
                                sizes[i] = ((int)reader.ReadUInt32()) ^ actualKey;
                            }
                            for (int i = 0; i < stringCount; i++)
                            {
                                key = (0x91BD3 * (i + 1)) & 0xFFFF;
                                messagesString.Add(DecodeMessage(reader, key, offsets[i], sizes[i]));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        reader.Close();
                        fileStream.Close();
                        throw;
                    }
                }
            }

            fileStream.Close();
            return messagesString;
        }

        private static int[] EncodeMessage(string message, bool isTrainerName = false)
        {
            List<int> encoded = [];
            int compressionBuffer = 0;
            int bit = 0;
            if (isTrainerName)
            {
                encoded.Add(0xF100);
            }
            var charArray = message.ToCharArray();
            string characterId;
            for (int i = 0; i < message.Length; i++)
            {
                switch (charArray[i])
                {
                    case '\\':
                        switch (charArray[i + 1])
                        {
                            case 'r':
                                encoded.Add(0x25BC);
                                i++;
                                break;

                            case 'n':
                                encoded.Add(0xE000);
                                i++;
                                break;

                            case 'f':
                                encoded.Add(0x25BD);
                                i++;
                                break;

                            case 'v':
                                encoded.Add(0xFFFE);
                                characterId = $"{charArray[i + 2]}{charArray[i + 3]}{charArray[i + 4]}{charArray[i + 5]}";
                                encoded.Add((int)Convert.ToUInt32(characterId, 16));
                                i += 5;
                                break;

                            case 'x':
                                if (charArray[i + 2] == '0' && charArray[i + 3] == '0' && charArray[i + 4] == '0' && charArray[i + 5] == '0')
                                {
                                    encoded.Add(0x0000);
                                    i += 5;
                                    break;
                                }
                                else if (charArray[i + 2] == '0' && charArray[i + 3] == '0' && charArray[i + 4] == '0' && charArray[i + 5] == '1')
                                {
                                    encoded.Add(0x0001);
                                    i += 5;
                                    break;
                                }
                                else
                                {
                                    characterId = $"{charArray[i + 2]}{charArray[i + 3]}{charArray[i + 4]}{charArray[i + 5]}";
                                    encoded.Add((int)Convert.ToUInt32(characterId, 16));
                                    i += 5;
                                    break;
                                }
                        }
                        break;

                    case '[':
                        switch (charArray[i + 1])
                        {
                            case 'P':
                                encoded.Add(0x01E0);
                                i += 3;
                                break;

                            case 'M':
                                encoded.Add(0x01E1);
                                i += 3;
                                break;
                        }
                        break;

                    default:

                        WriteTextDictionary.TryGetValue(charArray[i], out int code);
                        if (isTrainerName)
                        {
                            compressionBuffer |= code << bit;
                            bit += 9;
                            if (bit >= 15)
                            {
                                bit -= 15;
                                encoded.Add((int)Convert.ToUInt32(compressionBuffer & 0x7FFF));
                                compressionBuffer >>= 15;
                            }
                        }
                        else
                        {
                            encoded.Add(code);
                        }
                        break;
                }
            }
            if (isTrainerName && bit > 1)
            {
                compressionBuffer |= (0xFFFF << bit);
                encoded.Add((int)Convert.ToUInt32(compressionBuffer & 0x7FFF));
            }
            encoded.Add(0xFFFF);
            return [.. encoded];
        }

        private static List<int[]> EncodeMessages(List<string> messages, bool isTrainerName = false)
        {
            List<int[]> encoded = [];
            foreach (var message in messages)
            {
                encoded.Add(EncodeMessage(message, isTrainerName));
            }
            return encoded;
        }

        private static int GetInitialKey(string filePath)
        {
            int initialKey = 0;
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using (BinaryReader reader = new(fileStream))
            {
                try
                {
                    reader.BaseStream.Position = 2;
                    initialKey = reader.ReadUInt16();
                    reader.Close();
                    fileStream.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    reader.Close();
                    fileStream.Close();
                    throw;
                }
            }
            return initialKey;
        }

        public static bool WriteMessageArchive(int messageArchiveId, List<string> messages, bool isTrainerName = false)
        {
            string filePath = $"{GameDirectories[NarcDirectory.textArchives].unpackedDirectory}\\{messageArchiveId:D4}";
            int initialKey = GetInitialKey(filePath);

            // Create a memory stream for in-memory writing
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            try
            {
                // Encode the messages first
                List<int[]> encoded = EncodeMessages(messages, isTrainerName);
                int encodedSize = encoded.Count;

                // Write encoded size and initial key
                writer.Write((ushort)encodedSize);
                writer.Write((ushort)initialKey);

                // Initialize keys
                int key = (initialKey * 0x2FD) & 0xFFFF;
                int offset = 0x4 + (encodedSize * 8); // Starting offset for string lengths
                int[] stringLengths = new int[encodedSize];

                // First loop: Write offsets and lengths
                for (int i = 0; i < encodedSize; i++)
                {
                    int key2 = (key * (i + 1)) & 0xFFFF;
                    int actualKey = key2 | (key2 << 16);

                    writer.Write(offset ^ actualKey);

                    int[] currentString = encoded[i];
                    int length = currentString.Length;
                    stringLengths[i] = length;

                    writer.Write(length ^ actualKey);
                    offset += length * 2; // Length is in words (2 bytes per word)
                }

                // Second loop: Write the actual messages with key transformations
                for (int i = 0; i < encodedSize; i++)
                {
                    key = (0x91BD3 * (i + 1)) & 0xFFFF;
                    int[] currentMessage = encoded[i];

                    for (int j = 0; j < stringLengths[i] - 1; j++)
                    {
                        writer.Write((ushort)(currentMessage[j] ^ key));
                        key = (key + 0x493D) & 0xFFFF; // Ensure the key remains 16-bit
                    }

                    // Write the termination character
                    writer.Write((ushort)(0xFFFF ^ key));
                }

                // Write all bytes to the file, wiping the existing content first
                File.WriteAllBytes(filePath, stream.ToArray());
            }
            catch (Exception ex)
            {
                // Log the error and return false
                Console.WriteLine(ex.Message);
                return false;
            }

            // Return true if successful
            return true;
        }
    }
}