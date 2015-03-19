using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edytejshyn.Logic
{
    public class EditorLogic
    {
        #region Variables
        #region Private
        private string _filename = null;
        #endregion
        #endregion


        #region Properties
        public string FilePath
        {
            get
            {
                return _filename;
            }
            private set
            {
                _filename = value;
            }
        }
        #endregion
        

        #region Methods
        public EditorLogic()
        {
            // TODO init serializer
        }

        public void LoadFile(string path)
        {
            throw new EditorException("Please, implement me");
            //try
            //{
            //    // TODO deserialize
            //    using(FileStream stream = File.Open(file, FileMode.Open))
            //    {
            //        this.Filename = file;
            //    }
            //}
            //catch 
            //{
            //    // TODO handle exception
            //    throw new EditorException("Error while loading XML file", ex);
            //}
        }

        /// <summary>
        /// Saves file with new name.
        /// </summary>
        /// <param name="path">Path to destination file</param>
        public void SaveFile(string path)
        {
            throw new EditorException("Not yet implemented");
        }

        /// <summary>
        /// Saves file without changing name
        /// </summary>
        public void SaveFile()
        {
            SaveFile(this.FilePath);
        }

        #endregion
    }
}
