namespace PrintInvoice
{
  partial class SetMaxDailyPackagesForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.edValue = new System.Windows.Forms.TextBox();
            this.btSet = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Value";
            // 
            // edValue
            // 
            this.edValue.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edValue.Location = new System.Drawing.Point(23, 40);
            this.edValue.Name = "edValue";
            this.edValue.Size = new System.Drawing.Size(206, 23);
            this.edValue.TabIndex = 1;
            // 
            // btSet
            // 
            this.btSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSet.Location = new System.Drawing.Point(23, 74);
            this.btSet.Name = "btSet";
            this.btSet.Size = new System.Drawing.Size(100, 35);
            this.btSet.TabIndex = 2;
            this.btSet.Text = "Set";
            this.btSet.UseVisualStyleBackColor = true;
            this.btSet.Click += new System.EventHandler(this.btSet_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCancel.Location = new System.Drawing.Point(129, 74);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(100, 35);
            this.btCancel.TabIndex = 3;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // SetMaxDailyPackagesForm
            // 
            this.AcceptButton = this.btSet;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(254, 129);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btSet);
            this.Controls.Add(this.edValue);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetMaxDailyPackagesForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Max Daily Packages";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox edValue;
    private System.Windows.Forms.Button btSet;
    private System.Windows.Forms.Button btCancel;
  }
}