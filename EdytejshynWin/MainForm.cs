using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Edytejshyn.Logic;

namespace Edytejshyn
{
    public partial class MainForm : Form
    {

        #region Variables
        private readonly string _appName = "Edytejszyn (Build " + Assembly.GetExecutingAssembly().GetName().Version + ")";
        private bool _dataChanged = false;
        private OpenFileDialog _openDialog;
        private SaveFileDialog _saveDialog;
        private GUIExceptionHandler _exceptionHandler;

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
            this._exceptionHandler = new GUIExceptionHandler(this);
            InitializeComponent();
            UpdateTitle();
            SetFileControlsEnabled(false);
            _openDialog = new OpenFileDialog();
            _saveDialog = new SaveFileDialog();
            _openDialog.Filter = _saveDialog.Filter = "XML file (*.*)|*.xml|All files (*.*)|*.*";
            this.Logic.Logger.LogEvent += ShowLogMessage;
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
            if (!this.DataChanged) return true;
            DialogResult result = MessageBox.Show("Data has been changed. Shall I save?", "Possible data lose", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            switch (result)
            {
                case DialogResult.Yes:      return SaveFile();
                case DialogResult.No:       return true;
                case DialogResult.Cancel:   return false;
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
                _exceptionHandler.HandleException(ex);
            }
            return false;
        }


        /// <summary>
        /// Saves opened data in new file and switches to that file as current.
        /// </summary>
        /// <returns><code>true</code> value if saved correctly.</returns>
        private bool SaveFileAs()
        {
            DialogResult result = _saveDialog.ShowDialog();
            if (result != DialogResult.OK) return false;
            try
            {
                this.Logic.SaveFile(_saveDialog.FileName);
                this.DataChanged = false;
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
            }
            return false;
        }

        /// <summary>
        /// Updates title caption in editor application.
        /// </summary>
        private void UpdateTitle()
        {
            StringBuilder sb = new StringBuilder();
            string file = Logic.SimpleFileName;
            if (file != null)
            {
                sb.Append(file);
                if (this.DataChanged) sb.Append("* ");
                sb.Append(" - ");
            }
            sb.Append(this._appName);
            this.Text = sb.ToString();
        }

        private void SetFileControlsEnabled(bool mode)
        {
            saveMenuItem.Enabled = mode;
            saveAsMenuItem.Enabled = mode;
        }

        public void ShowLogMessage(LoggerLevel level, string message)
        {
            if(level != LoggerLevel.Debug)
                statusBarLabel.Text = string.Format("{0}: {1}", level, message);
            statusBarLabel.ForeColor = level.GetColor(DefaultForeColor);
        }

        #region Events
        private void ExitEvent(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OpenEvent(object sender, EventArgs e)
        {
            DialogResult result = this._openDialog.ShowDialog();
            if (result != DialogResult.OK) return;
            try
            {
                this.Logic.LoadFile(this._openDialog.FileName);
                SetFileControlsEnabled(true);
                this.DataChanged = false;
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
            }
        }

        private void SaveEvent(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveAsEvent(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox.Instance.ShowDialog();
        }

        private void TreeViewObjects_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                treeViewObjects.SelectedNode.Remove();
                this.DataChanged = true;
            }
        }
        #endregion



        #endregion

    }
}
