using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace DatabaseCompare
{
    public partial class frmDatabaseCompare : Form
    {
        public frmDatabaseCompare()
        {
            InitializeComponent();
            GetConfigurationValues();
            cboJobType.DataSource = JobType.GetJobTypeList();
        }
        
        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                txtResults.Text = "";
                Application.DoEvents();
                Cursor.Current = Cursors.WaitCursor;
                ValidateInput();
                var jobSelection = (JobTypeEntry)cboJobType.SelectedValue;

                switch (jobSelection.JobTypeId)
                {
                    case JobTypeEnum.SchemaCompare:
                        var schemaCompare = new SchemaCompare();
                        txtResults.Text = schemaCompare.GetSchemaCompare(txtSourceServer.Text, txtSourceDatabase.Text, txtTargetServer.Text, txtTargetDatabase.Text);
                        break;
                    case JobTypeEnum.DataCompare:
                        var config = new DataCompareConfiguration(txtSourceServer.Text, txtSourceDatabase.Text, txtTargetServer.Text, txtTargetDatabase.Text);
                        var dataCompare = new DataCompare();
                        txtResults.Text = dataCompare.GetDataCompare(config);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                ExceptionHandler(exception);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void ValidateInput()
        {
            if (txtSourceServer.Text.Length == 0)
            {
                throw new ApplicationException("Source Server is blank");
            }
            if (txtSourceDatabase.Text.Length == 0)
            {
                throw new ApplicationException("Source Database is blank");
            }
            if (txtTargetServer.Text.Length == 0)
            {
                throw new ApplicationException("Target Server is blank");
            }
            if (txtTargetDatabase.Text.Length == 0)
            {
                throw new ApplicationException("Target Database is blank");
            }
        }

        private void ExceptionHandler(
            Exception exception
        )
        {
            string exceptionText = exception.Message + Environment.NewLine + exception.StackTrace;
            string caption = "Exception";
            MessageBoxButtons button = MessageBoxButtons.OK;
            MessageBox.Show(exceptionText, caption, button);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var fileSaver = new SaveFileDialog();
            fileSaver.Filter = "SQL Script | *.sql";
            fileSaver.FileName = "DatabaseCompare.sql";
            fileSaver.Title = "Save File";
            if (fileSaver.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(fileSaver.FileName, txtResults.Text);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtResults.Text);
        }

        private void GetConfigurationValues()
        {
            txtSourceServer.Text = ConfigurationManager.AppSettings["defaultSourceServer"];
            txtSourceDatabase.Text = ConfigurationManager.AppSettings["defaultSourceDatabase"];
            txtTargetServer.Text = ConfigurationManager.AppSettings["defaultDefaultServer"];
            txtTargetDatabase.Text = ConfigurationManager.AppSettings["defaultDefaultDatabase"];
        }
    }
}
