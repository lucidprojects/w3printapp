namespace PrintInvoice
{
  partial class RepeatedPrintForm
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
            this.laMessage = new System.Windows.Forms.Label();
            this.btPrint = new System.Windows.Forms.Button();
            this.btDontPrint = new System.Windows.Forms.Button();
            this.ckDontAsk = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // laMessage
            // 
            this.laMessage.Location = new System.Drawing.Point(20, 20);
            this.laMessage.Name = "laMessage";
            this.laMessage.Size = new System.Drawing.Size(260, 55);
            this.laMessage.TabIndex = 0;
            this.laMessage.Text = "label1";
            this.laMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btPrint
            // 
            this.btPrint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btPrint.Location = new System.Drawing.Point(46, 110);
            this.btPrint.Name = "btPrint";
            this.btPrint.Size = new System.Drawing.Size(100, 35);
            this.btPrint.TabIndex = 1;
            this.btPrint.Text = "Print";
            this.btPrint.UseVisualStyleBackColor = true;
            this.btPrint.Click += new System.EventHandler(this.btPrint_Click);
            // 
            // btDontPrint
            // 
            this.btDontPrint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btDontPrint.Location = new System.Drawing.Point(152, 110);
            this.btDontPrint.Name = "btDontPrint";
            this.btDontPrint.Size = new System.Drawing.Size(100, 35);
            this.btDontPrint.TabIndex = 2;
            this.btDontPrint.Text = "Don\'t print";
            this.btDontPrint.UseVisualStyleBackColor = true;
            this.btDontPrint.Click += new System.EventHandler(this.btDontPrint_Click);
            // 
            // ckDontAsk
            // 
            this.ckDontAsk.AutoSize = true;
            this.ckDontAsk.Location = new System.Drawing.Point(23, 82);
            this.ckDontAsk.Name = "ckDontAsk";
            this.ckDontAsk.Size = new System.Drawing.Size(114, 17);
            this.ckDontAsk.TabIndex = 3;
            this.ckDontAsk.Text = "Do not ask again";
            this.ckDontAsk.UseVisualStyleBackColor = true;
            // 
            // RepeatedPrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 165);
            this.Controls.Add(this.ckDontAsk);
            this.Controls.Add(this.btDontPrint);
            this.Controls.Add(this.btPrint);
            this.Controls.Add(this.laMessage);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RepeatedPrintForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Repeated print confirmation";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btPrint;
    private System.Windows.Forms.Button btDontPrint;
    public System.Windows.Forms.Label laMessage;
    public System.Windows.Forms.CheckBox ckDontAsk;
  }
}