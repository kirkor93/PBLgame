using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Edytejshyn.GUI;
using Edytejshyn.Logic;

namespace Edytejshyn
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm mainForm = new MainForm(new EditorLogic());
            mainForm.Show();

            EditorXna xna = new EditorXna(mainForm);
            xna.Run();

            //Application.Run();
        }
    }
}
