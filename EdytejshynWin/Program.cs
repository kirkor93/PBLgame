using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Edytejshyn.Logic;

namespace Edytejshyn
{
    static class Program
    {
        public static EditorLogic UglyStaticLogic;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string contentFile = @"content.xml";
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
            UglyStaticLogic = new EditorLogic();
            Application.Run(new MainForm(UglyStaticLogic, contentFile, sceneFile));
        }
    }
}
