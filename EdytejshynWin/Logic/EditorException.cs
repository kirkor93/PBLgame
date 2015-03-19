using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edytejshyn.Logic
{
    public class EditorException : Exception
    {
        public EditorException(string message)
            : base(message)
        {
        }

        public EditorException(string message, Exception inner)
            : base(message, inner)
        {
        }
        
    }
}
