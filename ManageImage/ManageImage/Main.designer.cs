namespace ManageImage
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.panel1 = new ManageImage.FormEdit.overRidePanel();
            this.dgvListArea = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableEditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbkAutoScroll = new System.Windows.Forms.CheckBox();
            this.lblMapTotalLedValue = new System.Windows.Forms.Label();
            this.lblMapTotalLed = new System.Windows.Forms.Label();
            this.lblMapHeightValue = new System.Windows.Forms.Label();
            this.lblMapHeight = new System.Windows.Forms.Label();
            this.lblMapWidthValue = new System.Windows.Forms.Label();
            this.lblMapWidth = new System.Windows.Forms.Label();
            this.trkbGridSize = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ptbEnableEdit = new System.Windows.Forms.PictureBox();
            this.ptbExport = new System.Windows.Forms.PictureBox();
            this.ptbPause = new System.Windows.Forms.PictureBox();
            this.ptbPlay = new System.Windows.Forms.PictureBox();
            this.ptbMap = new System.Windows.Forms.PictureBox();
            this.ptbDelete = new System.Windows.Forms.PictureBox();
            this.ptbSave = new System.Windows.Forms.PictureBox();
            this.ptbAddNew = new System.Windows.Forms.PictureBox();
            this.cbxLedType = new System.Windows.Forms.ComboBox();
            this.treeViewProgram = new System.Windows.Forms.TreeView();
            this.newProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListArea)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkbGridSize)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbEnableEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbExport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbPause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbPlay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbAddNew)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(755, 780);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // dgvListArea
            // 
            this.dgvListArea.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvListArea.Location = new System.Drawing.Point(16, 182);
            this.dgvListArea.Margin = new System.Windows.Forms.Padding(4);
            this.dgvListArea.Name = "dgvListArea";
            this.dgvListArea.ReadOnly = true;
            this.dgvListArea.Size = new System.Drawing.Size(356, 134);
            this.dgvListArea.TabIndex = 1;
            this.dgvListArea.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvListArea_CellDoubleClick);
            this.dgvListArea.SelectionChanged += new System.EventHandler(this.dgvListArea_SelectionChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1317, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProgramToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(228, 26);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableEditionToolStripMenuItem,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(77, 24);
            this.optionsToolStripMenuItem.Text = "Công Cụ";
            // 
            // enableEditionToolStripMenuItem
            // 
            this.enableEditionToolStripMenuItem.Enabled = false;
            this.enableEditionToolStripMenuItem.Name = "enableEditionToolStripMenuItem";
            this.enableEditionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.enableEditionToolStripMenuItem.Size = new System.Drawing.Size(241, 26);
            this.enableEditionToolStripMenuItem.Text = "Cho Phép Chỉnh Sửa";
            this.enableEditionToolStripMenuItem.Click += new System.EventHandler(this.enableEditionToolStripMenuItem_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.startToolStripMenuItem.Size = new System.Drawing.Size(241, 26);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(241, 26);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.cbkAutoScroll);
            this.groupBox1.Controls.Add(this.lblMapTotalLedValue);
            this.groupBox1.Controls.Add(this.lblMapTotalLed);
            this.groupBox1.Controls.Add(this.lblMapHeightValue);
            this.groupBox1.Controls.Add(this.lblMapHeight);
            this.groupBox1.Controls.Add(this.lblMapWidthValue);
            this.groupBox1.Controls.Add(this.lblMapWidth);
            this.groupBox1.Controls.Add(this.trkbGridSize);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(17, 642);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(355, 203);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Thông Số Màn Hình";
            // 
            // cbkAutoScroll
            // 
            this.cbkAutoScroll.AutoSize = true;
            this.cbkAutoScroll.Location = new System.Drawing.Point(179, 111);
            this.cbkAutoScroll.Margin = new System.Windows.Forms.Padding(4);
            this.cbkAutoScroll.Name = "cbkAutoScroll";
            this.cbkAutoScroll.Size = new System.Drawing.Size(127, 22);
            this.cbkAutoScroll.TabIndex = 10;
            this.cbkAutoScroll.Text = "Tự Động Cuộn";
            this.cbkAutoScroll.UseVisualStyleBackColor = true;
            // 
            // lblMapTotalLedValue
            // 
            this.lblMapTotalLedValue.AutoSize = true;
            this.lblMapTotalLedValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapTotalLedValue.Location = new System.Drawing.Point(113, 167);
            this.lblMapTotalLedValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapTotalLedValue.Name = "lblMapTotalLedValue";
            this.lblMapTotalLedValue.Size = new System.Drawing.Size(18, 20);
            this.lblMapTotalLedValue.TabIndex = 8;
            this.lblMapTotalLedValue.Text = "0";
            // 
            // lblMapTotalLed
            // 
            this.lblMapTotalLed.AutoSize = true;
            this.lblMapTotalLed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapTotalLed.Location = new System.Drawing.Point(8, 167);
            this.lblMapTotalLed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapTotalLed.Name = "lblMapTotalLed";
            this.lblMapTotalLed.Size = new System.Drawing.Size(97, 18);
            this.lblMapTotalLed.TabIndex = 7;
            this.lblMapTotalLed.Text = "Tổng Số Led:";
            // 
            // lblMapHeightValue
            // 
            this.lblMapHeightValue.AutoSize = true;
            this.lblMapHeightValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapHeightValue.Location = new System.Drawing.Point(61, 139);
            this.lblMapHeightValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapHeightValue.Name = "lblMapHeightValue";
            this.lblMapHeightValue.Size = new System.Drawing.Size(18, 20);
            this.lblMapHeightValue.TabIndex = 6;
            this.lblMapHeightValue.Text = "0";
            // 
            // lblMapHeight
            // 
            this.lblMapHeight.AutoSize = true;
            this.lblMapHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapHeight.Location = new System.Drawing.Point(8, 139);
            this.lblMapHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapHeight.Name = "lblMapHeight";
            this.lblMapHeight.Size = new System.Drawing.Size(44, 18);
            this.lblMapHeight.TabIndex = 5;
            this.lblMapHeight.Text = "Cao :";
            // 
            // lblMapWidthValue
            // 
            this.lblMapWidthValue.AutoSize = true;
            this.lblMapWidthValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapWidthValue.Location = new System.Drawing.Point(61, 111);
            this.lblMapWidthValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapWidthValue.Name = "lblMapWidthValue";
            this.lblMapWidthValue.Size = new System.Drawing.Size(18, 20);
            this.lblMapWidthValue.TabIndex = 4;
            this.lblMapWidthValue.Text = "0";
            // 
            // lblMapWidth
            // 
            this.lblMapWidth.AutoSize = true;
            this.lblMapWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapWidth.Location = new System.Drawing.Point(8, 111);
            this.lblMapWidth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMapWidth.Name = "lblMapWidth";
            this.lblMapWidth.Size = new System.Drawing.Size(48, 18);
            this.lblMapWidth.TabIndex = 3;
            this.lblMapWidth.Text = "Rộng:";
            // 
            // trkbGridSize
            // 
            this.trkbGridSize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.trkbGridSize.Location = new System.Drawing.Point(12, 52);
            this.trkbGridSize.Margin = new System.Windows.Forms.Padding(4);
            this.trkbGridSize.Maximum = 20;
            this.trkbGridSize.Minimum = 4;
            this.trkbGridSize.Name = "trkbGridSize";
            this.trkbGridSize.Size = new System.Drawing.Size(307, 56);
            this.trkbGridSize.SmallChange = 2;
            this.trkbGridSize.TabIndex = 2;
            this.trkbGridSize.TickFrequency = 2;
            this.trkbGridSize.Value = 20;
            this.trkbGridSize.ValueChanged += new System.EventHandler(this.trkbGridSize_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kích Thước Led:";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoScroll = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Location = new System.Drawing.Point(380, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(937, 843);
            this.panel2.TabIndex = 9;
            this.panel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.panel2_Scroll);
            // 
            // ptbEnableEdit
            // 
            this.ptbEnableEdit.BackgroundImage = global::ManageImage.Properties.Resources.edit_property;
            this.ptbEnableEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ptbEnableEdit.Enabled = false;
            this.ptbEnableEdit.Location = new System.Drawing.Point(92, 49);
            this.ptbEnableEdit.Margin = new System.Windows.Forms.Padding(4);
            this.ptbEnableEdit.Name = "ptbEnableEdit";
            this.ptbEnableEdit.Size = new System.Drawing.Size(64, 59);
            this.ptbEnableEdit.TabIndex = 17;
            this.ptbEnableEdit.TabStop = false;
            this.ptbEnableEdit.Click += new System.EventHandler(this.btnEnableEdit_Click);
            this.ptbEnableEdit.MouseEnter += new System.EventHandler(this.ptbEnableEdit_MouseEnter);
            this.ptbEnableEdit.MouseLeave += new System.EventHandler(this.ptbEnableEdit_MouseLeave);
            // 
            // ptbExport
            // 
            this.ptbExport.BackgroundImage = global::ManageImage.Properties.Resources.SD;
            this.ptbExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ptbExport.Location = new System.Drawing.Point(308, 49);
            this.ptbExport.Margin = new System.Windows.Forms.Padding(4);
            this.ptbExport.Name = "ptbExport";
            this.ptbExport.Size = new System.Drawing.Size(64, 59);
            this.ptbExport.TabIndex = 16;
            this.ptbExport.TabStop = false;
            this.ptbExport.Click += new System.EventHandler(this.btnExport_Click);
            this.ptbExport.MouseEnter += new System.EventHandler(this.ptbExport_MouseEnter);
            this.ptbExport.MouseLeave += new System.EventHandler(this.ptbExport_MouseLeave);
            // 
            // ptbPause
            // 
            this.ptbPause.BackgroundImage = global::ManageImage.Properties.Resources.Pause;
            this.ptbPause.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ptbPause.Location = new System.Drawing.Point(92, 116);
            this.ptbPause.Margin = new System.Windows.Forms.Padding(4);
            this.ptbPause.Name = "ptbPause";
            this.ptbPause.Size = new System.Drawing.Size(64, 59);
            this.ptbPause.TabIndex = 15;
            this.ptbPause.TabStop = false;
            this.ptbPause.Click += new System.EventHandler(this.btnStop_Click);
            this.ptbPause.MouseEnter += new System.EventHandler(this.ptbPause_MouseEnter);
            this.ptbPause.MouseLeave += new System.EventHandler(this.ptbPause_MouseLeave);
            // 
            // ptbPlay
            // 
            this.ptbPlay.BackgroundImage = global::ManageImage.Properties.Resources.Play;
            this.ptbPlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ptbPlay.Location = new System.Drawing.Point(17, 116);
            this.ptbPlay.Margin = new System.Windows.Forms.Padding(4);
            this.ptbPlay.Name = "ptbPlay";
            this.ptbPlay.Size = new System.Drawing.Size(64, 59);
            this.ptbPlay.TabIndex = 14;
            this.ptbPlay.TabStop = false;
            this.ptbPlay.Click += new System.EventHandler(this.btnPlay_Click);
            this.ptbPlay.MouseEnter += new System.EventHandler(this.ptbPlay_MouseEnter);
            this.ptbPlay.MouseLeave += new System.EventHandler(this.ptbPlay_MouseLeave);
            // 
            // ptbMap
            // 
            this.ptbMap.BackgroundImage = global::ManageImage.Properties.Resources.MAP;
            this.ptbMap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ptbMap.Location = new System.Drawing.Point(17, 46);
            this.ptbMap.Margin = new System.Windows.Forms.Padding(4);
            this.ptbMap.Name = "ptbMap";
            this.ptbMap.Size = new System.Drawing.Size(64, 59);
            this.ptbMap.TabIndex = 13;
            this.ptbMap.TabStop = false;
            this.ptbMap.Click += new System.EventHandler(this.btnEdit_Click);
            this.ptbMap.MouseEnter += new System.EventHandler(this.ptbMap_MouseEnter);
            this.ptbMap.MouseLeave += new System.EventHandler(this.ptbMap_MouseLeave);
            // 
            // ptbDelete
            // 
            this.ptbDelete.BackColor = System.Drawing.SystemColors.Control;
            this.ptbDelete.BackgroundImage = global::ManageImage.Properties.Resources.DIV;
            this.ptbDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ptbDelete.Location = new System.Drawing.Point(236, 116);
            this.ptbDelete.Margin = new System.Windows.Forms.Padding(4);
            this.ptbDelete.Name = "ptbDelete";
            this.ptbDelete.Size = new System.Drawing.Size(64, 59);
            this.ptbDelete.TabIndex = 12;
            this.ptbDelete.TabStop = false;
            this.ptbDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.ptbDelete.MouseEnter += new System.EventHandler(this.ptbDelete_MouseEnter);
            this.ptbDelete.MouseLeave += new System.EventHandler(this.ptbDelete_MouseLeave);
            // 
            // ptbSave
            // 
            this.ptbSave.BackgroundImage = global::ManageImage.Properties.Resources.OK;
            this.ptbSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ptbSave.Location = new System.Drawing.Point(308, 116);
            this.ptbSave.Margin = new System.Windows.Forms.Padding(4);
            this.ptbSave.Name = "ptbSave";
            this.ptbSave.Size = new System.Drawing.Size(64, 59);
            this.ptbSave.TabIndex = 11;
            this.ptbSave.TabStop = false;
            this.ptbSave.Click += new System.EventHandler(this.btnSave_Click);
            this.ptbSave.MouseEnter += new System.EventHandler(this.ptbSave_MouseEnter);
            this.ptbSave.MouseLeave += new System.EventHandler(this.ptbSave_MouseLeave);
            // 
            // ptbAddNew
            // 
            this.ptbAddNew.BackgroundImage = global::ManageImage.Properties.Resources.Add;
            this.ptbAddNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ptbAddNew.Location = new System.Drawing.Point(164, 116);
            this.ptbAddNew.Margin = new System.Windows.Forms.Padding(4);
            this.ptbAddNew.Name = "ptbAddNew";
            this.ptbAddNew.Size = new System.Drawing.Size(64, 59);
            this.ptbAddNew.TabIndex = 10;
            this.ptbAddNew.TabStop = false;
            this.ptbAddNew.Click += new System.EventHandler(this.btnNewArea_Click);
            this.ptbAddNew.MouseEnter += new System.EventHandler(this.ptbAddNew_MouseEnter);
            this.ptbAddNew.MouseLeave += new System.EventHandler(this.ptbAddNew_MouseLeave);
            // 
            // cbxLedType
            // 
            this.cbxLedType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxLedType.FormattingEnabled = true;
            this.cbxLedType.Items.AddRange(new object[] {
            "6803",
            "9803",
            "1903"});
            this.cbxLedType.Location = new System.Drawing.Point(164, 49);
            this.cbxLedType.Margin = new System.Windows.Forms.Padding(4);
            this.cbxLedType.Name = "cbxLedType";
            this.cbxLedType.Size = new System.Drawing.Size(135, 33);
            this.cbxLedType.TabIndex = 0;
            // 
            // treeViewProgram
            // 
            this.treeViewProgram.Location = new System.Drawing.Point(17, 337);
            this.treeViewProgram.Name = "treeViewProgram";
            this.treeViewProgram.Size = new System.Drawing.Size(355, 298);
            this.treeViewProgram.TabIndex = 18;
            this.treeViewProgram.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewProgram_AfterSelect);
            // 
            // newProgramToolStripMenuItem
            // 
            this.newProgramToolStripMenuItem.Name = "newProgramToolStripMenuItem";
            this.newProgramToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newProgramToolStripMenuItem.Size = new System.Drawing.Size(228, 26);
            this.newProgramToolStripMenuItem.Text = "New Program";
            this.newProgramToolStripMenuItem.Click += new System.EventHandler(this.newProgramToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 858);
            this.Controls.Add(this.treeViewProgram);
            this.Controls.Add(this.cbxLedType);
            this.Controls.Add(this.ptbEnableEdit);
            this.Controls.Add(this.ptbExport);
            this.Controls.Add(this.ptbPause);
            this.Controls.Add(this.ptbPlay);
            this.Controls.Add(this.ptbMap);
            this.Controls.Add(this.ptbDelete);
            this.Controls.Add(this.ptbSave);
            this.Controls.Add(this.ptbAddNew);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvListArea);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1327, 895);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Magic Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Resize += new System.EventHandler(this.Main_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgvListArea)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkbGridSize)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbEnableEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbExport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbPause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbPlay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbMap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbAddNew)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public FormEdit.overRidePanel panel1;
        private System.Windows.Forms.DataGridView dgvListArea;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trkbGridSize;
        private System.Windows.Forms.ToolStripMenuItem enableEditionToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox cbkAutoScroll;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.Label lblMapWidth;
        private System.Windows.Forms.Label lblMapWidthValue;
        private System.Windows.Forms.Label lblMapHeightValue;
        private System.Windows.Forms.Label lblMapHeight;
        private System.Windows.Forms.Label lblMapTotalLedValue;
        private System.Windows.Forms.Label lblMapTotalLed;
        private System.Windows.Forms.PictureBox ptbAddNew;
        private System.Windows.Forms.PictureBox ptbSave;
        private System.Windows.Forms.PictureBox ptbDelete;
        private System.Windows.Forms.PictureBox ptbMap;
        private System.Windows.Forms.PictureBox ptbPlay;
        private System.Windows.Forms.PictureBox ptbPause;
        private System.Windows.Forms.PictureBox ptbExport;
		private System.Windows.Forms.PictureBox ptbEnableEdit;
		private System.Windows.Forms.ComboBox cbxLedType;
        private System.Windows.Forms.TreeView treeViewProgram;
        private System.Windows.Forms.ToolStripMenuItem newProgramToolStripMenuItem;
    }
}