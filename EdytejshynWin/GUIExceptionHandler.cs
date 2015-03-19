using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Edytejshyn.Logic;

namespace Edytejshyn
{
    public class GUIExceptionHandler
    {
        private MainForm _form;

        public GUIExceptionHandler(MainForm form)
        {
            this._form = form;
        }

        #region Methods
        public void HandleException(Exception ex)
        {
            StringBuilder sb = new StringBuilder(ex.Message);
            sb.AppendLine().AppendLine().Append(ex.ToString());
            if (ex.InnerException != null)
            {
                sb.AppendLine("Caused by:").AppendLine().Append(ex.InnerException.ToString());
            }
            MessageBox.Show(sb.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            _form.Logic.Logger.Log(LoggerLevel.Error, ex.Message);
        }
        #endregion
    }
}
