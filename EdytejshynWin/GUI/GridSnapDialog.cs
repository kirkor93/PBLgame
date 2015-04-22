using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Edytejshyn.GUI
{
    public partial class GridSnapDialog : Form
    {
        private readonly MainForm _parent;

        public GridSnapDialog(MainForm parent)
        {
            _parent = parent;
            InitializeComponent();
            gridStepTextBox.Text = _parent.ViewportControl.Grid.GridSpacing.ToString();
            moveSnapTextBox.Text   = _parent.ViewportControl.Gizmo.TranslationSnapValue.ToString();
            rotateSnapTextBox.Text = _parent.ViewportControl.Gizmo.RotationSnapValue.ToString();
            scaleSnapTextBox.Text  = _parent.ViewportControl.Gizmo.ScaleSnapValue.ToString();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // TODO save changes
            try
            {
                _parent.ViewportControl.Grid.GridSpacing = float.Parse(gridStepTextBox.Text);
                _parent.ViewportControl.Gizmo.TranslationSnapValue = float.Parse(moveSnapTextBox.Text);
                _parent.ViewportControl.Gizmo.RotationSnapValue = float.Parse(rotateSnapTextBox.Text);
                _parent.ViewportControl.Gizmo.ScaleSnapValue = float.Parse(scaleSnapTextBox.Text);
                Close();
            }
            catch (FormatException fe)
            {
                _parent.ExceptionHandler.HandleException(fe);
            }

        }
    }
}
