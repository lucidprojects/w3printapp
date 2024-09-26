namespace PrintInvoice
{
  partial class LastBatchesForm
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
      this.lbBatches = new System.Windows.Forms.ListBox();
      this.btSelect = new System.Windows.Forms.Button();
      this.btCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lbBatches
      // 
      this.lbBatches.FormattingEnabled = true;
      this.lbBatches.Location = new System.Drawing.Point(12, 12);
      this.lbBatches.Name = "lbBatches";
      this.lbBatches.Size = new System.Drawing.Size(268, 316);
      this.lbBatches.TabIndex = 0;
      this.lbBatches.DoubleClick += new System.EventHandler(this.lbBatches_DoubleClick);
      // 
      // btSelect
      // 
      this.btSelect.Location = new System.Drawing.Point(36, 349);
      this.btSelect.Name = "btSelect";
      this.btSelect.Size = new System.Drawing.Size(100, 23);
      this.btSelect.TabIndex = 1;
      this.btSelect.Text = "Select";
      this.btSelect.UseVisualStyleBackColor = true;
      this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
      // 
      // btCancel
      // 
      this.btCancel.Location = new System.Drawing.Point(155, 349);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(100, 23);
      this.btCancel.TabIndex = 2;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
      // 
      // LastBatchesForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(292, 384);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btSelect);
      this.Controls.Add(this.lbBatches);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "LastBatchesForm";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Last Batches";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btSelect;
    private System.Windows.Forms.Button btCancel;
    public System.Windows.Forms.ListBox lbBatches;
  }
}