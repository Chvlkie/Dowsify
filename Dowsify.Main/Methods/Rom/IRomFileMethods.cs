
using Dowsify.Main.Data;
using Dowsify.Main.Enums;

namespace Dowsify.Main.Methods
{
    public interface IRomFileMethods
    {
        /// <summary>
        /// Extract the contents of opened ROM File.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<(bool Success, string ExceptionMessage)> ExtractRomContentsAsync(string workingDirectory, string fileName);
        List<HiddenItem> GetHiddenItems();


        /// <summary>
        /// Get a list of Item Names
        /// </summary>
        /// <param name="itemNameArchive"></param>
        /// <returns></returns>
        List<string> GetItemNames();

        /// <summary>
        /// Get the contents of a Message Archive for given messageArchiveId.
        /// </summary>
        /// <param name="messageArchiveId"></param>
        /// <param name="discardLines"></param>
        /// <returns></returns>
        List<MessageArchive> GetMessageArchiveContents(int messageArchiveId, bool discardLines = false);

        /// <summary>
        /// Get the Message Intial Key
        /// </summary>
        /// <param name="messageArchive"></param>
        /// <returns></returns>
        int GetMessageInitialKey(int messageArchive);

        /// <summary>
        /// Load Initial data from Rom File
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        (bool Success, string ErrorMessage) LoadInitialRomData(string filePath);
             

        /// <summary>
        /// Setup the required NarcDirectory paths for opened ROM File.
        /// </summary>
        void SetNarcDirectories();

        /// <summary>
        /// Unpack required NARCs from a ROM's Extracted data.
        /// </summary>
        /// <param name="narcs"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task<(bool Success, string ExceptionMessage)> UnpackNarcsAsync(List<NarcDirectory> narcs, IProgress<int> progress);
    }
}