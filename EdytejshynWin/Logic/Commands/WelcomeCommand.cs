using System.Windows.Forms;

namespace Edytejshyn.Logic.Commands
{
    public class WelcomeCommand : ICommand
    {
        private MainForm _form;

        public WelcomeCommand(MainForm form)
        {
            _form = form;
        }

        public string Description
        {
            get { return "Welcome"; }
        }

        public string Message
        {
            get { return "User hello and welcome."; }
        }

        public void Do()
        {
            MessageBox.Show("Greetings, good master. Welcome to the Tavern of the Rising Sun!", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void Undo()
        {
            MessageBox.Show("Get out! Please feel unwelcomed!", "Unwelcome", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}