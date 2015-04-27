using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Edytejshyn.Logic;

namespace Edytejshyn
{
    static class Program
    {
        public static EditorLogic UglyStaticLogic { get; private set; }
        public static MainForm    UglyStaticMain  { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string contentFile = @"content.xml";
            string sceneFile = null;
            bool basicRender = false;

            int i;
            for (i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        ShowUsage();
                        return;

                    case "-c":
                    case "--content":
                        contentFile = GetOptionArgument(args, ref i, "Content");
                        if (contentFile == null) return;
                        break;

                    case "-s":
                    case "--scene":
                        sceneFile = GetOptionArgument(args, ref i, "Scene");
                        if (sceneFile == null) return;
                        break;

                    case "-b":
                    case "--basic":
                        basicRender = true;
                        break;
                    default:
                        ShowError(string.Format("Unknown option: {0}", args[i]));
                        return;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            UglyStaticLogic = new EditorLogic();
            UglyStaticMain = new MainForm(UglyStaticLogic, contentFile, sceneFile, basicRender);
            Application.Run(UglyStaticMain);
        }

        private static string GetOptionArgument(string[] args, ref int i, string name)
        {
            i++;
            if (args.Length > i) return args[i];

            ShowError(string.Format("Missing argument for {0}", name));
            return null;
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(string.Format("{0}\nFor usage hint invoke with \"--help\" option.", message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.ExitCode = 1;
        }

        private static void ShowUsage()
        {
            MessageBox.Show("Usage: Edytejszyn [OPTIONS]\n" +
                            "Possible options:\n" +
                            "  -h, --help           show this help\n" +
                            "  -c, --content FILE \topen content from given FILE\n" +
                            "  -s, --scene   FILE \topen scene from given FILE\n" +
                            "  -b, --basic   set render mode to basic", "Edytejszyn usage", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
