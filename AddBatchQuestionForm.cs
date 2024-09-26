﻿using System;
using System.Windows.Forms;

namespace PrintInvoice
{
    public partial class AddBatchQuestionForm : Form
    {
        public enum Result
        {
            CANCEL,
            SINGLE,
            BATCH
        }

        public Result _result;

        public AddBatchQuestionForm()
        {
            InitializeComponent();
        }

        private void btSingle_Click(object sender, EventArgs e)
        {
            _result = Result.SINGLE;
            Close();
        }

        private void btBatch_Click(object sender, EventArgs e)
        {
            _result = Result.BATCH;
            Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            _result = Result.CANCEL;
            Close();
        }
    }
}