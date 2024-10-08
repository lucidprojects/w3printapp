﻿using System;
using System.Reflection;
using System.Windows.Forms;

namespace PrintInvoice.UI
{
    public partial class SetMaxDailyPackagesForm : Form
    {
        public SetMaxDailyPackagesForm()
        {
            InitializeComponent();
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly()?.Location ?? "");
        }

        public int Value
        {
            set => edValue.Text = value.ToString();
            get => int.Parse(edValue.Text);
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btSet_Click(object sender, EventArgs e)
        {
            try
            {
                int.Parse(edValue.Text);
                DialogResult = DialogResult.OK;
            }
            catch (Exception)
            {
                MessageBox.Show(this, @"Please enter correct integer value", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                edValue.Focus();
                edValue.SelectAll();
            }
        }
    }
}