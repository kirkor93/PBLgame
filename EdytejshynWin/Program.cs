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
            string contentFile = @"..\..\..\PBLgame\PBLgame\content.xml";
            string sceneFile = null;
            if (args.Length >= 1)
            {
                contentFile = args[0];
                if (args.Length >= 2)
                {
                    sceneFile = args[1];
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(new EditorLogic(), contentFile, sceneFile));
        }
    }
}
