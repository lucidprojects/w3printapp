using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security;

namespace PrintInvoice
{
  public partial class LastBatchesForm : Form
  {
    private Dictionary<int, int> batchIdIndex = new Dictionary<int, int>();
    private LabelService client;
    private Config config;

    public LastBatchesForm(LabelService aClient, Config aConfig)
    {
      InitializeComponent();

      client = aClient;
      config = aConfig;
    }

    public int BatchId
    {
      get { return batchIdIndex[lbBatches.SelectedIndex];}
    }

    private void btSelect_Click(object sender, EventArgs e)
    {
      if (lbBatches.SelectedIndex >= 0)
      {
        DialogResult = DialogResult.OK;
      }
      else 
      {
        MessageBox.Show(
          this,
          "No one batch selected.",
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
          );
      }
    }

    private void btCancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
    }

    private void lbBatches_DoubleClick(object sender, EventArgs e)
    {
      if (lbBatches.SelectedIndex >= 0)
      { 
        btSelect_Click(btSelect, new EventArgs());
      }
    }

    public int load()
    {
      RunSqlQueryRequestType request = new RunSqlQueryRequestType();
      request.query = SecurityElement.Escape(config.LastBatchesQuery);
      request.clientVersion = Routines.getVersion();
      RunSqlQueryResponseType response = client.runSqlQuery(request);
      if (response.status != 0)
      {
        throw new Exception(String.Format("Label service returns error status\nStatus: {0}\nMessage: {1}\nSubstatus: {2}\nSubmessage: {3}", response.status, response.message, response.substatus, response.submessage));
      }

      lbBatches.Items.Clear();
      batchIdIndex.Clear();

      if (response.rows != null)
      {
        foreach (RowType row in response.rows)
        {
          int packageId = Int32.Parse(row.columns[0]);
          int itemIndex = lbBatches.Items.Add(String.Format("{0} by {1}", row.columns[1], row.columns[2]));
          batchIdIndex[itemIndex] = packageId;
        }
      }

      return lbBatches.Items.Count;
    }
  }
}