using Dowsify.Main.Data;
using Dowsify.Main.DsUtils;
using Dowsify.Main.Enums;
using Dowsify.Main.Global;
using Dowsify.Main.Methods;
using static Dowsify.Main.Global.GlobalVariables;

namespace Dowsify.Main
{
    public partial class Mainform : Form
    {
        private DataGridView dt_HiddenItems;
        private bool IsLoadingData = false;
        private LoadingData loadingData;
        private IRomFileMethods romFileMethods;
        private bool RomLoaded = false;
        private bool UnsavedChanges = false;

        public Mainform()
        {
            InitializeComponent();
        }

        public async Task BeginUnpackNarcsAsync(IProgress<int> progress)
        {
            try
            {
                var narcs = GameFamilyNarcs.GetGameFamilyNarcs(RomData.GameFamily);

                var (success, exception) = await romFileMethods.UnpackNarcsAsync(narcs, progress);

                if (!success)
                {
                    MessageBox.Show(exception, "Unable to Unpack NARCs", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    progress?.Report(100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error during NARC unpacking: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public async Task BeginUnpackRomDataAsync()
        {
            try
            {
                var (Success, ExceptionMessage) = await romFileMethods.ExtractRomContentsAsync(RomData.WorkingDirectory, RomData.FileName);

                if (!Success)
                {
                    MessageBox.Show($"{ExceptionMessage}", "Unable to Extract ROM Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error during ROM extraction: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void ChooseRomFolder()
        {
            FolderBrowserDialog selectFolder = new()
            {
                Description = "Choose Extracted ROM Contents Folder",
                UseDescriptionForTitle = true
            };

            if (selectFolder.ShowDialog() == DialogResult.OK)
            {
                CloseProject();
                ReadRomExtractedFolder(selectFolder.SelectedPath);
            }
        }

        public async Task LoadRomDataAsync(IProgress<int> progress)
        {
            try
            {
                IsLoadingData = true;
                int progressCount = 0;
                const int totalSteps = 2;
                const int increment = 100 / totalSteps;

                void ReportProgress()
                {
                    progressCount += increment;
                    progress?.Report(progressCount);
                }

                // Check HG-Engine
                if (RomData.GameVersion == GameVersion.HeartGold && File.Exists(Overlay.OverlayFilePath(129)))
                {
                    RomData.GameVersion = GameVersion.HgEngine;
                }

                RomData.ItemNames = romFileMethods.GetItemNames();
                ReportProgress();

                RomData.HiddenItems = romFileMethods.GetHiddenItems();
                ReportProgress();

                progress?.Report(100);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ROM data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                progress?.Report(0);
            }
        }

        public void ReadRomExtractedFolder(string filePath)
        {
            Console.WriteLine("Reading ROM Contents from " + filePath);

            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Cannot load ROM header.bin." +
                       "\n\nPlease ensure you select an extracted ROM contents folder" +
                       "\nand that data is not corrupted.", "Unable to Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Cannot load ROM header.bin");
                RomLoaded = false;
                return;
            }

            string? fileName = Directory.GetFiles(filePath).SingleOrDefault(x => x.Contains(GlobalVariables.HeaderFilePath));

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Cannot load ROM header.bin." +
                    "\n\nPlease ensure you select an extracted ROM contents folder" +
                    "\nand that data is not corrupted.", "Unable to Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Cannot load ROM header.bin");
                RomLoaded = false;
                return;
            }

            ReadRomFileData(filePath + "\\", fileName);
            if (RomLoaded)
            {
                BeginExtractRomData();
                OpenLoadingDialog(LoadType.LoadRomData);
                InitializeHiddentItemTable();
                PopulateHiddenItemTable();
                EnableMenus();
            }
        }

        private void BeginExtractRomData() => OpenLoadingDialog(LoadType.UnpackNarcs);

        private void btn_OpenFolder_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            OpenRomFolder();
            IsLoadingData = false;
        }

        private void btn_SaveChanges_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            SaveChanges();
            IsLoadingData = false;
        }

        private void btn_Standardize_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            Patch_StandardizeTable();
            IsLoadingData = false;
        }

        private void CloseProject()
        {
            RomData.Reset();
            RomLoaded = false;
            UnsavedChanges = false;
            saveToolStripMenuItem.Enabled = false;
            patchesToolStripMenuItem.Enabled = false;
            btn_SaveChanges.Enabled = false;
            btn_Patches.Enabled = false;
            hiddenItemTablePanel.Controls.Remove(dt_HiddenItems);
        }

        private void dt_HiddenItems_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dt_HiddenItems.IsCurrentCellDirty)
            {
                dt_HiddenItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            UnsavedChanges = true;
        }

        private void EnableMenus()
        {
            saveToolStripMenuItem.Enabled = true;
            patchesToolStripMenuItem.Enabled = true;
            btn_SaveChanges.Enabled = true;
            btn_Patches.Enabled = true;
        }

        private void InitializeHiddentItemTable()
        {
            dt_HiddenItems = new DataGridView
            {
                Name = "dt_HiddenItems",
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dt_HiddenItems.CurrentCellDirtyStateChanged -= dt_HiddenItems_CurrentCellDirtyStateChanged;

            dt_HiddenItems.CurrentCellDirtyStateChanged += dt_HiddenItems_CurrentCellDirtyStateChanged;

            dt_HiddenItems.Columns.Clear();

            DataGridViewComboBoxColumn hiddenItem = new()
            {
                HeaderText = "Item",
                Name = "hiddenItem"
            };
            hiddenItem.Items.Clear();
            hiddenItem.Items.AddRange([.. RomData.ItemNames]);
            hiddenItem.ValueType = typeof(string);
            dt_HiddenItems.Columns.Add(hiddenItem);

            DataGridViewTextBoxColumn hiddenItemQuantity = new()
            {
                HeaderText = "Quantity",
                Name = "hiddenItemQuantity",
                ValueType = typeof(ushort)
            };
            hiddenItemQuantity.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dt_HiddenItems.Columns.Add(hiddenItemQuantity);

            DataGridViewTextBoxColumn hiddenItemIndex = new()
            {
                HeaderText = "Index",
                Name = "hiddenItemIndex",
                ValueType = typeof(ushort)
            };
            hiddenItemIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dt_HiddenItems.Columns.Add(hiddenItemIndex);

            DataGridViewTextBoxColumn commonScript = new()
            {
                HeaderText = "Common Script",
                Name = "commonScript",
                ValueType = typeof(string),
                ReadOnly = true
            };
            dt_HiddenItems.Columns.Add(commonScript);

            int totalWidth = dt_HiddenItems.Width;
            dt_HiddenItems.Columns["hiddenItem"].Width = (int)(totalWidth * 0.60);
            dt_HiddenItems.Columns["hiddenItemQuantity"].Width = (int)(totalWidth * 0.10);
            dt_HiddenItems.Columns["hiddenItemIndex"].Width = (int)(totalWidth * 0.10);
            dt_HiddenItems.Columns["commonScript"].Width = (int)(totalWidth * 0.20);

            hiddenItemTablePanel.Controls.Add(dt_HiddenItems);
        }

        private void Mainform_Shown(object sender, EventArgs e)
        {
            romFileMethods = new RomFileMethods();
        }

        private void OpenLoadingDialog(LoadType loadType)
        {
            UseWaitCursor = true;
            loadingData = new(this, loadType);
            loadingData.ShowDialog();
            UseWaitCursor = false;
        }

        private void OpenRomFolder()
        {
            if (UnsavedChanges)
            {
                var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                       "You will lose these changes if you close the project.\n" +
                       "Do you still want to close?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (saveChanges != DialogResult.Yes)
                {
                    return;
                }
            }

            ChooseRomFolder();
        }

        private void openROMFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            OpenRomFolder();
            IsLoadingData = false;
        }

        private void Patch_StandardizeTable()
        {
            var confirm = MessageBox.Show(
                "This patch will sort the Hidden Items Table by Index." +
                "\nThis means the CommonScripts will also be in consecutive order." +
                "\nHidden Item overworld events will point to different items than vanilla.\n" +
                "\nIf you are wanting a 'Vanilla Experience' and do not plan on changing hidden item locations, do not use this patch.",
                "Confirm Patch",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information
            );

            if (confirm != DialogResult.OK)
                return;

            var items = dt_HiddenItems.Rows
                .Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow)
                .Select((row, index) => new HiddenItem(
                    index,
                    (ushort)RomData.ItemNames.FindIndex(x => x == row.Cells[0].Value?.ToString()),
                    (ushort)(row.Cells[1].Value ?? 0),
                    (ushort)index
                ))
                .OrderBy(item => item.Index)
                .ToList();

            dt_HiddenItems.Rows.Clear();
            RomData.HiddenItems = items;
            PopulateHiddenItemTable();

            MessageBox.Show("Patch applied. Click save to commit changes.", "Success");
        }

