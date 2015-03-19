using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Edytejshyn.Logic;
// ReSharper disable All

namespace Edytejshyn
{
    public partial class MainForm : Form
    {

        #region Variables
        private readonly string appName = "Edytejszyn (Build " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
        private bool _dataChanged = false;
        private OpenFileDialog openDialog;
        private SaveFileDialog saveDialog;
        #endregion

        #region Properties
        public bool DataChanged
        {
            get
            {
                return _dataChanged;
            }
            set
            {
                _dataChanged = value;
                UpdateTitle();
            }
        }

        public EditorLogic Logic
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public MainForm(EditorLogic logic)
        {
            this.Logic = logic;
            InitializeComponent();
            UpdateTitle();
            this.openDialog = new OpenFileDialog();
            this.saveDialog = new SaveFileDialog();
            openDialog.Filter = saveDialog.Filter = "XML file (*.*)|*.xml|All files (*.*)|*.*";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!IsSafeToUnload())
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Call me before possible data lose like closing program or loading other file.
        /// If there are changes, user will be asked for saving.
        /// </summary>
        /// <returns>true if safe to close</returns>
        private bool IsSafeToUnload()
        {
            if(this.DataChanged)
            {
                DialogResult result = MessageBox.Show("Data has been changed. Shall I save?", "Possible data lose", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (result)
                {
                    case DialogResult.Yes:      return SaveFile();
                    case DialogResult.No:       return true;
                    case DialogResult.Cancel:   return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Saves currently opened file.
        /// </summary>
        /// <returns><code>true</code> value if saved correctly.</returns>
        private bool SaveFile()
        {
            try
            {
                this.Logic.SaveFile();
                this.DataChanged = false;
                return true;
            }
            catch(Exception ex)
            {
                GUIExceptionHandler.ShowMessage(ex);
            }
            return false;
        }

        /// <summary>
        /// Updates title caption in editor application.
        /// </summary>
        private void UpdateTitle()
        {
            StringBuilder sb = new StringBuilder();
            string file = Logic.FilePath;
            if (file != null)
            {
                sb.Append(file);
                if (this.DataChanged) sb.Append("* ");
                sb.Append(" - ");
            }
            sb.Append(this.appName);
            this.Text = sb.ToString();
        }

        #region Events
        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion


    }
}
