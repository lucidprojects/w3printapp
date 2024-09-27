namespace PrintInvoice
{
  partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.invoiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miFindInvoice = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miFindStartIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.tpTemplate = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel6 = new System.Windows.Forms.Panel();
            this.dgvSubset = new System.Windows.Forms.DataGridView();
            this.cmsSubset = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miRemoveFromSubset = new System.Windows.Forms.ToolStripMenuItem();
            this.miSubsetAddToUnship = new System.Windows.Forms.ToolStripMenuItem();
            this.miPreviewSubsetInvoice = new System.Windows.Forms.ToolStripMenuItem();
            this.miPreviewSubsetInvoiceWithSequenceNumber = new System.Windows.Forms.ToolStripMenuItem();
            this.previewPackJacketInvoiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miSubsetCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.cbSubset = new System.Windows.Forms.ComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ssSubset = new System.Windows.Forms.StatusStrip();
            this.tsslSubsetTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSubsetUnprinted = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSubsetPrinted = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSubsetFailed = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSubsetLocked = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkPrintSequenceNumber = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btExportErrors = new System.Windows.Forms.Button();
            this.cbPrinter = new System.Windows.Forms.ComboBox();
            this.btPrint = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.dgvQuery = new System.Windows.Forms.DataGridView();
            this.cmsSet = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToCustomSubsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miPreviewSetInvoice = new System.Windows.Forms.ToolStripMenuItem();
            this.miSetCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.cbQueryShowLocked = new System.Windows.Forms.CheckBox();
            this.cbQueryShowError = new System.Windows.Forms.CheckBox();
            this.cbQueryShowPrinted = new System.Windows.Forms.CheckBox();
            this.cbQueryShowUnprinted = new System.Windows.Forms.CheckBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.ssSet = new System.Windows.Forms.StatusStrip();
            this.tsslSetTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSetUnprinted = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSetPrinted = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSetFailed = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSetLocked = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.btReload = new System.Windows.Forms.Button();
            this.panel21 = new System.Windows.Forms.Panel();
            this.tbHelp = new System.Windows.Forms.TextBox();
            this.sfdExportErrors = new System.Windows.Forms.SaveFileDialog();
            this.tcQueries = new System.Windows.Forms.TabControl();
            this.cmsRepair = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miRepairCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tpUnship = new System.Windows.Forms.TabPage();
            this.panel9 = new System.Windows.Forms.Panel();
            this.dgvUnship = new System.Windows.Forms.DataGridView();
            this.cmsUnship = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miUnshipRemoveSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.miUnshipCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.panel12 = new System.Windows.Forms.Panel();
            this.ssUnship = new System.Windows.Forms.StatusStrip();
            this.tsslUnshipTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslUnshipShipped = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslUnshipUnshipped = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslUnshipFailed = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.chkUnshipFilterShowFailed = new System.Windows.Forms.CheckBox();
            this.chkUnshipFilterShowUnshipped = new System.Windows.Forms.CheckBox();
            this.chkUnshipFilterShowShipped = new System.Windows.Forms.CheckBox();
            this.panel10 = new System.Windows.Forms.Panel();
            this.btUnshipLastBatches = new System.Windows.Forms.Button();
            this.btUnshipClear = new System.Windows.Forms.Button();
            this.btExportUnshipErrors = new System.Windows.Forms.Button();
            this.btUnship = new System.Windows.Forms.Button();
            this.btAddUnshipInvoice = new System.Windows.Forms.Button();
            this.panel23 = new System.Windows.Forms.Panel();
            this.tbHelpUnship = new System.Windows.Forms.TextBox();
            this.tpUnshipped = new System.Windows.Forms.TabPage();
            this.panel13 = new System.Windows.Forms.Panel();
            this.dgvUnshipped = new System.Windows.Forms.DataGridView();
            this.cmsUnshipped = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miUnshippedCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.panel15 = new System.Windows.Forms.Panel();
            this.chkUnshippedShowNonupdated = new System.Windows.Forms.CheckBox();
            this.chkUnshippedShowFailed = new System.Windows.Forms.CheckBox();
            this.chkUnshippedShowUpdated = new System.Windows.Forms.CheckBox();
            this.chkUnshippedShowUnshipped = new System.Windows.Forms.CheckBox();
            this.chkUnshippedShowNew = new System.Windows.Forms.CheckBox();
            this.panel14 = new System.Windows.Forms.Panel();
            this.ssUnshipped = new System.Windows.Forms.StatusStrip();
            this.tsslUnshippedTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslNonupdated = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslUnshippedUpdated = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslUnshippedFailed = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel16 = new System.Windows.Forms.Panel();
            this.btOnHold = new System.Windows.Forms.Button();
            this.btSetPmodMaxDailyPackages = new System.Windows.Forms.Button();
            this.edPmodMaxDailyPackages = new System.Windows.Forms.TextBox();
            this.btUnshippedResetStatus = new System.Windows.Forms.Button();
            this.btUnshippedClearSelection = new System.Windows.Forms.Button();
            this.btUnshippedSelectAll = new System.Windows.Forms.Button();
            this.btUnshippedSetPickable = new System.Windows.Forms.Button();
            this.btReloarUnshipped = new System.Windows.Forms.Button();
            this.panel22 = new System.Windows.Forms.Panel();
            this.tbHelpUnshipped = new System.Windows.Forms.TextBox();
            this.tpReprint = new System.Windows.Forms.TabPage();
            this.panel17 = new System.Windows.Forms.Panel();
            this.dgvReprint = new System.Windows.Forms.DataGridView();
            this.cmsReprint = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiReprintRemoveSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.miPreviewReprintInvoice = new System.Windows.Forms.ToolStripMenuItem();
            this.miReprintCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.panel18 = new System.Windows.Forms.Panel();
            this.ssReprint = new System.Windows.Forms.StatusStrip();
            this.tsslReprintTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslReprintUnprinted = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslReprintPrinted = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslReprintFailed = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslReprintLocked = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel19 = new System.Windows.Forms.Panel();
            this.chkReprintShowLocked = new System.Windows.Forms.CheckBox();
            this.chkReprintShowFailed = new System.Windows.Forms.CheckBox();
            this.chkReprintShowPrinted = new System.Windows.Forms.CheckBox();
            this.chkReprintShowUnprinted = new System.Windows.Forms.CheckBox();
            this.panel20 = new System.Windows.Forms.Panel();
            this.chkReprintSequenceNumber = new System.Windows.Forms.CheckBox();
            this.btReprintLastBatches = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbReprintPrinter = new System.Windows.Forms.ComboBox();
            this.btReprintExportErrors = new System.Windows.Forms.Button();
            this.btReprintPrint = new System.Windows.Forms.Button();
            this.btReprintClear = new System.Windows.Forms.Button();
            this.btAddReprintPackage = new System.Windows.Forms.Button();
            this.panel24 = new System.Windows.Forms.Panel();
            this.tbHelpReprint = new System.Windows.Forms.TextBox();
            this.tpRepair = new System.Windows.Forms.TabPage();
            this.panel25 = new System.Windows.Forms.Panel();
            this.dgvRepair = new System.Windows.Forms.DataGridView();
            this.panel26 = new System.Windows.Forms.Panel();
            this.ssRepair = new System.Windows.Forms.StatusStrip();
            this.tsslRepairTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslRepairNonrepaired = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslRepairRepaired = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel27 = new System.Windows.Forms.Panel();
            this.chkRepairShowRepaired = new System.Windows.Forms.CheckBox();
            this.chkRepairShowNonrepaired = new System.Windows.Forms.CheckBox();
            this.panel28 = new System.Windows.Forms.Panel();
            this.btRepairExportPackagesErrors = new System.Windows.Forms.Button();
            this.btRepairExportErrors = new System.Windows.Forms.Button();
            this.btRepair = new System.Windows.Forms.Button();
            this.btRepairClearSelection = new System.Windows.Forms.Button();
            this.btRepairSelectAll = new System.Windows.Forms.Button();
            this.btRepairReload = new System.Windows.Forms.Button();
            this.panel29 = new System.Windows.Forms.Panel();
            this.tbHelpRepair = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.tpTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubset)).BeginInit();
            this.cmsSubset.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.ssSubset.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuery)).BeginInit();
            this.cmsSet.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.ssSet.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel21.SuspendLayout();
            this.tcQueries.SuspendLayout();
            this.cmsRepair.SuspendLayout();
            this.tpUnship.SuspendLayout();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnship)).BeginInit();
            this.cmsUnship.SuspendLayout();
            this.panel12.SuspendLayout();
            this.ssUnship.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel23.SuspendLayout();
            this.tpUnshipped.SuspendLayout();
            this.panel13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnshipped)).BeginInit();
            this.cmsUnshipped.SuspendLayout();
            this.panel15.SuspendLayout();
            this.panel14.SuspendLayout();
            this.ssUnshipped.SuspendLayout();
            this.panel16.SuspendLayout();
            this.panel22.SuspendLayout();
            this.tpReprint.SuspendLayout();
            this.panel17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReprint)).BeginInit();
            this.cmsReprint.SuspendLayout();
            this.panel18.SuspendLayout();
            this.ssReprint.SuspendLayout();
            this.panel19.SuspendLayout();
            this.panel20.SuspendLayout();
            this.panel24.SuspendLayout();
            this.tpRepair.SuspendLayout();
            this.panel25.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRepair)).BeginInit();
            this.panel26.SuspendLayout();
            this.ssRepair.SuspendLayout();
            this.panel27.SuspendLayout();
            this.panel28.SuspendLayout();
            this.panel29.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.invoiceToolStripMenuItem,
            this.printToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1184, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // miFileExit
            // 
            this.miFileExit.Name = "miFileExit";
            this.miFileExit.Size = new System.Drawing.Size(93, 22);
            this.miFileExit.Text = "Exit";
            this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
            // 
            // invoiceToolStripMenuItem
            // 
            this.invoiceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFindInvoice});
            this.invoiceToolStripMenuItem.Name = "invoiceToolStripMenuItem";
            this.invoiceToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.invoiceToolStripMenuItem.Text = "Invoice";
            // 
            // miFindInvoice
            // 
            this.miFindInvoice.Name = "miFindInvoice";
            this.miFindInvoice.Size = new System.Drawing.Size(109, 22);
            this.miFindInvoice.Text = "Find ...";
            this.miFindInvoice.Click += new System.EventHandler(this.miFindInvoice_Click);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFindStartIndex});
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.printToolStripMenuItem.Text = "Print";
            // 
            // miFindStartIndex
            // 
            this.miFindStartIndex.Name = "miFindStartIndex";
            this.miFindStartIndex.Size = new System.Drawing.Size(167, 22);
            this.miFindStartIndex.Text = "Find start index ...";
            this.miFindStartIndex.Click += new System.EventHandler(this.miFindStartIndex_Click);
            // 
            // tpTemplate
            // 
            this.tpTemplate.Controls.Add(this.splitContainer1);
            this.tpTemplate.Controls.Add(this.panel21);
            this.tpTemplate.Location = new System.Drawing.Point(4, 28);
            this.tpTemplate.Name = "tpTemplate";
            this.tpTemplate.Padding = new System.Windows.Forms.Padding(3);
            this.tpTemplate.Size = new System.Drawing.Size(1176, 505);
            this.tpTemplate.TabIndex = 0;
            this.tpTemplate.Text = "Template Tab Page";
            this.tpTemplate.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 65);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel6);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel8);
            this.splitContainer1.Panel2.Controls.Add(this.panel7);
            this.splitContainer1.Size = new System.Drawing.Size(1170, 437);
            this.splitContainer1.SplitterDistance = 236;
            this.splitContainer1.TabIndex = 1;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.dgvSubset);
            this.panel6.Controls.Add(this.panel2);
            this.panel6.Controls.Add(this.panel3);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(968, 234);
            this.panel6.TabIndex = 5;
            // 
            // dgvSubset
            // 
            this.dgvSubset.AllowUserToAddRows = false;
            this.dgvSubset.AllowUserToDeleteRows = false;
            this.dgvSubset.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSubset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSubset.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubset.ContextMenuStrip = this.cmsSubset;
            this.dgvSubset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSubset.Location = new System.Drawing.Point(0, 49);
            this.dgvSubset.Name = "dgvSubset";
            this.dgvSubset.ReadOnly = true;
            this.dgvSubset.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSubset.Size = new System.Drawing.Size(968, 160);
            this.dgvSubset.TabIndex = 3;
            this.dgvSubset.Sorted += new System.EventHandler(this.dgvSubset_Sorted);
            // 
            // cmsSubset
            // 
            this.cmsSubset.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRemoveFromSubset,
            this.miSubsetAddToUnship,
            this.miPreviewSubsetInvoice,
            this.miPreviewSubsetInvoiceWithSequenceNumber,
            this.previewPackJacketInvoiceToolStripMenuItem,
            this.previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem,
            this.miSubsetCopyToClipboard});
            this.cmsSubset.Name = "cmsSubset";
            this.cmsSubset.Size = new System.Drawing.Size(344, 158);
            this.cmsSubset.Opening += new System.ComponentModel.CancelEventHandler(this.cmsSubset_Opening);
            // 
            // miRemoveFromSubset
            // 
            this.miRemoveFromSubset.Name = "miRemoveFromSubset";
            this.miRemoveFromSubset.Size = new System.Drawing.Size(343, 22);
            this.miRemoveFromSubset.Text = "Remove Selected";
            this.miRemoveFromSubset.Click += new System.EventHandler(this.miRemoveFromSubset_Click);
            // 
            // miSubsetAddToUnship
            // 
            this.miSubsetAddToUnship.Name = "miSubsetAddToUnship";
            this.miSubsetAddToUnship.Size = new System.Drawing.Size(343, 22);
            this.miSubsetAddToUnship.Text = "Add Selected to Unship List";
            this.miSubsetAddToUnship.Click += new System.EventHandler(this.miSubsetAddToUnship_Click);
            // 
            // miPreviewSubsetInvoice
            // 
            this.miPreviewSubsetInvoice.Name = "miPreviewSubsetInvoice";
            this.miPreviewSubsetInvoice.Size = new System.Drawing.Size(343, 22);
            this.miPreviewSubsetInvoice.Text = "Preview Invoice";
            this.miPreviewSubsetInvoice.Click += new System.EventHandler(this.miPreviewSubsetInvoice_Click);
            // 
            // miPreviewSubsetInvoiceWithSequenceNumber
            // 
            this.miPreviewSubsetInvoiceWithSequenceNumber.Name = "miPreviewSubsetInvoiceWithSequenceNumber";
            this.miPreviewSubsetInvoiceWithSequenceNumber.Size = new System.Drawing.Size(343, 22);
            this.miPreviewSubsetInvoiceWithSequenceNumber.Text = "Preview Invoice With Sequence Number";
            this.miPreviewSubsetInvoiceWithSequenceNumber.Click += new System.EventHandler(this.miPreviewSubsetInvoiceWithSequenceNumber_Click);
            // 
            // previewPackJacketInvoiceToolStripMenuItem
            // 
            this.previewPackJacketInvoiceToolStripMenuItem.Name = "previewPackJacketInvoiceToolStripMenuItem";
            this.previewPackJacketInvoiceToolStripMenuItem.Size = new System.Drawing.Size(343, 22);
            this.previewPackJacketInvoiceToolStripMenuItem.Text = "Preview Pack Jacket invoice";
            this.previewPackJacketInvoiceToolStripMenuItem.Click += new System.EventHandler(this.previewPackJacketInvoiceToolStripMenuItem_Click);
            // 
            // previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem
            // 
            this.previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem.Name = "previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem";
            this.previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem.Size = new System.Drawing.Size(343, 22);
            this.previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem.Text = "Preview Pack Jacket invoice with sequence number";
            this.previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem.Click += new System.EventHandler(this.previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem_Click);
            // 
            // miSubsetCopyToClipboard
            // 
            this.miSubsetCopyToClipboard.Name = "miSubsetCopyToClipboard";
            this.miSubsetCopyToClipboard.Size = new System.Drawing.Size(343, 22);
            this.miSubsetCopyToClipboard.Text = "Copy Field Value To Clipboard";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.cbSubset);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(968, 49);
            this.panel2.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.Navy;
            this.label4.Location = new System.Drawing.Point(4, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Invoices to be printed";
            // 
            // cbSubset
            // 
            this.cbSubset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubset.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSubset.FormattingEnabled = true;
            this.cbSubset.Location = new System.Drawing.Point(7, 21);
            this.cbSubset.Name = "cbSubset";
            this.cbSubset.Size = new System.Drawing.Size(220, 23);
            this.cbSubset.TabIndex = 0;
            this.cbSubset.SelectedIndexChanged += new System.EventHandler(this.cbSubqueries_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.ssSubset);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 209);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(968, 25);
            this.panel3.TabIndex = 1;
            // 
            // ssSubset
            // 
            this.ssSubset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssSubset.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslSubsetTotal,
            this.tsslSubsetUnprinted,
            this.tsslSubsetPrinted,
            this.tsslSubsetFailed,
            this.tsslSubsetLocked});
            this.ssSubset.Location = new System.Drawing.Point(0, 0);
            this.ssSubset.Name = "ssSubset";
            this.ssSubset.Size = new System.Drawing.Size(968, 25);
            this.ssSubset.SizingGrip = false;
            this.ssSubset.TabIndex = 0;
            // 
            // tsslSubsetTotal
            // 
            this.tsslSubsetTotal.Name = "tsslSubsetTotal";
            this.tsslSubsetTotal.Size = new System.Drawing.Size(118, 20);
            this.tsslSubsetTotal.Text = "toolStripStatusLabel1";
            // 
            // tsslSubsetUnprinted
            // 
            this.tsslSubsetUnprinted.Name = "tsslSubsetUnprinted";
            this.tsslSubsetUnprinted.Size = new System.Drawing.Size(118, 20);
            this.tsslSubsetUnprinted.Text = "toolStripStatusLabel1";
            // 
            // tsslSubsetPrinted
            // 
            this.tsslSubsetPrinted.Name = "tsslSubsetPrinted";
            this.tsslSubsetPrinted.Size = new System.Drawing.Size(118, 20);
            this.tsslSubsetPrinted.Text = "toolStripStatusLabel1";
            // 
            // tsslSubsetFailed
            // 
            this.tsslSubsetFailed.Name = "tsslSubsetFailed";
            this.tsslSubsetFailed.Size = new System.Drawing.Size(118, 20);
            this.tsslSubsetFailed.Text = "toolStripStatusLabel1";
            // 
            // tsslSubsetLocked
            // 
            this.tsslSubsetLocked.Name = "tsslSubsetLocked";
            this.tsslSubsetLocked.Size = new System.Drawing.Size(118, 20);
            this.tsslSubsetLocked.Text = "toolStripStatusLabel1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.chkPrintSequenceNumber);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btExportErrors);
            this.panel1.Controls.Add(this.cbPrinter);
            this.panel1.Controls.Add(this.btPrint);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(968, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 234);
            this.panel1.TabIndex = 4;
            // 
            // chkPrintSequenceNumber
            // 
            this.chkPrintSequenceNumber.AutoSize = true;
            this.chkPrintSequenceNumber.Location = new System.Drawing.Point(11, 83);
            this.chkPrintSequenceNumber.Name = "chkPrintSequenceNumber";
            this.chkPrintSequenceNumber.Size = new System.Drawing.Size(186, 17);
            this.chkPrintSequenceNumber.TabIndex = 8;
            this.chkPrintSequenceNumber.Text = "Print Invoice Sequence Number";
            this.chkPrintSequenceNumber.UseVisualStyleBackColor = true;
            this.chkPrintSequenceNumber.Click += new System.EventHandler(this.chkPrintSequenceNumber_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Printer";
            // 
            // btExportErrors
            // 
            this.btExportErrors.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btExportErrors.Location = new System.Drawing.Point(11, 147);
            this.btExportErrors.Name = "btExportErrors";
            this.btExportErrors.Size = new System.Drawing.Size(180, 35);
            this.btExportErrors.TabIndex = 7;
            this.btExportErrors.Text = "Export errors";
            this.btExportErrors.UseVisualStyleBackColor = true;
            this.btExportErrors.Click += new System.EventHandler(this.btExportErrors_Click);
            // 
            // cbPrinter
            // 
            this.cbPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrinter.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPrinter.FormattingEnabled = true;
            this.cbPrinter.Location = new System.Drawing.Point(11, 21);
            this.cbPrinter.Name = "cbPrinter";
            this.cbPrinter.Size = new System.Drawing.Size(180, 21);
            this.cbPrinter.TabIndex = 4;
            // 
            // btPrint
            // 
            this.btPrint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btPrint.Location = new System.Drawing.Point(11, 106);
            this.btPrint.Name = "btPrint";
            this.btPrint.Size = new System.Drawing.Size(180, 35);
            this.btPrint.TabIndex = 6;
            this.btPrint.Text = "Print";
            this.btPrint.UseVisualStyleBackColor = true;
            this.btPrint.Click += new System.EventHandler(this.btPrint_Click);
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.dgvQuery);
            this.panel8.Controls.Add(this.panel4);
            this.panel8.Controls.Add(this.panel5);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(968, 195);
            this.panel8.TabIndex = 6;
            // 
            // dgvQuery
            // 
            this.dgvQuery.AllowUserToAddRows = false;
            this.dgvQuery.AllowUserToDeleteRows = false;
            this.dgvQuery.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvQuery.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvQuery.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvQuery.ContextMenuStrip = this.cmsSet;
            this.dgvQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvQuery.Location = new System.Drawing.Point(0, 42);
            this.dgvQuery.Name = "dgvQuery";
            this.dgvQuery.ReadOnly = true;
            this.dgvQuery.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQuery.Size = new System.Drawing.Size(968, 125);
            this.dgvQuery.TabIndex = 2;
            // 
            // cmsSet
            // 
            this.cmsSet.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToCustomSubsetToolStripMenuItem,
            this.miPreviewSetInvoice,
            this.miSetCopyToClipboard});
            this.cmsSet.Name = "cmsSubset";
            this.cmsSet.Size = new System.Drawing.Size(173, 70);
            this.cmsSet.Opening += new System.ComponentModel.CancelEventHandler(this.cmsSet_Opening);
            // 
            // addToCustomSubsetToolStripMenuItem
            // 
            this.addToCustomSubsetToolStripMenuItem.Name = "addToCustomSubsetToolStripMenuItem";
            this.addToCustomSubsetToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.addToCustomSubsetToolStripMenuItem.Text = "Add to Custom";
            this.addToCustomSubsetToolStripMenuItem.Click += new System.EventHandler(this.addToCustomSubsetToolStripMenuItem_Click);
            // 
            // miPreviewSetInvoice
            // 
            this.miPreviewSetInvoice.Name = "miPreviewSetInvoice";
            this.miPreviewSetInvoice.Size = new System.Drawing.Size(172, 22);
            this.miPreviewSetInvoice.Text = "Preview Invoice";
            this.miPreviewSetInvoice.Click += new System.EventHandler(this.miPreviewSetInvoice_Click);
            // 
            // miSetCopyToClipboard
            // 
            this.miSetCopyToClipboard.Name = "miSetCopyToClipboard";
            this.miSetCopyToClipboard.Size = new System.Drawing.Size(172, 22);
            this.miSetCopyToClipboard.Text = "Copy To Clipboard";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.cbQueryShowLocked);
            this.panel4.Controls.Add(this.cbQueryShowError);
            this.panel4.Controls.Add(this.cbQueryShowPrinted);
            this.panel4.Controls.Add(this.cbQueryShowUnprinted);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(968, 42);
            this.panel4.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.Color.Navy;
            this.label5.Location = new System.Drawing.Point(4, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "All invoices";
            // 
            // cbQueryShowLocked
            // 
            this.cbQueryShowLocked.AutoSize = true;
            this.cbQueryShowLocked.Checked = true;
            this.cbQueryShowLocked.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbQueryShowLocked.Location = new System.Drawing.Point(320, 21);
            this.cbQueryShowLocked.Name = "cbQueryShowLocked";
            this.cbQueryShowLocked.Size = new System.Drawing.Size(94, 17);
            this.cbQueryShowLocked.TabIndex = 3;
            this.cbQueryShowLocked.Text = "Show Locked";
            this.cbQueryShowLocked.UseVisualStyleBackColor = true;
            this.cbQueryShowLocked.CheckedChanged += new System.EventHandler(this.cbQueryShowLocked_CheckedChanged);
            // 
            // cbQueryShowError
            // 
            this.cbQueryShowError.AutoSize = true;
            this.cbQueryShowError.Checked = true;
            this.cbQueryShowError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbQueryShowError.Location = new System.Drawing.Point(225, 21);
            this.cbQueryShowError.Name = "cbQueryShowError";
            this.cbQueryShowError.Size = new System.Drawing.Size(89, 17);
            this.cbQueryShowError.TabIndex = 2;
            this.cbQueryShowError.Text = "Show Failed";
            this.cbQueryShowError.UseVisualStyleBackColor = true;
            this.cbQueryShowError.CheckedChanged += new System.EventHandler(this.cbQueryShowError_CheckedChanged);
            // 
            // cbQueryShowPrinted
            // 
            this.cbQueryShowPrinted.AutoSize = true;
            this.cbQueryShowPrinted.Checked = true;
            this.cbQueryShowPrinted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbQueryShowPrinted.Location = new System.Drawing.Point(124, 21);
            this.cbQueryShowPrinted.Name = "cbQueryShowPrinted";
            this.cbQueryShowPrinted.Size = new System.Drawing.Size(95, 17);
            this.cbQueryShowPrinted.TabIndex = 1;
            this.cbQueryShowPrinted.Text = "Show Printed";
            this.cbQueryShowPrinted.UseVisualStyleBackColor = true;
            this.cbQueryShowPrinted.CheckedChanged += new System.EventHandler(this.cbQueryShowPrinted_CheckedChanged);
            // 
            // cbQueryShowUnprinted
            // 
            this.cbQueryShowUnprinted.AutoSize = true;
            this.cbQueryShowUnprinted.Checked = true;
            this.cbQueryShowUnprinted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbQueryShowUnprinted.Location = new System.Drawing.Point(7, 21);
            this.cbQueryShowUnprinted.Name = "cbQueryShowUnprinted";
            this.cbQueryShowUnprinted.Size = new System.Drawing.Size(111, 17);
            this.cbQueryShowUnprinted.TabIndex = 0;
            this.cbQueryShowUnprinted.Text = "Show Unprinted";
            this.cbQueryShowUnprinted.UseVisualStyleBackColor = true;
            this.cbQueryShowUnprinted.CheckedChanged += new System.EventHandler(this.cbQueryShowUnprinted_CheckedChanged);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.Control;
            this.panel5.Controls.Add(this.ssSet);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 167);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(968, 28);
            this.panel5.TabIndex = 1;
            // 
            // ssSet
            // 
            this.ssSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssSet.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslSetTotal,
            this.tsslSetUnprinted,
            this.tsslSetPrinted,
            this.tsslSetFailed,
            this.tsslSetLocked});
            this.ssSet.Location = new System.Drawing.Point(0, 0);
            this.ssSet.Name = "ssSet";
            this.ssSet.Size = new System.Drawing.Size(968, 28);
            this.ssSet.SizingGrip = false;
            this.ssSet.TabIndex = 0;
            this.ssSet.Text = "statusStrip1";
            // 
            // tsslSetTotal
            // 
            this.tsslSetTotal.Name = "tsslSetTotal";
            this.tsslSetTotal.Size = new System.Drawing.Size(118, 23);
            this.tsslSetTotal.Text = "toolStripStatusLabel1";
            // 
            // tsslSetUnprinted
            // 
            this.tsslSetUnprinted.Name = "tsslSetUnprinted";
            this.tsslSetUnprinted.Size = new System.Drawing.Size(118, 23);
            this.tsslSetUnprinted.Text = "toolStripStatusLabel1";
            // 
            // tsslSetPrinted
            // 
            this.tsslSetPrinted.Name = "tsslSetPrinted";
            this.tsslSetPrinted.Size = new System.Drawing.Size(118, 23);
            this.tsslSetPrinted.Text = "toolStripStatusLabel1";
            // 
            // tsslSetFailed
            // 
            this.tsslSetFailed.Name = "tsslSetFailed";
            this.tsslSetFailed.Size = new System.Drawing.Size(118, 23);
            this.tsslSetFailed.Text = "toolStripStatusLabel1";
            // 
            // tsslSetLocked
            // 
            this.tsslSetLocked.Name = "tsslSetLocked";
            this.tsslSetLocked.Size = new System.Drawing.Size(118, 23);
            this.tsslSetLocked.Text = "toolStripStatusLabel1";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.SystemColors.Control;
            this.panel7.Controls.Add(this.btReload);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel7.Location = new System.Drawing.Point(968, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(200, 195);
            this.panel7.TabIndex = 5;
            // 
            // btReload
            // 
            this.btReload.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btReload.Location = new System.Drawing.Point(11, 42);
            this.btReload.Name = "btReload";
            this.btReload.Size = new System.Drawing.Size(180, 35);
            this.btReload.TabIndex = 7;
            this.btReload.Text = "Reload";
            this.btReload.UseVisualStyleBackColor = true;
            this.btReload.Click += new System.EventHandler(this.btReload_Click);
            // 
            // panel21
            // 
            this.panel21.Controls.Add(this.tbHelp);
            this.panel21.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel21.Location = new System.Drawing.Point(3, 3);
            this.panel21.Name = "panel21";
            this.panel21.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel21.Size = new System.Drawing.Size(1170, 62);
            this.panel21.TabIndex = 2;
            // 
            // tbHelp
            // 
            this.tbHelp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHelp.Location = new System.Drawing.Point(0, 0);
            this.tbHelp.Multiline = true;
            this.tbHelp.Name = "tbHelp";
            this.tbHelp.ReadOnly = true;
            this.tbHelp.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbHelp.Size = new System.Drawing.Size(1170, 59);
            this.tbHelp.TabIndex = 0;
            // 
            // sfdExportErrors
            // 
            this.sfdExportErrors.DefaultExt = "txt";
            this.sfdExportErrors.Title = "Export errors to file";
            // 
            // tcQueries
            // 
            this.tcQueries.ContextMenuStrip = this.cmsRepair;
            this.tcQueries.Controls.Add(this.tpTemplate);
            this.tcQueries.Controls.Add(this.tpUnship);
            this.tcQueries.Controls.Add(this.tpUnshipped);
            this.tcQueries.Controls.Add(this.tpReprint);
            this.tcQueries.Controls.Add(this.tpRepair);
            this.tcQueries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcQueries.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tcQueries.Location = new System.Drawing.Point(0, 24);
            this.tcQueries.Name = "tcQueries";
            this.tcQueries.Padding = new System.Drawing.Point(24, 6);
            this.tcQueries.SelectedIndex = 0;
            this.tcQueries.Size = new System.Drawing.Size(1184, 537);
            this.tcQueries.TabIndex = 1;
            this.tcQueries.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tcQueries_DrawItem);
            this.tcQueries.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            this.tcQueries.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tcQueries_Deselecting);
            this.tcQueries.Deselected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Deselected);
            // 
            // cmsRepair
            // 
            this.cmsRepair.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRepairCopyToClipboard});
            this.cmsRepair.Name = "cmsRepair";
            this.cmsRepair.Size = new System.Drawing.Size(173, 26);
            // 
            // miRepairCopyToClipboard
            // 
            this.miRepairCopyToClipboard.Name = "miRepairCopyToClipboard";
            this.miRepairCopyToClipboard.Size = new System.Drawing.Size(172, 22);
            this.miRepairCopyToClipboard.Text = "Copy To Clipboard";
            // 
            // tpUnship
            // 
            this.tpUnship.Controls.Add(this.panel9);
            this.tpUnship.Controls.Add(this.panel10);
            this.tpUnship.Controls.Add(this.panel23);
            this.tpUnship.Location = new System.Drawing.Point(4, 28);
            this.tpUnship.Name = "tpUnship";
            this.tpUnship.Padding = new System.Windows.Forms.Padding(3);
            this.tpUnship.Size = new System.Drawing.Size(1176, 505);
            this.tpUnship.TabIndex = 1;
            this.tpUnship.Text = "Unship";
            this.tpUnship.UseVisualStyleBackColor = true;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.dgvUnship);
            this.panel9.Controls.Add(this.panel12);
            this.panel9.Controls.Add(this.panel11);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(3, 65);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(970, 437);
            this.panel9.TabIndex = 0;
            // 
            // dgvUnship
            // 
            this.dgvUnship.AllowUserToAddRows = false;
            this.dgvUnship.AllowUserToDeleteRows = false;
            this.dgvUnship.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUnship.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvUnship.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUnship.ContextMenuStrip = this.cmsUnship;
            this.dgvUnship.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUnship.Location = new System.Drawing.Point(0, 28);
            this.dgvUnship.Name = "dgvUnship";
            this.dgvUnship.ReadOnly = true;
            this.dgvUnship.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUnship.Size = new System.Drawing.Size(970, 381);
            this.dgvUnship.TabIndex = 4;
            // 
            // cmsUnship
            // 
            this.cmsUnship.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miUnshipRemoveSelected,
            this.miUnshipCopyToClipboard});
            this.cmsUnship.Name = "cmsSubset";
            this.cmsUnship.Size = new System.Drawing.Size(173, 48);
            this.cmsUnship.Opening += new System.ComponentModel.CancelEventHandler(this.cmsUnship_Opening);
            // 
            // miUnshipRemoveSelected
            // 
            this.miUnshipRemoveSelected.Name = "miUnshipRemoveSelected";
            this.miUnshipRemoveSelected.Size = new System.Drawing.Size(172, 22);
            this.miUnshipRemoveSelected.Text = "Remove Selected";
            this.miUnshipRemoveSelected.Click += new System.EventHandler(this.miUnshipRemoveSelected_Click);
            // 
            // miUnshipCopyToClipboard
            // 
            this.miUnshipCopyToClipboard.Name = "miUnshipCopyToClipboard";
            this.miUnshipCopyToClipboard.Size = new System.Drawing.Size(172, 22);
            this.miUnshipCopyToClipboard.Text = "Copy To Clipboard";
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.ssUnship);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel12.Location = new System.Drawing.Point(0, 409);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(970, 28);
            this.panel12.TabIndex = 3;
            // 
            // ssUnship
            // 
            this.ssUnship.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssUnship.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslUnshipTotal,
            this.tsslUnshipShipped,
            this.tsslUnshipUnshipped,
            this.tsslUnshipFailed});
            this.ssUnship.Location = new System.Drawing.Point(0, 0);
            this.ssUnship.Name = "ssUnship";
            this.ssUnship.Size = new System.Drawing.Size(970, 28);
            this.ssUnship.SizingGrip = false;
            this.ssUnship.TabIndex = 3;
            this.ssUnship.Text = "statusStrip1";
            // 
            // tsslUnshipTotal
            // 
            this.tsslUnshipTotal.Name = "tsslUnshipTotal";
            this.tsslUnshipTotal.Size = new System.Drawing.Size(118, 23);
            this.tsslUnshipTotal.Text = "toolStripStatusLabel1";
            // 
            // tsslUnshipShipped
            // 
            this.tsslUnshipShipped.Name = "tsslUnshipShipped";
            this.tsslUnshipShipped.Size = new System.Drawing.Size(118, 23);
            this.tsslUnshipShipped.Text = "toolStripStatusLabel2";
            // 
            // tsslUnshipUnshipped
            // 
            this.tsslUnshipUnshipped.Name = "tsslUnshipUnshipped";
            this.tsslUnshipUnshipped.Size = new System.Drawing.Size(118, 23);
            this.tsslUnshipUnshipped.Text = "toolStripStatusLabel3";
            // 
            // tsslUnshipFailed
            // 
            this.tsslUnshipFailed.Name = "tsslUnshipFailed";
            this.tsslUnshipFailed.Size = new System.Drawing.Size(118, 23);
            this.tsslUnshipFailed.Text = "toolStripStatusLabel4";
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.SystemColors.Control;
            this.panel11.Controls.Add(this.chkUnshipFilterShowFailed);
            this.panel11.Controls.Add(this.chkUnshipFilterShowUnshipped);
            this.panel11.Controls.Add(this.chkUnshipFilterShowShipped);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(970, 28);
            this.panel11.TabIndex = 1;
            // 
            // chkUnshipFilterShowFailed
            // 
            this.chkUnshipFilterShowFailed.AutoSize = true;
            this.chkUnshipFilterShowFailed.Checked = true;
            this.chkUnshipFilterShowFailed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnshipFilterShowFailed.Location = new System.Drawing.Point(232, 6);
            this.chkUnshipFilterShowFailed.Name = "chkUnshipFilterShowFailed";
            this.chkUnshipFilterShowFailed.Size = new System.Drawing.Size(89, 17);
            this.chkUnshipFilterShowFailed.TabIndex = 2;
            this.chkUnshipFilterShowFailed.Text = "Show Failed";
            this.chkUnshipFilterShowFailed.UseVisualStyleBackColor = true;
            this.chkUnshipFilterShowFailed.CheckedChanged += new System.EventHandler(this.chkUnshipFilterShowShipped_CheckedChanged);
            // 
            // chkUnshipFilterShowUnshipped
            // 
            this.chkUnshipFilterShowUnshipped.AutoSize = true;
            this.chkUnshipFilterShowUnshipped.Checked = true;
            this.chkUnshipFilterShowUnshipped.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnshipFilterShowUnshipped.Location = new System.Drawing.Point(111, 6);
            this.chkUnshipFilterShowUnshipped.Name = "chkUnshipFilterShowUnshipped";
            this.chkUnshipFilterShowUnshipped.Size = new System.Drawing.Size(115, 17);
            this.chkUnshipFilterShowUnshipped.TabIndex = 1;
            this.chkUnshipFilterShowUnshipped.Text = "Show Unshipped";
            this.chkUnshipFilterShowUnshipped.UseVisualStyleBackColor = true;
            this.chkUnshipFilterShowUnshipped.CheckedChanged += new System.EventHandler(this.chkUnshipFilterShowShipped_CheckedChanged);
            // 
            // chkUnshipFilterShowShipped
            // 
            this.chkUnshipFilterShowShipped.AutoSize = true;
            this.chkUnshipFilterShowShipped.Checked = true;
            this.chkUnshipFilterShowShipped.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnshipFilterShowShipped.Location = new System.Drawing.Point(4, 6);
            this.chkUnshipFilterShowShipped.Name = "chkUnshipFilterShowShipped";
            this.chkUnshipFilterShowShipped.Size = new System.Drawing.Size(101, 17);
            this.chkUnshipFilterShowShipped.TabIndex = 0;
            this.chkUnshipFilterShowShipped.Text = "Show Shipped";
            this.chkUnshipFilterShowShipped.UseVisualStyleBackColor = true;
            this.chkUnshipFilterShowShipped.CheckedChanged += new System.EventHandler(this.chkUnshipFilterShowShipped_CheckedChanged);
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.SystemColors.Control;
            this.panel10.Controls.Add(this.btUnshipLastBatches);
            this.panel10.Controls.Add(this.btUnshipClear);
            this.panel10.Controls.Add(this.btExportUnshipErrors);
            this.panel10.Controls.Add(this.btUnship);
            this.panel10.Controls.Add(this.btAddUnshipInvoice);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel10.Location = new System.Drawing.Point(973, 65);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(200, 437);
            this.panel10.TabIndex = 1;
            // 
            // btUnshipLastBatches
            // 
            this.btUnshipLastBatches.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUnshipLastBatches.Location = new System.Drawing.Point(10, 69);
            this.btUnshipLastBatches.Name = "btUnshipLastBatches";
            this.btUnshipLastBatches.Size = new System.Drawing.Size(180, 35);
            this.btUnshipLastBatches.TabIndex = 9;
            this.btUnshipLastBatches.Text = "Last Batches";
            this.btUnshipLastBatches.UseVisualStyleBackColor = true;
            this.btUnshipLastBatches.Click += new System.EventHandler(this.btUnshipLastBatches_Click);
            // 
            // btUnshipClear
            // 
            this.btUnshipClear.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUnshipClear.Location = new System.Drawing.Point(10, 125);
            this.btUnshipClear.Name = "btUnshipClear";
            this.btUnshipClear.Size = new System.Drawing.Size(180, 35);
            this.btUnshipClear.TabIndex = 3;
            this.btUnshipClear.Text = "Clear List";
            this.btUnshipClear.UseVisualStyleBackColor = true;
            this.btUnshipClear.Click += new System.EventHandler(this.btUnshipClear_Click);
            // 
            // btExportUnshipErrors
            // 
            this.btExportUnshipErrors.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btExportUnshipErrors.Location = new System.Drawing.Point(10, 222);
            this.btExportUnshipErrors.Name = "btExportUnshipErrors";
            this.btExportUnshipErrors.Size = new System.Drawing.Size(180, 35);
            this.btExportUnshipErrors.TabIndex = 2;
            this.btExportUnshipErrors.Text = "Export errors";
            this.btExportUnshipErrors.UseVisualStyleBackColor = true;
            this.btExportUnshipErrors.Click += new System.EventHandler(this.btExportUnshipErrors_Click);
            // 
            // btUnship
            // 
            this.btUnship.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUnship.Location = new System.Drawing.Point(10, 181);
            this.btUnship.Name = "btUnship";
            this.btUnship.Size = new System.Drawing.Size(180, 35);
            this.btUnship.TabIndex = 1;
            this.btUnship.Text = "Unship";
            this.btUnship.UseVisualStyleBackColor = true;
            this.btUnship.Click += new System.EventHandler(this.btUnship_Click);
            // 
            // btAddUnshipInvoice
            // 
            this.btAddUnshipInvoice.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAddUnshipInvoice.Location = new System.Drawing.Point(10, 28);
            this.btAddUnshipInvoice.Name = "btAddUnshipInvoice";
            this.btAddUnshipInvoice.Size = new System.Drawing.Size(180, 35);
            this.btAddUnshipInvoice.TabIndex = 0;
            this.btAddUnshipInvoice.Text = "Add Invoices";
            this.btAddUnshipInvoice.UseVisualStyleBackColor = true;
            this.btAddUnshipInvoice.Click += new System.EventHandler(this.btAddUnshipInvoice_Click);
            // 
            // panel23
            // 
            this.panel23.Controls.Add(this.tbHelpUnship);
            this.panel23.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel23.Location = new System.Drawing.Point(3, 3);
            this.panel23.Name = "panel23";
            this.panel23.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel23.Size = new System.Drawing.Size(1170, 62);
            this.panel23.TabIndex = 4;
            // 
            // tbHelpUnship
            // 
            this.tbHelpUnship.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbHelpUnship.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHelpUnship.Location = new System.Drawing.Point(0, 0);
            this.tbHelpUnship.Multiline = true;
            this.tbHelpUnship.Name = "tbHelpUnship";
            this.tbHelpUnship.ReadOnly = true;
            this.tbHelpUnship.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbHelpUnship.Size = new System.Drawing.Size(1170, 59);
            this.tbHelpUnship.TabIndex = 0;
            // 
            // tpUnshipped
            // 
            this.tpUnshipped.Controls.Add(this.panel13);
            this.tpUnshipped.Controls.Add(this.panel16);
            this.tpUnshipped.Controls.Add(this.panel22);
            this.tpUnshipped.Location = new System.Drawing.Point(4, 28);
            this.tpUnshipped.Name = "tpUnshipped";
            this.tpUnshipped.Padding = new System.Windows.Forms.Padding(3);
            this.tpUnshipped.Size = new System.Drawing.Size(1176, 505);
            this.tpUnshipped.TabIndex = 2;
            this.tpUnshipped.Text = "Pick New";
            this.tpUnshipped.UseVisualStyleBackColor = true;
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.dgvUnshipped);
            this.panel13.Controls.Add(this.panel15);
            this.panel13.Controls.Add(this.panel14);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel13.Location = new System.Drawing.Point(3, 65);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(970, 437);
            this.panel13.TabIndex = 0;
            // 
            // dgvUnshipped
            // 
            this.dgvUnshipped.AllowUserToAddRows = false;
            this.dgvUnshipped.AllowUserToDeleteRows = false;
            this.dgvUnshipped.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUnshipped.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvUnshipped.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUnshipped.ContextMenuStrip = this.cmsUnshipped;
            this.dgvUnshipped.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUnshipped.Location = new System.Drawing.Point(0, 28);
            this.dgvUnshipped.Name = "dgvUnshipped";
            this.dgvUnshipped.ReadOnly = true;
            this.dgvUnshipped.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUnshipped.Size = new System.Drawing.Size(970, 381);
            this.dgvUnshipped.TabIndex = 4;
            // 
            // cmsUnshipped
            // 
            this.cmsUnshipped.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miUnshippedCopyToClipboard});
            this.cmsUnshipped.Name = "cmsUnshipped";
            this.cmsUnshipped.Size = new System.Drawing.Size(173, 26);
            // 
            // miUnshippedCopyToClipboard
            // 
            this.miUnshippedCopyToClipboard.Name = "miUnshippedCopyToClipboard";
            this.miUnshippedCopyToClipboard.Size = new System.Drawing.Size(172, 22);
            this.miUnshippedCopyToClipboard.Text = "Copy To Clipboard";
            // 
            // panel15
            // 
            this.panel15.BackColor = System.Drawing.SystemColors.Control;
            this.panel15.Controls.Add(this.chkUnshippedShowNonupdated);
            this.panel15.Controls.Add(this.chkUnshippedShowFailed);
            this.panel15.Controls.Add(this.chkUnshippedShowUpdated);
            this.panel15.Controls.Add(this.chkUnshippedShowUnshipped);
            this.panel15.Controls.Add(this.chkUnshippedShowNew);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel15.Location = new System.Drawing.Point(0, 0);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(970, 28);
            this.panel15.TabIndex = 1;
            // 
            // chkUnshippedShowNonupdated
            // 
            this.chkUnshippedShowNonupdated.AutoSize = true;
            this.chkUnshippedShowNonupdated.Checked = true;
            this.chkUnshippedShowNonupdated.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnshippedShowNonupdated.Location = new System.Drawing.Point(5, 6);
            this.chkUnshippedShowNonupdated.Name = "chkUnshippedShowNonupdated";
            this.chkUnshippedShowNonupdated.Size = new System.Drawing.Size(124, 17);
            this.chkUnshippedShowNonupdated.TabIndex = 4;
            this.chkUnshippedShowNonupdated.Text = "Show Nonupdated";
            this.chkUnshippedShowNonupdated.UseVisualStyleBackColor = true;
            this.chkUnshippedShowNonupdated.CheckedChanged += new System.EventHandler(this.OnUnshippedFilterClick);
            // 
            // chkUnshippedShowFailed
            // 
            this.chkUnshippedShowFailed.AutoSize = true;
            this.chkUnshippedShowFailed.Checked = true;
            this.chkUnshippedShowFailed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnshippedShowFailed.Location = new System.Drawing.Point(244, 6);
            this.chkUnshippedShowFailed.Name = "chkUnshippedShowFailed";
            this.chkUnshippedShowFailed.Size = new System.Drawing.Size(89, 17);
            this.chkUnshippedShowFailed.TabIndex = 3;
            this.chkUnshippedShowFailed.Text = "Show Failed";
            this.chkUnshippedShowFailed.UseVisualStyleBackColor = true;
            this.chkUnshippedShowFailed.CheckedChanged += new System.EventHandler(this.OnUnshippedFilterClick);
            // 
            // chkUnshippedShowUpdated
            // 
            this.chkUnshippedShowUpdated.AutoSize = true;
            this.chkUnshippedShowUpdated.Checked = true;
            this.chkUnshippedShowUpdated.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnshippedShowUpdated.Location = new System.Drawing.Point(135, 6);
            this.chkUnshippedShowUpdated.Name = "chkUnshippedShowUpdated";
            this.chkUnshippedShowUpdated.Size = new System.Drawing.Size(103, 17);
            this.chkUnshippedShowUpdated.TabIndex = 2;
            this.chkUnshippedShowUpdated.Text = "Show Updated";
            this.chkUnshippedShowUpdated.UseVisualStyleBackColor = true;
            this.chkUnshippedShowUpdated.CheckedChanged += new System.EventHandler(this.OnUnshippedFilterClick);
            // 
            // chkUnshippedShowUnshipped
            // 
            this.chkUnshippedShowUnshipped.AutoSize = true;
            this.chkUnshippedShowUnshipped.Checked = true;
            this.chkUnshippedShowUnshipped.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnshippedShowUnshipped.Location = new System.Drawing.Point(436, 6);
            this.chkUnshippedShowUnshipped.Name = "chkUnshippedShowUnshipped";
            this.chkUnshippedShowUnshipped.Size = new System.Drawing.Size(115, 17);
            this.chkUnshippedShowUnshipped.TabIndex = 1;
            this.chkUnshippedShowUnshipped.Text = "Show Unshipped";
            this.chkUnshippedShowUnshipped.UseVisualStyleBackColor = true;
            this.chkUnshippedShowUnshipped.Visible = false;
            this.chkUnshippedShowUnshipped.CheckedChanged += new System.EventHandler(this.OnUnshippedFilterClick);
            // 
            // chkUnshippedShowNew
            // 
            this.chkUnshippedShowNew.AutoSize = true;
            this.chkUnshippedShowNew.Checked = true;
            this.chkUnshippedShowNew.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnshippedShowNew.Location = new System.Drawing.Point(557, 6);
            this.chkUnshippedShowNew.Name = "chkUnshippedShowNew";
            this.chkUnshippedShowNew.Size = new System.Drawing.Size(81, 17);
            this.chkUnshippedShowNew.TabIndex = 0;
            this.chkUnshippedShowNew.Text = "Show New";
            this.chkUnshippedShowNew.UseVisualStyleBackColor = true;
            this.chkUnshippedShowNew.Visible = false;
            this.chkUnshippedShowNew.CheckedChanged += new System.EventHandler(this.OnUnshippedFilterClick);
            // 
            // panel14
            // 
            this.panel14.Controls.Add(this.ssUnshipped);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel14.Location = new System.Drawing.Point(0, 409);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(970, 28);
            this.panel14.TabIndex = 3;
            // 
            // ssUnshipped
            // 
            this.ssUnshipped.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssUnshipped.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslUnshippedTotal,
            this.tsslNonupdated,
            this.tsslUnshippedUpdated,
            this.tsslUnshippedFailed});
            this.ssUnshipped.Location = new System.Drawing.Point(0, 0);
            this.ssUnshipped.Name = "ssUnshipped";
            this.ssUnshipped.Size = new System.Drawing.Size(970, 28);
            this.ssUnshipped.SizingGrip = false;
            this.ssUnshipped.TabIndex = 3;
            this.ssUnshipped.Text = "statusStrip2";
            // 
            // tsslUnshippedTotal
            // 
            this.tsslUnshippedTotal.Name = "tsslUnshippedTotal";
            this.tsslUnshippedTotal.Size = new System.Drawing.Size(118, 23);
            this.tsslUnshippedTotal.Text = "toolStripStatusLabel1";
            // 
            // tsslNonupdated
            // 
            this.tsslNonupdated.Name = "tsslNonupdated";
            this.tsslNonupdated.Size = new System.Drawing.Size(118, 23);
            this.tsslNonupdated.Text = "toolStripStatusLabel2";
            // 
            // tsslUnshippedUpdated
            // 
            this.tsslUnshippedUpdated.Name = "tsslUnshippedUpdated";
            this.tsslUnshippedUpdated.Size = new System.Drawing.Size(118, 23);
            this.tsslUnshippedUpdated.Text = "toolStripStatusLabel4";
            // 
            // tsslUnshippedFailed
            // 
            this.tsslUnshippedFailed.Name = "tsslUnshippedFailed";
            this.tsslUnshippedFailed.Size = new System.Drawing.Size(118, 23);
            this.tsslUnshippedFailed.Text = "toolStripStatusLabel1";
            // 
            // panel16
            // 
            this.panel16.BackColor = System.Drawing.SystemColors.Control;
            this.panel16.Controls.Add(this.btOnHold);
            this.panel16.Controls.Add(this.btSetPmodMaxDailyPackages);
            this.panel16.Controls.Add(this.edPmodMaxDailyPackages);
            this.panel16.Controls.Add(this.btUnshippedResetStatus);
            this.panel16.Controls.Add(this.btUnshippedClearSelection);
            this.panel16.Controls.Add(this.btUnshippedSelectAll);
            this.panel16.Controls.Add(this.btUnshippedSetPickable);
            this.panel16.Controls.Add(this.btReloarUnshipped);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel16.Location = new System.Drawing.Point(973, 65);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(200, 437);
            this.panel16.TabIndex = 1;
            // 
            // btOnHold
            // 
            this.btOnHold.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btOnHold.Location = new System.Drawing.Point(10, 178);
            this.btOnHold.Name = "btOnHold";
            this.btOnHold.Size = new System.Drawing.Size(180, 35);
            this.btOnHold.TabIndex = 11;
            this.btOnHold.Text = "Set On-Hold";
            this.btOnHold.UseVisualStyleBackColor = true;
            this.btOnHold.Click += new System.EventHandler(this.btOnHold_Click);
            // 
            // btSetPmodMaxDailyPackages
            // 
            this.btSetPmodMaxDailyPackages.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSetPmodMaxDailyPackages.Location = new System.Drawing.Point(10, 391);
            this.btSetPmodMaxDailyPackages.Name = "btSetPmodMaxDailyPackages";
            this.btSetPmodMaxDailyPackages.Size = new System.Drawing.Size(180, 35);
            this.btSetPmodMaxDailyPackages.TabIndex = 10;
            this.btSetPmodMaxDailyPackages.Text = "Update Value";
            this.btSetPmodMaxDailyPackages.UseVisualStyleBackColor = true;
            this.btSetPmodMaxDailyPackages.Click += new System.EventHandler(this.btSetPmodMaxDailyPackages_Click);
            // 
            // edPmodMaxDailyPackages
            // 
            this.edPmodMaxDailyPackages.Enabled = false;
            this.edPmodMaxDailyPackages.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edPmodMaxDailyPackages.Location = new System.Drawing.Point(10, 365);
            this.edPmodMaxDailyPackages.Name = "edPmodMaxDailyPackages";
            this.edPmodMaxDailyPackages.Size = new System.Drawing.Size(180, 22);
            this.edPmodMaxDailyPackages.TabIndex = 9;
            // 
            // btUnshippedResetStatus
            // 
            this.btUnshippedResetStatus.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUnshippedResetStatus.Location = new System.Drawing.Point(10, 238);
            this.btUnshippedResetStatus.Name = "btUnshippedResetStatus";
            this.btUnshippedResetStatus.Size = new System.Drawing.Size(180, 35);
            this.btUnshippedResetStatus.TabIndex = 4;
            this.btUnshippedResetStatus.Text = "Reset Status";
            this.btUnshippedResetStatus.UseVisualStyleBackColor = true;
            this.btUnshippedResetStatus.Click += new System.EventHandler(this.btUnshippedResetStatus_Click);
            // 
            // btUnshippedClearSelection
            // 
            this.btUnshippedClearSelection.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUnshippedClearSelection.Location = new System.Drawing.Point(10, 86);
            this.btUnshippedClearSelection.Name = "btUnshippedClearSelection";
            this.btUnshippedClearSelection.Size = new System.Drawing.Size(180, 35);
            this.btUnshippedClearSelection.TabIndex = 3;
            this.btUnshippedClearSelection.Text = "Clear Selection";
            this.btUnshippedClearSelection.UseVisualStyleBackColor = true;
            this.btUnshippedClearSelection.Click += new System.EventHandler(this.btUnshippedClearSelection_Click);
            // 
            // btUnshippedSelectAll
            // 
            this.btUnshippedSelectAll.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUnshippedSelectAll.Location = new System.Drawing.Point(10, 45);
            this.btUnshippedSelectAll.Name = "btUnshippedSelectAll";
            this.btUnshippedSelectAll.Size = new System.Drawing.Size(180, 35);
            this.btUnshippedSelectAll.TabIndex = 2;
            this.btUnshippedSelectAll.Text = "Select All";
            this.btUnshippedSelectAll.UseVisualStyleBackColor = true;
            this.btUnshippedSelectAll.Click += new System.EventHandler(this.btUnshippedSelectAll_Click);
            // 
            // btUnshippedSetPickable
            // 
            this.btUnshippedSetPickable.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUnshippedSetPickable.Location = new System.Drawing.Point(10, 137);
            this.btUnshippedSetPickable.Name = "btUnshippedSetPickable";
            this.btUnshippedSetPickable.Size = new System.Drawing.Size(180, 35);
            this.btUnshippedSetPickable.TabIndex = 1;
            this.btUnshippedSetPickable.Text = "Set PICKABLE";
            this.btUnshippedSetPickable.UseVisualStyleBackColor = true;
            this.btUnshippedSetPickable.Click += new System.EventHandler(this.btUnshippedSetPickable_Click);
            // 
            // btReloarUnshipped
            // 
            this.btReloarUnshipped.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btReloarUnshipped.Location = new System.Drawing.Point(10, 291);
            this.btReloarUnshipped.Name = "btReloarUnshipped";
            this.btReloarUnshipped.Size = new System.Drawing.Size(180, 35);
            this.btReloarUnshipped.TabIndex = 0;
            this.btReloarUnshipped.Text = "Reload List";
            this.btReloarUnshipped.UseVisualStyleBackColor = true;
            this.btReloarUnshipped.Click += new System.EventHandler(this.btReloadUnshipped_Click);
            // 
            // panel22
            // 
            this.panel22.Controls.Add(this.tbHelpUnshipped);
            this.panel22.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel22.Location = new System.Drawing.Point(3, 3);
            this.panel22.Name = "panel22";
            this.panel22.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel22.Size = new System.Drawing.Size(1170, 62);
            this.panel22.TabIndex = 3;
            // 
            // tbHelpUnshipped
            // 
            this.tbHelpUnshipped.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbHelpUnshipped.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHelpUnshipped.Location = new System.Drawing.Point(0, 0);
            this.tbHelpUnshipped.Multiline = true;
            this.tbHelpUnshipped.Name = "tbHelpUnshipped";
            this.tbHelpUnshipped.ReadOnly = true;
            this.tbHelpUnshipped.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbHelpUnshipped.Size = new System.Drawing.Size(1170, 59);
            this.tbHelpUnshipped.TabIndex = 0;
            // 
            // tpReprint
            // 
            this.tpReprint.Controls.Add(this.panel17);
            this.tpReprint.Controls.Add(this.panel20);
            this.tpReprint.Controls.Add(this.panel24);
            this.tpReprint.Location = new System.Drawing.Point(4, 28);
            this.tpReprint.Name = "tpReprint";
            this.tpReprint.Padding = new System.Windows.Forms.Padding(3);
            this.tpReprint.Size = new System.Drawing.Size(1176, 505);
            this.tpReprint.TabIndex = 3;
            this.tpReprint.Text = "Reprint";
            this.tpReprint.UseVisualStyleBackColor = true;
            // 
            // panel17
            // 
            this.panel17.Controls.Add(this.dgvReprint);
            this.panel17.Controls.Add(this.panel18);
            this.panel17.Controls.Add(this.panel19);
            this.panel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel17.Location = new System.Drawing.Point(3, 65);
            this.panel17.Name = "panel17";
            this.panel17.Size = new System.Drawing.Size(970, 437);
            this.panel17.TabIndex = 0;
            // 
            // dgvReprint
            // 
            this.dgvReprint.AllowUserToAddRows = false;
            this.dgvReprint.AllowUserToDeleteRows = false;
            this.dgvReprint.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReprint.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvReprint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReprint.ContextMenuStrip = this.cmsReprint;
            this.dgvReprint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReprint.Location = new System.Drawing.Point(0, 28);
            this.dgvReprint.Name = "dgvReprint";
            this.dgvReprint.ReadOnly = true;
            this.dgvReprint.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReprint.Size = new System.Drawing.Size(970, 381);
            this.dgvReprint.TabIndex = 4;
            // 
            // cmsReprint
            // 
            this.cmsReprint.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiReprintRemoveSelected,
            this.miPreviewReprintInvoice,
            this.miReprintCopyToClipboard});
            this.cmsReprint.Name = "cmsSubset";
            this.cmsReprint.Size = new System.Drawing.Size(173, 70);
            this.cmsReprint.Opening += new System.ComponentModel.CancelEventHandler(this.cmsReprint_Opening);
            // 
            // tsmiReprintRemoveSelected
            // 
            this.tsmiReprintRemoveSelected.Name = "tsmiReprintRemoveSelected";
            this.tsmiReprintRemoveSelected.Size = new System.Drawing.Size(172, 22);
            this.tsmiReprintRemoveSelected.Text = "Remove Selected";
            this.tsmiReprintRemoveSelected.Click += new System.EventHandler(this.tsmiReprintRemoveSelected_Click);
            // 
            // miPreviewReprintInvoice
            // 
            this.miPreviewReprintInvoice.Name = "miPreviewReprintInvoice";
            this.miPreviewReprintInvoice.Size = new System.Drawing.Size(172, 22);
            this.miPreviewReprintInvoice.Text = "Preview Invoice";
            this.miPreviewReprintInvoice.Click += new System.EventHandler(this.miPreviewReprintInvoice_Click);
            // 
            // miReprintCopyToClipboard
            // 
            this.miReprintCopyToClipboard.Name = "miReprintCopyToClipboard";
            this.miReprintCopyToClipboard.Size = new System.Drawing.Size(172, 22);
            this.miReprintCopyToClipboard.Text = "Copy To Clipboard";
            // 
            // panel18
            // 
            this.panel18.Controls.Add(this.ssReprint);
            this.panel18.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel18.Location = new System.Drawing.Point(0, 409);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(970, 28);
            this.panel18.TabIndex = 3;
            // 
            // ssReprint
            // 
            this.ssReprint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssReprint.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslReprintTotal,
            this.tsslReprintUnprinted,
            this.tsslReprintPrinted,
            this.tsslReprintFailed,
            this.tsslReprintLocked});
            this.ssReprint.Location = new System.Drawing.Point(0, 0);
            this.ssReprint.Name = "ssReprint";
            this.ssReprint.Size = new System.Drawing.Size(970, 28);
            this.ssReprint.SizingGrip = false;
            this.ssReprint.TabIndex = 3;
            this.ssReprint.Text = "statusStrip3";
            // 
            // tsslReprintTotal
            // 
            this.tsslReprintTotal.Name = "tsslReprintTotal";
            this.tsslReprintTotal.Size = new System.Drawing.Size(118, 23);
            this.tsslReprintTotal.Text = "toolStripStatusLabel1";
            // 
            // tsslReprintUnprinted
            // 
            this.tsslReprintUnprinted.Name = "tsslReprintUnprinted";
            this.tsslReprintUnprinted.Size = new System.Drawing.Size(118, 23);
            this.tsslReprintUnprinted.Text = "toolStripStatusLabel2";
            // 
            // tsslReprintPrinted
            // 
            this.tsslReprintPrinted.Name = "tsslReprintPrinted";
            this.tsslReprintPrinted.Size = new System.Drawing.Size(118, 23);
            this.tsslReprintPrinted.Text = "toolStripStatusLabel3";
            // 
            // tsslReprintFailed
            // 
            this.tsslReprintFailed.Name = "tsslReprintFailed";
            this.tsslReprintFailed.Size = new System.Drawing.Size(118, 23);
            this.tsslReprintFailed.Text = "toolStripStatusLabel4";
            // 
            // tsslReprintLocked
            // 
            this.tsslReprintLocked.Name = "tsslReprintLocked";
            this.tsslReprintLocked.Size = new System.Drawing.Size(118, 23);
            this.tsslReprintLocked.Text = "toolStripStatusLabel5";
            // 
            // panel19
            // 
            this.panel19.BackColor = System.Drawing.SystemColors.Control;
            this.panel19.Controls.Add(this.chkReprintShowLocked);
            this.panel19.Controls.Add(this.chkReprintShowFailed);
            this.panel19.Controls.Add(this.chkReprintShowPrinted);
            this.panel19.Controls.Add(this.chkReprintShowUnprinted);
            this.panel19.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel19.Location = new System.Drawing.Point(0, 0);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(970, 28);
            this.panel19.TabIndex = 1;
            // 
            // chkReprintShowLocked
            // 
            this.chkReprintShowLocked.AutoSize = true;
            this.chkReprintShowLocked.Checked = true;
            this.chkReprintShowLocked.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReprintShowLocked.Location = new System.Drawing.Point(318, 6);
            this.chkReprintShowLocked.Name = "chkReprintShowLocked";
            this.chkReprintShowLocked.Size = new System.Drawing.Size(94, 17);
            this.chkReprintShowLocked.TabIndex = 7;
            this.chkReprintShowLocked.Text = "Show Locked";
            this.chkReprintShowLocked.UseVisualStyleBackColor = true;
            this.chkReprintShowLocked.CheckedChanged += new System.EventHandler(this.OnReprintFilterClick);
            // 
            // chkReprintShowFailed
            // 
            this.chkReprintShowFailed.AutoSize = true;
            this.chkReprintShowFailed.Checked = true;
            this.chkReprintShowFailed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReprintShowFailed.Location = new System.Drawing.Point(223, 6);
            this.chkReprintShowFailed.Name = "chkReprintShowFailed";
            this.chkReprintShowFailed.Size = new System.Drawing.Size(89, 17);
            this.chkReprintShowFailed.TabIndex = 6;
            this.chkReprintShowFailed.Text = "Show Failed";
            this.chkReprintShowFailed.UseVisualStyleBackColor = true;
            this.chkReprintShowFailed.CheckedChanged += new System.EventHandler(this.OnReprintFilterClick);
            // 
            // chkReprintShowPrinted
            // 
            this.chkReprintShowPrinted.AutoSize = true;
            this.chkReprintShowPrinted.Checked = true;
            this.chkReprintShowPrinted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReprintShowPrinted.Location = new System.Drawing.Point(122, 6);
            this.chkReprintShowPrinted.Name = "chkReprintShowPrinted";
            this.chkReprintShowPrinted.Size = new System.Drawing.Size(95, 17);
            this.chkReprintShowPrinted.TabIndex = 5;
            this.chkReprintShowPrinted.Text = "Show Printed";
            this.chkReprintShowPrinted.UseVisualStyleBackColor = true;
            this.chkReprintShowPrinted.CheckedChanged += new System.EventHandler(this.OnReprintFilterClick);
            // 
            // chkReprintShowUnprinted
            // 
            this.chkReprintShowUnprinted.AutoSize = true;
            this.chkReprintShowUnprinted.Checked = true;
            this.chkReprintShowUnprinted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReprintShowUnprinted.Location = new System.Drawing.Point(5, 6);
            this.chkReprintShowUnprinted.Name = "chkReprintShowUnprinted";
            this.chkReprintShowUnprinted.Size = new System.Drawing.Size(111, 17);
            this.chkReprintShowUnprinted.TabIndex = 4;
            this.chkReprintShowUnprinted.Text = "Show Unprinted";
            this.chkReprintShowUnprinted.UseVisualStyleBackColor = true;
            this.chkReprintShowUnprinted.CheckedChanged += new System.EventHandler(this.OnReprintFilterClick);
            // 
            // panel20
            // 
            this.panel20.BackColor = System.Drawing.SystemColors.Control;
            this.panel20.Controls.Add(this.chkReprintSequenceNumber);
            this.panel20.Controls.Add(this.btReprintLastBatches);
            this.panel20.Controls.Add(this.label3);
            this.panel20.Controls.Add(this.cbReprintPrinter);
            this.panel20.Controls.Add(this.btReprintExportErrors);
            this.panel20.Controls.Add(this.btReprintPrint);
            this.panel20.Controls.Add(this.btReprintClear);
            this.panel20.Controls.Add(this.btAddReprintPackage);
            this.panel20.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel20.Location = new System.Drawing.Point(973, 65);
            this.panel20.Name = "panel20";
            this.panel20.Size = new System.Drawing.Size(200, 437);
            this.panel20.TabIndex = 1;
            // 
            // chkReprintSequenceNumber
            // 
            this.chkReprintSequenceNumber.AutoSize = true;
            this.chkReprintSequenceNumber.Location = new System.Drawing.Point(10, 235);
            this.chkReprintSequenceNumber.Name = "chkReprintSequenceNumber";
            this.chkReprintSequenceNumber.Size = new System.Drawing.Size(186, 17);
            this.chkReprintSequenceNumber.TabIndex = 9;
            this.chkReprintSequenceNumber.Text = "Print Invoice Sequence Number";
            this.chkReprintSequenceNumber.UseVisualStyleBackColor = true;
            this.chkReprintSequenceNumber.Click += new System.EventHandler(this.chkReprintSequenceNumber_Click);
            // 
            // btReprintLastBatches
            // 
            this.btReprintLastBatches.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btReprintLastBatches.Location = new System.Drawing.Point(10, 110);
            this.btReprintLastBatches.Name = "btReprintLastBatches";
            this.btReprintLastBatches.Size = new System.Drawing.Size(180, 35);
            this.btReprintLastBatches.TabIndex = 8;
            this.btReprintLastBatches.Text = "Last Batches";
            this.btReprintLastBatches.UseVisualStyleBackColor = true;
            this.btReprintLastBatches.Click += new System.EventHandler(this.btReprintLastBatches_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Printer";
            // 
            // cbReprintPrinter
            // 
            this.cbReprintPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReprintPrinter.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.cbReprintPrinter.FormattingEnabled = true;
            this.cbReprintPrinter.Location = new System.Drawing.Point(10, 28);
            this.cbReprintPrinter.Name = "cbReprintPrinter";
            this.cbReprintPrinter.Size = new System.Drawing.Size(180, 21);
            this.cbReprintPrinter.TabIndex = 6;
            // 
            // btReprintExportErrors
            // 
            this.btReprintExportErrors.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btReprintExportErrors.Location = new System.Drawing.Point(10, 313);
            this.btReprintExportErrors.Name = "btReprintExportErrors";
            this.btReprintExportErrors.Size = new System.Drawing.Size(180, 35);
            this.btReprintExportErrors.TabIndex = 4;
            this.btReprintExportErrors.Text = "Export Errors";
            this.btReprintExportErrors.UseVisualStyleBackColor = true;
            this.btReprintExportErrors.Click += new System.EventHandler(this.btReprintExportErrors_Click);
            // 
            // btReprintPrint
            // 
            this.btReprintPrint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btReprintPrint.Location = new System.Drawing.Point(10, 258);
            this.btReprintPrint.Name = "btReprintPrint";
            this.btReprintPrint.Size = new System.Drawing.Size(180, 35);
            this.btReprintPrint.TabIndex = 3;
            this.btReprintPrint.Text = "Print";
            this.btReprintPrint.UseVisualStyleBackColor = true;
            this.btReprintPrint.Click += new System.EventHandler(this.btReprintPrint_Click);
            // 
            // btReprintClear
            // 
            this.btReprintClear.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btReprintClear.Location = new System.Drawing.Point(10, 166);
            this.btReprintClear.Name = "btReprintClear";
            this.btReprintClear.Size = new System.Drawing.Size(180, 35);
            this.btReprintClear.TabIndex = 2;
            this.btReprintClear.Text = "Clear List";
            this.btReprintClear.UseVisualStyleBackColor = true;
            this.btReprintClear.Click += new System.EventHandler(this.btReprintClear_Click);
            // 
            // btAddReprintPackage
            // 
            this.btAddReprintPackage.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAddReprintPackage.Location = new System.Drawing.Point(10, 69);
            this.btAddReprintPackage.Name = "btAddReprintPackage";
            this.btAddReprintPackage.Size = new System.Drawing.Size(180, 35);
            this.btAddReprintPackage.TabIndex = 1;
            this.btAddReprintPackage.Text = "Add Invoices";
            this.btAddReprintPackage.UseVisualStyleBackColor = true;
            this.btAddReprintPackage.Click += new System.EventHandler(this.btAddReprintPackage_Click);
            // 
            // panel24
            // 
            this.panel24.Controls.Add(this.tbHelpReprint);
            this.panel24.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel24.Location = new System.Drawing.Point(3, 3);
            this.panel24.Name = "panel24";
            this.panel24.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel24.Size = new System.Drawing.Size(1170, 62);
            this.panel24.TabIndex = 5;
            // 
            // tbHelpReprint
            // 
            this.tbHelpReprint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbHelpReprint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHelpReprint.Location = new System.Drawing.Point(0, 0);
            this.tbHelpReprint.Multiline = true;
            this.tbHelpReprint.Name = "tbHelpReprint";
            this.tbHelpReprint.ReadOnly = true;
            this.tbHelpReprint.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbHelpReprint.Size = new System.Drawing.Size(1170, 59);
            this.tbHelpReprint.TabIndex = 0;
            // 
            // tpRepair
            // 
            this.tpRepair.Controls.Add(this.panel25);
            this.tpRepair.Controls.Add(this.panel28);
            this.tpRepair.Controls.Add(this.panel29);
            this.tpRepair.Location = new System.Drawing.Point(4, 28);
            this.tpRepair.Name = "tpRepair";
            this.tpRepair.Padding = new System.Windows.Forms.Padding(3);
            this.tpRepair.Size = new System.Drawing.Size(1176, 505);
            this.tpRepair.TabIndex = 5;
            this.tpRepair.Text = "Repair";
            this.tpRepair.UseVisualStyleBackColor = true;
            // 
            // panel25
            // 
            this.panel25.Controls.Add(this.dgvRepair);
            this.panel25.Controls.Add(this.panel26);
            this.panel25.Controls.Add(this.panel27);
            this.panel25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel25.Location = new System.Drawing.Point(3, 65);
            this.panel25.Name = "panel25";
            this.panel25.Size = new System.Drawing.Size(970, 437);
            this.panel25.TabIndex = 0;
            // 
            // dgvRepair
            // 
            this.dgvRepair.AllowUserToAddRows = false;
            this.dgvRepair.AllowUserToDeleteRows = false;
            this.dgvRepair.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRepair.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvRepair.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRepair.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRepair.Location = new System.Drawing.Point(0, 28);
            this.dgvRepair.Name = "dgvRepair";
            this.dgvRepair.ReadOnly = true;
            this.dgvRepair.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRepair.Size = new System.Drawing.Size(970, 381);
            this.dgvRepair.TabIndex = 4;
            // 
            // panel26
            // 
            this.panel26.Controls.Add(this.ssRepair);
            this.panel26.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel26.Location = new System.Drawing.Point(0, 409);
            this.panel26.Name = "panel26";
            this.panel26.Size = new System.Drawing.Size(970, 28);
            this.panel26.TabIndex = 3;
            // 
            // ssRepair
            // 
            this.ssRepair.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssRepair.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslRepairTotal,
            this.tsslRepairNonrepaired,
            this.tsslRepairRepaired});
            this.ssRepair.Location = new System.Drawing.Point(0, 0);
            this.ssRepair.Name = "ssRepair";
            this.ssRepair.Size = new System.Drawing.Size(970, 28);
            this.ssRepair.SizingGrip = false;
            this.ssRepair.TabIndex = 3;
            this.ssRepair.Text = "statusStrip2";
            // 
            // tsslRepairTotal
            // 
            this.tsslRepairTotal.Name = "tsslRepairTotal";
            this.tsslRepairTotal.Size = new System.Drawing.Size(118, 23);
            this.tsslRepairTotal.Text = "toolStripStatusLabel1";
            // 
            // tsslRepairNonrepaired
            // 
            this.tsslRepairNonrepaired.Name = "tsslRepairNonrepaired";
            this.tsslRepairNonrepaired.Size = new System.Drawing.Size(118, 23);
            this.tsslRepairNonrepaired.Text = "toolStripStatusLabel2";
            // 
            // tsslRepairRepaired
            // 
            this.tsslRepairRepaired.Name = "tsslRepairRepaired";
            this.tsslRepairRepaired.Size = new System.Drawing.Size(118, 23);
            this.tsslRepairRepaired.Text = "toolStripStatusLabel3";
            // 
            // panel27
            // 
            this.panel27.BackColor = System.Drawing.SystemColors.Control;
            this.panel27.Controls.Add(this.chkRepairShowRepaired);
            this.panel27.Controls.Add(this.chkRepairShowNonrepaired);
            this.panel27.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel27.Location = new System.Drawing.Point(0, 0);
            this.panel27.Name = "panel27";
            this.panel27.Size = new System.Drawing.Size(970, 28);
            this.panel27.TabIndex = 1;
            // 
            // chkRepairShowRepaired
            // 
            this.chkRepairShowRepaired.AutoSize = true;
            this.chkRepairShowRepaired.Checked = true;
            this.chkRepairShowRepaired.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRepairShowRepaired.Location = new System.Drawing.Point(134, 6);
            this.chkRepairShowRepaired.Name = "chkRepairShowRepaired";
            this.chkRepairShowRepaired.Size = new System.Drawing.Size(104, 17);
            this.chkRepairShowRepaired.TabIndex = 7;
            this.chkRepairShowRepaired.Text = "Show Repaired";
            this.chkRepairShowRepaired.UseVisualStyleBackColor = true;
            // 
            // chkRepairShowNonrepaired
            // 
            this.chkRepairShowNonrepaired.AutoSize = true;
            this.chkRepairShowNonrepaired.Checked = true;
            this.chkRepairShowNonrepaired.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRepairShowNonrepaired.Location = new System.Drawing.Point(5, 6);
            this.chkRepairShowNonrepaired.Name = "chkRepairShowNonrepaired";
            this.chkRepairShowNonrepaired.Size = new System.Drawing.Size(123, 17);
            this.chkRepairShowNonrepaired.TabIndex = 6;
            this.chkRepairShowNonrepaired.Text = "Show Nonrepaired";
            this.chkRepairShowNonrepaired.UseVisualStyleBackColor = true;
            this.chkRepairShowNonrepaired.CheckedChanged += new System.EventHandler(this.OnRepairFilterClick);
            // 
            // panel28
            // 
            this.panel28.BackColor = System.Drawing.SystemColors.Control;
            this.panel28.Controls.Add(this.btRepairExportPackagesErrors);
            this.panel28.Controls.Add(this.btRepairExportErrors);
            this.panel28.Controls.Add(this.btRepair);
            this.panel28.Controls.Add(this.btRepairClearSelection);
            this.panel28.Controls.Add(this.btRepairSelectAll);
            this.panel28.Controls.Add(this.btRepairReload);
            this.panel28.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel28.Location = new System.Drawing.Point(973, 65);
            this.panel28.Name = "panel28";
            this.panel28.Size = new System.Drawing.Size(200, 437);
            this.panel28.TabIndex = 1;
            // 
            // btRepairExportPackagesErrors
            // 
            this.btRepairExportPackagesErrors.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRepairExportPackagesErrors.Location = new System.Drawing.Point(10, 218);
            this.btRepairExportPackagesErrors.Name = "btRepairExportPackagesErrors";
            this.btRepairExportPackagesErrors.Size = new System.Drawing.Size(180, 35);
            this.btRepairExportPackagesErrors.TabIndex = 6;
            this.btRepairExportPackagesErrors.Text = "Export Packages Errors";
            this.btRepairExportPackagesErrors.UseVisualStyleBackColor = true;
            this.btRepairExportPackagesErrors.Click += new System.EventHandler(this.btRepairExportPackagesErrors_Click);
            // 
            // btRepairExportErrors
            // 
            this.btRepairExportErrors.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRepairExportErrors.Location = new System.Drawing.Point(10, 272);
            this.btRepairExportErrors.Name = "btRepairExportErrors";
            this.btRepairExportErrors.Size = new System.Drawing.Size(180, 35);
            this.btRepairExportErrors.TabIndex = 5;
            this.btRepairExportErrors.Text = "Export Errors";
            this.btRepairExportErrors.UseVisualStyleBackColor = true;
            this.btRepairExportErrors.Click += new System.EventHandler(this.btRepairExportErrors_Click);
            // 
            // btRepair
            // 
            this.btRepair.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRepair.Location = new System.Drawing.Point(10, 177);
            this.btRepair.Name = "btRepair";
            this.btRepair.Size = new System.Drawing.Size(180, 35);
            this.btRepair.TabIndex = 4;
            this.btRepair.Text = "Repair";
            this.btRepair.UseVisualStyleBackColor = true;
            this.btRepair.Click += new System.EventHandler(this.btRepair_Click);
            // 
            // btRepairClearSelection
            // 
            this.btRepairClearSelection.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRepairClearSelection.Location = new System.Drawing.Point(10, 121);
            this.btRepairClearSelection.Name = "btRepairClearSelection";
            this.btRepairClearSelection.Size = new System.Drawing.Size(180, 35);
            this.btRepairClearSelection.TabIndex = 3;
            this.btRepairClearSelection.Text = "Clear Selection";
            this.btRepairClearSelection.UseVisualStyleBackColor = true;
            this.btRepairClearSelection.Click += new System.EventHandler(this.btRepairClearSelection_Click);
            // 
            // btRepairSelectAll
            // 
            this.btRepairSelectAll.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRepairSelectAll.Location = new System.Drawing.Point(10, 80);
            this.btRepairSelectAll.Name = "btRepairSelectAll";
            this.btRepairSelectAll.Size = new System.Drawing.Size(180, 35);
            this.btRepairSelectAll.TabIndex = 2;
            this.btRepairSelectAll.Text = "Select All";
            this.btRepairSelectAll.UseVisualStyleBackColor = true;
            this.btRepairSelectAll.Click += new System.EventHandler(this.btRepairSelectAll_Click);
            // 
            // btRepairReload
            // 
            this.btRepairReload.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRepairReload.Location = new System.Drawing.Point(10, 28);
            this.btRepairReload.Name = "btRepairReload";
            this.btRepairReload.Size = new System.Drawing.Size(180, 35);
            this.btRepairReload.TabIndex = 0;
            this.btRepairReload.Text = "Reload List";
            this.btRepairReload.UseVisualStyleBackColor = true;
            this.btRepairReload.Click += new System.EventHandler(this.btRepairReload_Click);
            // 
            // panel29
            // 
            this.panel29.Controls.Add(this.tbHelpRepair);
            this.panel29.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel29.Location = new System.Drawing.Point(3, 3);
            this.panel29.Name = "panel29";
            this.panel29.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel29.Size = new System.Drawing.Size(1170, 62);
            this.panel29.TabIndex = 6;
            // 
            // tbHelpRepair
            // 
            this.tbHelpRepair.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbHelpRepair.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHelpRepair.Location = new System.Drawing.Point(0, 0);
            this.tbHelpRepair.Multiline = true;
            this.tbHelpRepair.Name = "tbHelpRepair";
            this.tbHelpRepair.ReadOnly = true;
            this.tbHelpRepair.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbHelpRepair.Size = new System.Drawing.Size(1170, 59);
            this.tbHelpRepair.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 561);
            this.Controls.Add(this.tcQueries);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1200, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "W3 PACK";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tpTemplate.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubset)).EndInit();
            this.cmsSubset.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ssSubset.ResumeLayout(false);
            this.ssSubset.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuery)).EndInit();
            this.cmsSet.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ssSet.ResumeLayout(false);
            this.ssSet.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel21.ResumeLayout(false);
            this.panel21.PerformLayout();
            this.tcQueries.ResumeLayout(false);
            this.cmsRepair.ResumeLayout(false);
            this.tpUnship.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnship)).EndInit();
            this.cmsUnship.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.ssUnship.ResumeLayout(false);
            this.ssUnship.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel23.ResumeLayout(false);
            this.panel23.PerformLayout();
            this.tpUnshipped.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnshipped)).EndInit();
            this.cmsUnshipped.ResumeLayout(false);
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            this.ssUnshipped.ResumeLayout(false);
            this.ssUnshipped.PerformLayout();
            this.panel16.ResumeLayout(false);
            this.panel16.PerformLayout();
            this.panel22.ResumeLayout(false);
            this.panel22.PerformLayout();
            this.tpReprint.ResumeLayout(false);
            this.panel17.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReprint)).EndInit();
            this.cmsReprint.ResumeLayout(false);
            this.panel18.ResumeLayout(false);
            this.panel18.PerformLayout();
            this.ssReprint.ResumeLayout(false);
            this.ssReprint.PerformLayout();
            this.panel19.ResumeLayout(false);
            this.panel19.PerformLayout();
            this.panel20.ResumeLayout(false);
            this.panel20.PerformLayout();
            this.panel24.ResumeLayout(false);
            this.panel24.PerformLayout();
            this.tpRepair.ResumeLayout(false);
            this.panel25.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRepair)).EndInit();
            this.panel26.ResumeLayout(false);
            this.panel26.PerformLayout();
            this.ssRepair.ResumeLayout(false);
            this.ssRepair.PerformLayout();
            this.panel27.ResumeLayout(false);
            this.panel27.PerformLayout();
            this.panel28.ResumeLayout(false);
            this.panel29.ResumeLayout(false);
            this.panel29.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.TabPage tpTemplate;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.Panel panel5;
    private System.Windows.Forms.Panel panel4;
    private System.Windows.Forms.CheckBox cbQueryShowError;
    private System.Windows.Forms.CheckBox cbQueryShowPrinted;
    private System.Windows.Forms.CheckBox cbQueryShowUnprinted;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel6;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.ComboBox cbSubset;
    private System.Windows.Forms.Panel panel7;
    private System.Windows.Forms.Panel panel8;
    private System.Windows.Forms.ContextMenuStrip cmsSet;
    private System.Windows.Forms.ToolStripMenuItem addToCustomSubsetToolStripMenuItem;
    private System.Windows.Forms.ContextMenuStrip cmsSubset;
    private System.Windows.Forms.ToolStripMenuItem miRemoveFromSubset;
    private System.Windows.Forms.ToolStripMenuItem miPreviewSubsetInvoice;
    private System.Windows.Forms.ToolStripMenuItem miPreviewSetInvoice;
    private System.Windows.Forms.ToolStripMenuItem invoiceToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem miFindInvoice;
    private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem miFindStartIndex;
    private System.Windows.Forms.DataGridView dgvQuery;
    private System.Windows.Forms.DataGridView dgvSubset;
    private System.Windows.Forms.SaveFileDialog sfdExportErrors;
    private System.Windows.Forms.StatusStrip ssSet;
    private System.Windows.Forms.ToolStripStatusLabel tsslSetTotal;
    private System.Windows.Forms.ToolStripStatusLabel tsslSetUnprinted;
    private System.Windows.Forms.ToolStripStatusLabel tsslSetPrinted;
    private System.Windows.Forms.ToolStripStatusLabel tsslSetFailed;
    private System.Windows.Forms.TabControl tcQueries;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btExportErrors;
    private System.Windows.Forms.ComboBox cbPrinter;
    private System.Windows.Forms.Button btPrint;
    private System.Windows.Forms.TabPage tpUnship;
    private System.Windows.Forms.Panel panel10;
    private System.Windows.Forms.Panel panel9;
    private System.Windows.Forms.Panel panel11;
    private System.Windows.Forms.CheckBox chkUnshipFilterShowFailed;
    private System.Windows.Forms.CheckBox chkUnshipFilterShowUnshipped;
    private System.Windows.Forms.CheckBox chkUnshipFilterShowShipped;
    private System.Windows.Forms.Panel panel12;
    private System.Windows.Forms.StatusStrip ssUnship;
    private System.Windows.Forms.Button btAddUnshipInvoice;
    private System.Windows.Forms.DataGridView dgvUnship;
    private System.Windows.Forms.Button btExportUnshipErrors;
    private System.Windows.Forms.Button btUnship;
    private System.Windows.Forms.ToolStripStatusLabel tsslUnshipTotal;
    private System.Windows.Forms.ToolStripStatusLabel tsslUnshipShipped;
    private System.Windows.Forms.ToolStripStatusLabel tsslUnshipUnshipped;
    private System.Windows.Forms.ToolStripStatusLabel tsslUnshipFailed;
    private System.Windows.Forms.ContextMenuStrip cmsUnship;
    private System.Windows.Forms.ToolStripMenuItem miUnshipRemoveSelected;
    private System.Windows.Forms.Button btUnshipClear;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.StatusStrip ssSubset;
    private System.Windows.Forms.ToolStripStatusLabel tsslSubsetTotal;
    private System.Windows.Forms.ToolStripStatusLabel tsslSubsetUnprinted;
    private System.Windows.Forms.ToolStripStatusLabel tsslSubsetPrinted;
    private System.Windows.Forms.ToolStripStatusLabel tsslSubsetFailed;
    private System.Windows.Forms.ToolStripMenuItem miFileExit;
    private System.Windows.Forms.TabPage tpUnshipped;
    private System.Windows.Forms.Panel panel13;
    private System.Windows.Forms.DataGridView dgvUnshipped;
    private System.Windows.Forms.Panel panel14;
    private System.Windows.Forms.StatusStrip ssUnshipped;
    private System.Windows.Forms.Panel panel15;
    private System.Windows.Forms.Panel panel16;
    private System.Windows.Forms.TabPage tpReprint;
    private System.Windows.Forms.Panel panel17;
    private System.Windows.Forms.DataGridView dgvReprint;
    private System.Windows.Forms.Panel panel18;
    private System.Windows.Forms.StatusStrip ssReprint;
    private System.Windows.Forms.Panel panel19;
    private System.Windows.Forms.Panel panel20;
    private System.Windows.Forms.Button btReloarUnshipped;
    private System.Windows.Forms.Button btUnshippedSetPickable;
    private System.Windows.Forms.Button btUnshippedResetStatus;
    private System.Windows.Forms.Button btUnshippedClearSelection;
    private System.Windows.Forms.Button btUnshippedSelectAll;
    private System.Windows.Forms.Button btAddReprintPackage;
    private System.Windows.Forms.ContextMenuStrip cmsReprint;
    private System.Windows.Forms.ToolStripMenuItem tsmiReprintRemoveSelected;
    private System.Windows.Forms.Button btReprintExportErrors;
    private System.Windows.Forms.Button btReprintPrint;
    private System.Windows.Forms.Button btReprintClear;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox cbReprintPrinter;
    private System.Windows.Forms.ToolStripMenuItem miPreviewReprintInvoice;
    private System.Windows.Forms.CheckBox cbQueryShowLocked;
    private System.Windows.Forms.ToolStripStatusLabel tsslSubsetLocked;
    private System.Windows.Forms.ToolStripStatusLabel tsslSetLocked;
    private System.Windows.Forms.TabPage tpRepair;
    private System.Windows.Forms.Panel panel25;
    private System.Windows.Forms.DataGridView dgvRepair;
    private System.Windows.Forms.Panel panel26;
    private System.Windows.Forms.StatusStrip ssRepair;
    private System.Windows.Forms.Panel panel27;
    private System.Windows.Forms.Panel panel28;
    private System.Windows.Forms.Button btRepairClearSelection;
    private System.Windows.Forms.Button btRepairSelectAll;
    private System.Windows.Forms.Button btRepairReload;
    private System.Windows.Forms.Button btRepair;
    private System.Windows.Forms.Button btRepairExportErrors;
    private System.Windows.Forms.Button btRepairExportPackagesErrors;
    private System.Windows.Forms.Button btReprintLastBatches;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button btSetPmodMaxDailyPackages;
    private System.Windows.Forms.TextBox edPmodMaxDailyPackages;
    private System.Windows.Forms.Button btUnshipLastBatches;
    private System.Windows.Forms.Button btReload;
    private System.Windows.Forms.Button btOnHold;
    private System.Windows.Forms.ToolStripMenuItem miSubsetAddToUnship;
    private System.Windows.Forms.Panel panel21;
    private System.Windows.Forms.TextBox tbHelp;
    private System.Windows.Forms.Panel panel22;
    private System.Windows.Forms.TextBox tbHelpUnshipped;
    private System.Windows.Forms.Panel panel23;
    private System.Windows.Forms.TextBox tbHelpUnship;
    private System.Windows.Forms.Panel panel24;
    private System.Windows.Forms.TextBox tbHelpReprint;
    private System.Windows.Forms.Panel panel29;
    private System.Windows.Forms.TextBox tbHelpRepair;
    private System.Windows.Forms.ToolStripMenuItem miSubsetCopyToClipboard;
    private System.Windows.Forms.ToolStripMenuItem miSetCopyToClipboard;
    private System.Windows.Forms.ToolStripMenuItem miUnshipCopyToClipboard;
    private System.Windows.Forms.ToolStripMenuItem miReprintCopyToClipboard;
    private System.Windows.Forms.ContextMenuStrip cmsUnshipped;
    private System.Windows.Forms.ToolStripMenuItem miUnshippedCopyToClipboard;
    private System.Windows.Forms.ContextMenuStrip cmsRepair;
    private System.Windows.Forms.ToolStripMenuItem miRepairCopyToClipboard;
    private System.Windows.Forms.CheckBox chkUnshippedShowUnshipped;
    private System.Windows.Forms.CheckBox chkUnshippedShowNew;
    private System.Windows.Forms.CheckBox chkUnshippedShowUpdated;
    private System.Windows.Forms.ToolStripStatusLabel tsslUnshippedTotal;
    private System.Windows.Forms.ToolStripStatusLabel tsslNonupdated;
    private System.Windows.Forms.ToolStripStatusLabel tsslUnshippedUpdated;
    private System.Windows.Forms.CheckBox chkReprintShowLocked;
    private System.Windows.Forms.CheckBox chkReprintShowFailed;
    private System.Windows.Forms.CheckBox chkReprintShowPrinted;
    private System.Windows.Forms.CheckBox chkReprintShowUnprinted;
    private System.Windows.Forms.ToolStripStatusLabel tsslReprintTotal;
    private System.Windows.Forms.ToolStripStatusLabel tsslReprintUnprinted;
    private System.Windows.Forms.ToolStripStatusLabel tsslReprintPrinted;
    private System.Windows.Forms.ToolStripStatusLabel tsslReprintFailed;
    private System.Windows.Forms.ToolStripStatusLabel tsslReprintLocked;
    private System.Windows.Forms.ToolStripStatusLabel tsslRepairTotal;
    private System.Windows.Forms.ToolStripStatusLabel tsslRepairNonrepaired;
    private System.Windows.Forms.ToolStripStatusLabel tsslRepairRepaired;
    private System.Windows.Forms.CheckBox chkRepairShowRepaired;
    private System.Windows.Forms.CheckBox chkRepairShowNonrepaired;
    private System.Windows.Forms.CheckBox chkUnshippedShowFailed;
    private System.Windows.Forms.ToolStripStatusLabel tsslUnshippedFailed;
    private System.Windows.Forms.CheckBox chkUnshippedShowNonupdated;
    private System.Windows.Forms.ToolStripMenuItem miPreviewSubsetInvoiceWithSequenceNumber;
    private System.Windows.Forms.CheckBox chkPrintSequenceNumber;
    private System.Windows.Forms.ToolStripMenuItem previewPackJacketInvoiceToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem previewPackJacketInvoiceWithSequenceNumberToolStripMenuItem;
    private System.Windows.Forms.CheckBox chkReprintSequenceNumber;

  }
}

