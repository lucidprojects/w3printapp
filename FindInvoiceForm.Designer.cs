namespace PrintInvoice
{
  partial class FindInvoiceForm
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
      this.edFindValue = new System.Windows.Forms.TextBox();
      this.btFind = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(82, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Invoice Number";
      // 
      // edFindValue
      // 
      this.edFindValue.Location = new System.Drawing.Point(15, 25);
      this.edFindValue.Name = "edFindValue";
      this.edFindValue.Size = new System.Drawing.Size(217, 20);
      this.edFindValue.TabIndex = 1;
      // 
      // btFind
      // 
      this.btFind.Location = new System.Drawing.Point(132, 52);
      this.btFind.Name = "btFind";
      this.btFind.Size = new System.Drawing.Size(100, 23);
      this.btFind.TabIndex = 2;
      this.btFind.Text = "Find";
      this.btFind.UseVisualStyleBackColor = true;
      this.btFind.Click += new System.EventHandler(this.btFind_Click);
      // 
      // FindInvoiceForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(244, 87);
      this.Controls.Add(this.btFind);
      this.Controls.Add(this.edFindValue);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "FindInvoiceForm";
      this.ShowInTaskbar = false;
      this.Text = "Find Invoice";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindInvoiceForm_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox edFindValue;
    private System.Windows.Forms.Button btFind;
  }
}