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
            toolsToolStripMenuItem = new ToolStripMenuItem();
            extractNDSRomToolStripMenuItem = new ToolStripMenuItem();
            patchesToolStripMenuItem = new ToolStripMenuItem();
            standardizeTableIndexToolStripMenuItem = new ToolStripMenuItem();
            btn_OpenFolder = new ToolStripButton();
            btn_SaveChanges = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            btn_ExtractRom = new ToolStripButton();
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            standardizeTableIndexToolStripMenuItem1 = new ToolStripMenuItem();
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
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openROMFolderToolStripMenuItem, saveToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openROMFolderToolStripMenuItem
            // 
            openROMFolderToolStripMenuItem.Name = "openROMFolderToolStripMenuItem";
            openROMFolderToolStripMenuItem.Size = new Size(191, 22);
            openROMFolderToolStripMenuItem.Text = "Open Extracted Folder";
            openROMFolderToolStripMenuItem.Click += openROMFolderToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(191, 22);
            saveToolStripMenuItem.Text = "Save Changes";
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
            extractNDSRomToolStripMenuItem.Name = "extractNDSRomToolStripMenuItem";
            extractNDSRomToolStripMenuItem.Size = new Size(135, 22);
            extractNDSRomToolStripMenuItem.Text = "Extract .nds";
            // 
            // patchesToolStripMenuItem
            // 
            patchesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { standardizeTableIndexToolStripMenuItem });
            patchesToolStripMenuItem.Enabled = false;
            patchesToolStripMenuItem.Name = "patchesToolStripMenuItem";
            patchesToolStripMenuItem.Size = new Size(135, 22);
            patchesToolStripMenuItem.Text = "Patches";
            // 
            // standardizeTableIndexToolStripMenuItem
            // 
            standardizeTableIndexToolStripMenuItem.Name = "standardizeTableIndexToolStripMenuItem";
            standardizeTableIndexToolStripMenuItem.Size = new Size(197, 22);
            standardizeTableIndexToolStripMenuItem.Text = "Standardize Table Index";
            // 
            // btn_OpenFolder
            // 
            btn_OpenFolder.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btn_OpenFolder.Image = (Image)resources.GetObject("btn_OpenFolder.Image");
            btn_OpenFolder.ImageTransparentColor = Color.Magenta;
            btn_OpenFolder.Name = "btn_OpenFolder";
            btn_OpenFolder.Size = new Size(23, 22);
            btn_OpenFolder.Text = "toolStripButton1";
            btn_OpenFolder.Click += btn_OpenFolder_Click;
            // 
            // btn_SaveChanges
            // 
            btn_SaveChanges.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btn_SaveChanges.Enabled = false;
            btn_SaveChanges.Image = (Image)resources.GetObject("btn_SaveChanges.Image");
            btn_SaveChanges.ImageTransparentColor = Color.Magenta;
            btn_SaveChanges.Name = "btn_SaveChanges";
            btn_SaveChanges.Size = new Size(23, 22);
            btn_SaveChanges.Text = "toolStripButton2";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // btn_ExtractRom
            // 
            btn_ExtractRom.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btn_ExtractRom.Image = (Image)resources.GetObject("btn_ExtractRom.Image");
            btn_ExtractRom.ImageTransparentColor = Color.Magenta;
            btn_ExtractRom.Name = "btn_ExtractRom";
            btn_ExtractRom.Size = new Size(23, 22);
            btn_ExtractRom.Text = "toolStripButton3";
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { standardizeTableIndexToolStripMenuItem1 });
            toolStripDropDownButton1.Enabled = false;
            toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(29, 22);
            toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // standardizeTableIndexToolStripMenuItem1
            // 
            standardizeTableIndexToolStripMenuItem1.Name = "standardizeTableIndexToolStripMenuItem1";
            standardizeTableIndexToolStripMenuItem1.Size = new Size(197, 22);
            standardizeTableIndexToolStripMenuItem1.Text = "Standardize Table Index";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { btn_OpenFolder, btn_SaveChanges, toolStripSeparator1, btn_ExtractRom, toolStripDropDownButton1 });
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
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem standardizeTableIndexToolStripMenuItem1;
        private ToolStrip toolStrip1;
        private Panel hiddenItemTablePanel;
    }
}
