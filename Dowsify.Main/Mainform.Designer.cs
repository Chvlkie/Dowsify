namespace Dowsify.Main
{
    partial class Mainform
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openROMFolderToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            importToolStripMenuItem = new ToolStripMenuItem();
            exportToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            extractNDSRomToolStripMenuItem = new ToolStripMenuItem();
            patchesToolStripMenuItem = new ToolStripMenuItem();
            standardizeTableIndexToolStripMenuItem = new ToolStripMenuItem();
            btn_OpenFolder = new ToolStripButton();
            btn_SaveChanges = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            btn_ExtractRom = new ToolStripButton();
            btn_Patches = new ToolStripDropDownButton();
            btn_Standardize = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            hiddenItemTablePanel = new Panel();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolsToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(784, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openROMFolderToolStripMenuItem, saveToolStripMenuItem, toolStripSeparator2, importToolStripMenuItem, exportToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openROMFolderToolStripMenuItem
            // 
            openROMFolderToolStripMenuItem.Image = Properties.Resources.open_ico;
            openROMFolderToolStripMenuItem.Name = "openROMFolderToolStripMenuItem";
            openROMFolderToolStripMenuItem.Size = new Size(191, 22);
            openROMFolderToolStripMenuItem.Text = "Open Extracted Folder";
            openROMFolderToolStripMenuItem.Click += openROMFolderToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Image = Properties.Resources.save_ico;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(191, 22);
            saveToolStripMenuItem.Text = "Save Changes";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(188, 6);
            // 
            // importToolStripMenuItem
            // 
            importToolStripMenuItem.Enabled = false;
            importToolStripMenuItem.Name = "importToolStripMenuItem";
            importToolStripMenuItem.Size = new Size(191, 22);
            importToolStripMenuItem.Text = "Import .json";
            importToolStripMenuItem.Click += importToolStripMenuItem_Click;
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.Enabled = false;
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.Size = new Size(191, 22);
            exportToolStripMenuItem.Text = "Export .json";
            exportToolStripMenuItem.Click += exportToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { extractNDSRomToolStripMenuItem, patchesToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // extractNDSRomToolStripMenuItem
            // 
            extractNDSRomToolStripMenuItem.Enabled = false;
            extractNDSRomToolStripMenuItem.Image = Properties.Resources.unpack_ico;
            extractNDSRomToolStripMenuItem.Name = "extractNDSRomToolStripMenuItem";
            extractNDSRomToolStripMenuItem.Size = new Size(135, 22);
            extractNDSRomToolStripMenuItem.Text = "Extract .nds";
            // 
            // patchesToolStripMenuItem
            // 
            patchesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { standardizeTableIndexToolStripMenuItem });
            patchesToolStripMenuItem.Enabled = false;
            patchesToolStripMenuItem.Image = Properties.Resources.patch_ico;
            patchesToolStripMenuItem.Name = "patchesToolStripMenuItem";
            patchesToolStripMenuItem.Size = new Size(135, 22);
            patchesToolStripMenuItem.Text = "Patches";
            // 
            // standardizeTableIndexToolStripMenuItem
            // 
            standardizeTableIndexToolStripMenuItem.Image = Properties.Resources.sort_ico;
            standardizeTableIndexToolStripMenuItem.Name = "standardizeTableIndexToolStripMenuItem";
            standardizeTableIndexToolStripMenuItem.Size = new Size(197, 22);
            standardizeTableIndexToolStripMenuItem.Text = "Standardize Table Index";
            standardizeTableIndexToolStripMenuItem.Click += standardizeTableIndexToolStripMenuItem_Click;
            // 
            // btn_OpenFolder
            // 
            btn_OpenFolder.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btn_OpenFolder.Image = Properties.Resources.open_ico;
            btn_OpenFolder.ImageTransparentColor = Color.Magenta;
            btn_OpenFolder.Name = "btn_OpenFolder";
            btn_OpenFolder.Size = new Size(23, 22);
            btn_OpenFolder.Text = "Open Extracted Folder";
            btn_OpenFolder.Click += btn_OpenFolder_Click;
            // 
            // btn_SaveChanges
            // 
            btn_SaveChanges.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btn_SaveChanges.Enabled = false;
            btn_SaveChanges.Image = Properties.Resources.save_ico;
            btn_SaveChanges.ImageTransparentColor = Color.Magenta;
            btn_SaveChanges.Name = "btn_SaveChanges";
            btn_SaveChanges.Size = new Size(23, 22);
            btn_SaveChanges.Text = "Save Changes";
            btn_SaveChanges.Click += btn_SaveChanges_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // btn_ExtractRom
            // 
            btn_ExtractRom.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btn_ExtractRom.Enabled = false;
            btn_ExtractRom.Image = Properties.Resources.unpack_ico;
            btn_ExtractRom.ImageTransparentColor = Color.Magenta;
            btn_ExtractRom.Name = "btn_ExtractRom";
            btn_ExtractRom.Size = new Size(23, 22);
            btn_ExtractRom.Text = "Extract .nds";
            // 
            // btn_Patches
            // 
            btn_Patches.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btn_Patches.DropDownItems.AddRange(new ToolStripItem[] { btn_Standardize });
            btn_Patches.Enabled = false;
            btn_Patches.Image = Properties.Resources.patch_ico;
            btn_Patches.ImageTransparentColor = Color.Magenta;
            btn_Patches.Name = "btn_Patches";
            btn_Patches.Size = new Size(29, 22);
            // 
            // btn_Standardize
            // 
            btn_Standardize.Image = Properties.Resources.sort_ico;
            btn_Standardize.Name = "btn_Standardize";
            btn_Standardize.Size = new Size(197, 22);
            btn_Standardize.Text = "Standardize Table Index";
            btn_Standardize.Click += btn_Standardize_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { btn_OpenFolder, btn_SaveChanges, toolStripSeparator1, btn_ExtractRom, btn_Patches });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(784, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // hiddenItemTablePanel
            // 
            hiddenItemTablePanel.Dock = DockStyle.Fill;
            hiddenItemTablePanel.Location = new Point(0, 49);
            hiddenItemTablePanel.Name = "hiddenItemTablePanel";
            hiddenItemTablePanel.Size = new Size(784, 362);
            hiddenItemTablePanel.TabIndex = 2;
            // 
            // Mainform
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 411);
            Controls.Add(hiddenItemTablePanel);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(800, 450);
            Name = "Mainform";
            Text = "Dowsify - Hidden Item Editor";
            Shown += Mainform_Shown;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openROMFolderToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem extractNDSRomToolStripMenuItem;
        private ToolStripMenuItem patchesToolStripMenuItem;
        private ToolStripMenuItem standardizeTableIndexToolStripMenuItem;
        private ToolStripButton btn_OpenFolder;
        private ToolStripButton btn_SaveChanges;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btn_ExtractRom;
        private ToolStripDropDownButton btn_Patches;
        private ToolStripMenuItem btn_Standardize;
        private ToolStrip toolStrip1;
        private Panel hiddenItemTablePanel;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem importToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
    }
}
