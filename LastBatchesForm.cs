using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class LastBatchesForm : Form
    {
        private readonly Dictionary<int, int> _batchIdIndex = new Dictionary<int, int>();
        private readonly LabelService _client;
        private readonly Config _config;

        public LastBatchesForm(LabelService aClient, Config aConfig)
        {
            InitializeComponent();
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly()?.Location ?? "");

            _client = aClient;
            _config = aConfig;
        }

        public int BatchId => _batchIdIndex[lbBatches.SelectedIndex];

        private void btSelect_Click(object sender, EventArgs e)
        {
            if (lbBatches.SelectedIndex >= 0)
                DialogResult = DialogResult.OK;
            else
                this.ShowError(@"No one batch selected.");
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void lbBatches_DoubleClick(object sender, EventArgs e)
        {
            if (lbBatches.SelectedIndex >= 0) btSelect_Click(btSelect, EventArgs.Empty);
        }

        public int OnLoad()
        {
            var request = new RunSqlQueryRequestType
            {
                query = SecurityElement.Escape(_config.LastBatchesQuery),
                clientVersion = Routines.GetVersion()
            };

            var response = _client.runSqlQuery(request);

            if (response.status != 0)
                throw new Exception($"Label service returns error status\nStatus: {response.status}\nMessage: {response.message}\nSubstatus: {response.substatus}\nSubmessage: {response.submessage}");

            lbBatches.Items.Clear();
            _batchIdIndex.Clear();

            if (response.rows != null)
            {
                foreach (var row in response.rows)
                {
                    var packageId = int.Parse(row.columns[0]);
                    var itemIndex = lbBatches.Items.Add($"{row.columns[1]} by {row.columns[2]}");
                    _batchIdIndex[itemIndex] = packageId;
                }
            }

            return lbBatches.Items.Count;
        }
    }
}