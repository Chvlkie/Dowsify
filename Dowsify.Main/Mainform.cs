using Dowsify.Main.Data;
using Dowsify.Main.DsUtils;
using Dowsify.Main.Enums;
using Dowsify.Main.Global;
using Dowsify.Main.Methods;
using Newtonsoft.Json;
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
                const int totalSteps = 4;
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

                RomData.HiddenTableOffset = romFileMethods.GetHiddenItemTableOffset();
                ReportProgress();

                RomData.HiddenTableSize = romFileMethods.GetHiddenItemsTableSize();
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
            importToolStripMenuItem.Enabled = false;
            exportToolStripMenuItem.Enabled = false;
            btn_Patches.Enabled = false;
            hiddenItemTablePanel.Controls.Remove(dt_HiddenItems);
        }

        private void dt_HiddenItems_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dt_HiddenItems.IsCurrentCellDirty)
            {
                dt_HiddenItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
                UnsavedChanges = true;
            }
        }

        private void dt_HiddenItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell currentCell = dt_HiddenItems.Rows[e.RowIndex].Cells[e.ColumnIndex];
            int hiddenItemTableScriptIndex = RomData.IsPlatinum ?
                3 : 2;
            if (currentCell.ColumnIndex == 1 || currentCell.ColumnIndex == 2 || (RomData.IsPlatinum && currentCell.ColumnIndex == 3))
            {
                string newValue = currentCell.Value?.ToString();

                if (currentCell.ColumnIndex == hiddenItemTableScriptIndex)
                {
                    foreach (DataGridViewRow row in dt_HiddenItems.Rows)
                    {
                        if (row.Index != currentCell.RowIndex && row.Cells[hiddenItemTableScriptIndex].Value?.ToString() == newValue)
                        {
                            MessageBox.Show($"Duplicate value found. The value in column {hiddenItemTableScriptIndex + 1} must be unique.", "Duplicate Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            currentCell.Style.BackColor = Color.Red;
                            return;
                        }
                        else
                        {
                            currentCell.Style.BackColor = SystemColors.Window;
                        }
                    }
                }
                UpdateRomDataHiddenItems(e.RowIndex, e.ColumnIndex, newValue);
            }
        }

        private void UpdateRomDataHiddenItems(int rowIndex, int columnIndex, string newValue)
        {
            if (rowIndex >= 0 && rowIndex < RomData.HiddenItems.Count)
            {
                if (RomData.IsPlatinum)
                {
                    switch (columnIndex)
                    {
                        case 0:
                            RomData.HiddenItems[rowIndex].ItemId = (ushort)RomData.ItemNames.FindIndex(x => x == newValue);
                            break;

                        case 1:
                            RomData.HiddenItems[rowIndex].QuantityPlat = byte.Parse(newValue);
                            break;

                        case 2:
                            RomData.HiddenItems[rowIndex].RangePlat = byte.Parse(newValue);
                            break;

                        case 3:
                            RomData.HiddenItems[rowIndex].Index = ushort.Parse(newValue);
                            dt_HiddenItems.Rows[rowIndex].Cells[4].Value = RomData.HiddenItems[rowIndex].CommonScript;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    switch (columnIndex)
                    {
                        case 0:
                            RomData.HiddenItems[rowIndex].ItemId = (ushort)RomData.ItemNames.FindIndex(x => x == newValue);
                            break;

                        case 1:
                            RomData.HiddenItems[rowIndex].Quantity = ushort.Parse(newValue);
                            break;

                        case 2:
                            RomData.HiddenItems[rowIndex].Index = ushort.Parse(newValue);
                            dt_HiddenItems.Rows[rowIndex].Cells[3].Value = RomData.HiddenItems[rowIndex].CommonScript;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void dt_HiddenItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox textBox)
            {
                textBox.KeyPress -= TextBox_KeyPress_NumericOnly;

                int columnIndex = dt_HiddenItems.CurrentCell.ColumnIndex;
                if (columnIndex == 1 || columnIndex == 2)
                {
                    textBox.KeyPress += TextBox_KeyPress_NumericOnly;
                }
            }
        }

        private void EnableMenus()
        {
            saveToolStripMenuItem.Enabled = true;
            patchesToolStripMenuItem.Enabled = true;
            btn_SaveChanges.Enabled = true;
            importToolStripMenuItem.Enabled = true;
            exportToolStripMenuItem.Enabled = true;
            btn_Patches.Enabled = true;
        }

        private void SetupHeartGoldSoulSilverTable()
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
            dt_HiddenItems.EditingControlShowing -= dt_HiddenItems_EditingControlShowing;
            dt_HiddenItems.CellEndEdit -= dt_HiddenItems_CellEndEdit;

            dt_HiddenItems.CurrentCellDirtyStateChanged += dt_HiddenItems_CurrentCellDirtyStateChanged;
            dt_HiddenItems.EditingControlShowing += dt_HiddenItems_EditingControlShowing;
            dt_HiddenItems.CellEndEdit += dt_HiddenItems_CellEndEdit;

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
            commonScript.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dt_HiddenItems.Columns.Add(commonScript);

            int totalWidth = dt_HiddenItems.Width;
            dt_HiddenItems.Columns["hiddenItem"].Width = (int)(totalWidth * 0.60);
            dt_HiddenItems.Columns["hiddenItemQuantity"].Width = (int)(totalWidth * 0.10);
            dt_HiddenItems.Columns["hiddenItemIndex"].Width = (int)(totalWidth * 0.10);
            dt_HiddenItems.Columns["commonScript"].Width = (int)(totalWidth * 0.20);

            hiddenItemTablePanel.Controls.Add(dt_HiddenItems);
        }

        private void SetupPlatinumTable()
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
            dt_HiddenItems.EditingControlShowing -= dt_HiddenItems_EditingControlShowing;
            dt_HiddenItems.CellEndEdit -= dt_HiddenItems_CellEndEdit;

            dt_HiddenItems.CurrentCellDirtyStateChanged += dt_HiddenItems_CurrentCellDirtyStateChanged;
            dt_HiddenItems.EditingControlShowing += dt_HiddenItems_EditingControlShowing;
            dt_HiddenItems.CellEndEdit += dt_HiddenItems_CellEndEdit;

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
                ValueType = typeof(byte)
            };
            hiddenItemQuantity.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dt_HiddenItems.Columns.Add(hiddenItemQuantity);

            DataGridViewTextBoxColumn hiddenItemRange = new()
            {
                HeaderText = "Range",
                Name = "hiddenItemRange",
                ValueType = typeof(byte)
            };
            hiddenItemRange.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dt_HiddenItems.Columns.Add(hiddenItemRange);

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
            commonScript.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dt_HiddenItems.Columns.Add(commonScript);

            int totalWidth = dt_HiddenItems.Width;
            dt_HiddenItems.Columns["hiddenItem"].Width = (int)(totalWidth * 0.50);
            dt_HiddenItems.Columns["hiddenItemQuantity"].Width = (int)(totalWidth * 0.10);
            dt_HiddenItems.Columns["hiddenItemRange"].Width = (int)(totalWidth * 0.10);
            dt_HiddenItems.Columns["hiddenItemIndex"].Width = (int)(totalWidth * 0.10);
            dt_HiddenItems.Columns["commonScript"].Width = (int)(totalWidth * 0.20);

            hiddenItemTablePanel.Controls.Add(dt_HiddenItems);
        }

        private void InitializeHiddentItemTable()
        {
            switch (RomData.GameFamily)
            {
                case GameFamily.Platinum:
                    SetupPlatinumTable();
                    break;

                case GameFamily.HeartGoldSoulSilver:
                case GameFamily.HgEngine:
                    SetupHeartGoldSoulSilverTable();
                    break;
            }
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
            if (RomData.GameFamily == GameFamily.Platinum)
            {
                MessageBox.Show(
                               "Platinum's table is already in order.\nNo patch necessary", "No Need to Patch",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
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
        }

        public void ExportHiddenItemsToJson(List<HiddenItem> hiddenItems, string filePath)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    DefaultValueHandling = DefaultValueHandling.Ignore // Skip default values
                };

                string json = JsonConvert.SerializeObject(hiddenItems, settings);
                File.WriteAllText(filePath, json);

                Console.WriteLine($"Exported {hiddenItems.Count} hidden items to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Export failed: {ex.Message}");
                throw;
            }
        }

        public List<HiddenItem> ImportHiddenItemsFromJson(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var items = JsonConvert.DeserializeObject<List<HiddenItem>>(json);

                Console.WriteLine($"Imported {items.Count} hidden items from {filePath}");
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Import failed: {ex.Message}");
                throw;
            }
        }

        private void PopulateHiddenItemTable()
        {
            switch (RomData.GameFamily)
            {
                case GameFamily.Platinum:
                    for (int i = 0; i < RomData.HiddenItems.Count; i++)
                    {
                        var hiddenItem = RomData.HiddenItems[i];
                        string hiddenItemName = hiddenItem.ItemId <= RomData.ItemNames.Count ?
                            RomData.ItemNames[hiddenItem.ItemId] : RomData.ItemNames[0];
                        byte quantity = hiddenItem.QuantityPlat;
                        byte range = hiddenItem.RangePlat;
                        ushort itemIndex = hiddenItem.Index;
                        string commonScript = hiddenItem.CommonScript;

                        dt_HiddenItems.Rows.Add(hiddenItemName, quantity, range, itemIndex, commonScript);
                    }
                    break;

                case GameFamily.HeartGoldSoulSilver:
                case GameFamily.HgEngine:
                    for (int i = 0; i < RomData.HiddenItems.Count; i++)
                    {
                        var hiddenItem = RomData.HiddenItems[i];
                        string hiddenItemName = hiddenItem.ItemId <= RomData.ItemNames.Count ?
                            RomData.ItemNames[hiddenItem.ItemId] : RomData.ItemNames[0];
                        ushort quantity = hiddenItem.Quantity;
                        ushort itemIndex = hiddenItem.Index;
                        string commonScript = hiddenItem.CommonScript;

                        dt_HiddenItems.Rows.Add(hiddenItemName, quantity, itemIndex, commonScript);
                    }
                    break;
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
                                          "\n\nDowsify currently only supports Pok�mon Diamond, Pearl, Platinum, " +
                                          "HeartGold or SoulSilver versions.";
                    MessageBox.Show(errorMessage, "Unsupported ROM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine("ROM version is unsupported.");
                    CloseProject();
                    return;
                }

                if (RomData.GameFamily == GameFamily.DiamondPearl)
                {
                    MessageBox.Show("Dowsify can only currently open HGSS or Platinum ROMS.\n" +
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

        private void ResetCellToOriginalValue(DataGridViewCell cell)
        {
            if (cell != null && cell.RowIndex >= 0 && cell.RowIndex < RomData.HiddenItems.Count)
            {
                var originalValue = RomData.HiddenItems[cell.RowIndex].Index;
                cell.Value = originalValue;
            }
        }

        private void SaveChanges()
        {
            foreach (DataGridViewRow row in dt_HiddenItems.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Style.BackColor == Color.Red)
                    {
                        MessageBox.Show("Some cells have invalid values. Please correct them before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            var saveChanges = MessageBox.Show("Save changes to Hidden Item table?", "Save Changes", MessageBoxButtons.YesNo);
            if (saveChanges == DialogResult.Yes)
            {
                UnsavedChanges = false;
                var hiddenItems = new List<HiddenItem>();
                switch (RomData.GameFamily)
                {

                    case GameFamily.Platinum:
                        for (int i = 0; i < dt_HiddenItems.RowCount; i++)
                        {
                            var row = dt_HiddenItems.Rows[i];
                            int tableIndex = i;
                            ushort itemId = (ushort)RomData.ItemNames.FindIndex(x => x == row.Cells[0].Value?.ToString());
                            byte quantity = (byte)row.Cells[1].Value;
                            byte range = (byte)row.Cells[2].Value;
                            ushort hiddenItemIndex = (ushort)row.Cells[3].Value;

                            hiddenItems.Add(new HiddenItem(tableIndex, itemId, quantity, range, hiddenItemIndex));
                        }
                        break;
                    case GameFamily.HeartGoldSoulSilver:
                    case GameFamily.HgEngine:
                        for (int i = 0; i < dt_HiddenItems.RowCount; i++)
                        {
                            var row = dt_HiddenItems.Rows[i];
                            int tableIndex = i;
                            ushort itemId = (ushort)RomData.ItemNames.FindIndex(x => x == row.Cells[0].Value?.ToString());
                            ushort quantity = (ushort)row.Cells[1].Value;
                            ushort hiddenItemIndex = (ushort)row.Cells[2].Value;

                            hiddenItems.Add(new HiddenItem(tableIndex, itemId, quantity, hiddenItemIndex));
                        }
                        break;
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

        private void TextBox_KeyPress_NumericOnly(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "JSON Files (*.json)|*.json";
                openDialog.Title = "Import Hidden Items";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        RomData.HiddenItems = ImportHiddenItemsFromJson(openDialog.FileName);
                        MessageBox.Show($"Imported {RomData.HiddenItems.Count} items.", "Success");
                        RefreshHiddenItems();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Import failed: {ex.Message}", "Error");
                    }
                }
            }
        }

        public void RefreshHiddenItems()
        {
            dt_HiddenItems.Rows.Clear();
            PopulateHiddenItemTable();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "JSON Files (*.json)|*.json";
                saveDialog.Title = "Export Hidden Items";
                saveDialog.DefaultExt = "json";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportHiddenItemsToJson(RomData.HiddenItems, saveDialog.FileName);
                }
            }
        }
    }
}