        private void PopulateHiddenItemTable()
        {
            for (int i = 0; i < RomData.HiddenItems.Count; i++)
            {
                var hiddenItem = RomData.HiddenItems[i];
                string hiddenItemName = RomData.ItemNames[hiddenItem.ItemId];
                ushort quantity = hiddenItem.Quantity;
                ushort itemIndex = hiddenItem.Index;
                string commonScript = hiddenItem.CommonScript;

                dt_HiddenItems.Rows.Add(hiddenItemName, quantity, itemIndex, commonScript);
            }
        }

        private void ReadRomFileData(string workingDirectory, string fileName)
        {
            try
            {
                Console.WriteLine("Reading ROM File...");
                var (success, error) = romFileMethods.LoadInitialRomData(fileName);
                if (!success)
                {
                    MessageBox.Show(error, "Error Reading ROM File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine($"Error: {error}");
                    RomLoaded = false;
                    return;
                }
                Console.WriteLine("Load initial ROM Data | Success");
                RomData.SetupRomFile(fileName, workingDirectory);

                if (RomData.GameVersion == GameVersion.Unknown)
                {
                    const string errorMessage = "The ROM file you have selected is not supported by Dowsify." +
                                          "\n\nDowsify currently only supports Pokémon Diamond, Pearl, Platinum, " +
                                          "HeartGold or SoulSilver versions.";
                    MessageBox.Show(errorMessage, "Unsupported ROM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine("ROM version is unsupported.");
                    CloseProject();
                    return;
                }

                if (RomData.GameVersion != GameVersion.HeartGold)
                {
                    MessageBox.Show("Dowsify can only currently open HG roms (only tested on HG USA).\n" +
                        "Other ROM compatability will come soon.", "Not Yet Supported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CloseProject();
                    return;
                }
                romFileMethods.SetNarcDirectories();

                Console.WriteLine("Reading ROM File | Success");
                RomLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Error: {ex.Message}");
                RomLoaded = false;
                throw;
            }
        }

        private void SaveChanges()
        {
            var saveChanges = MessageBox.Show("Save changes to Hidden Item table?", "Save Changes", MessageBoxButtons.YesNo);
            if (saveChanges == DialogResult.Yes)
            {
                UnsavedChanges = false;
                var hiddenItems = new List<HiddenItem>();
                for (int i = 0; i < dt_HiddenItems.RowCount; i++)
                {
                    var row = dt_HiddenItems.Rows[i];
                    int tableIndex = i;
                    ushort itemId = (ushort)RomData.ItemNames.FindIndex(x => x == row.Cells[0].Value?.ToString());
                    ushort quantity = (ushort)row.Cells[1].Value;
                    ushort hiddenItemIndex = (ushort)row.Cells[2].Value;

                    hiddenItems.Add(new HiddenItem(tableIndex, itemId, quantity, hiddenItemIndex));
                }

                var (success, error) = romFileMethods.SaveChanges(hiddenItems);

                if (!success)
                {
                    MessageBox.Show("Something went wrong\n" + error);
                    Console.WriteLine("Unable to save changes " + error);
                }
                else
                {
                    RomData.HiddenItems = hiddenItems;
                    MessageBox.Show("Saved changes!", "Success");
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            SaveChanges();
            IsLoadingData = false;
        }

        private void standardizeTableIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            Patch_StandardizeTable();
            IsLoadingData = false;
        }
    }
}