using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Edytejshyn
{
    public static class GUIExceptionHandler
    {
        #region Methods
        public static void ShowMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder(ex.Message);
            sb.AppendLine().AppendLine().Append(ex.ToString());
            if (ex.InnerException != null)
            {
                sb.AppendLine("Caused by:").AppendLine().Append(ex.InnerException.ToString());
            }
            MessageBox.Show(sb.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
    }
}
