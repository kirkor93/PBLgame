using System;
using System.Windows.Forms;
using Edytejshyn.Logic;

namespace Edytejshyn
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string fileToOpen = null;
            if (args.Length >= 1)
            {
                fileToOpen = args[0];
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(new EditorLogic(), fileToOpen));
        }
    }
}
