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

        #endregion

        #region Public
        public readonly EditorLogger   Logger  = new EditorLogger();
        public readonly HistoryManager History;
        #endregion

        #endregion


        #region Properties
        public string FilePath { get; private set; }

        public string SimpleFileName
        {
            get { return Path.GetFileName(this.FilePath); }
        }


        #endregion
        

        #region Methods
        public EditorLogic()
        {
            // TODO init serializer
             History = new HistoryManager(this);
        }

        public void LoadFile(string path)
        {
            this.FilePath = path;
            this.Logger.Log(LoggerLevel.Warning, "Loading files not implemented");
            //throw new EditorException("Please, implement opening files");
            //this.Logger.Log(LoggerLevel.Info, string.Format("Loaded file {0}", path));
            //try
            //{
            //    // TODO deserialize
            //    using(FileStream stream = File.Open(file, FileMode.Open))
            //    {
            //        this.Filename = path;
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
            this.Logger.Log(LoggerLevel.Warning, "Saving not implemented.");
            try
            {
                // TODO serialize to xml

                if (!File.Exists(path))
                {
                    File.Create(path);
                    this.Logger.Log(LoggerLevel.Warning, "Saving not implemented. Created empty file.");
                }
                this.FilePath = path;
            }
            catch (Exception ex)
            {
                throw new EditorException("Failed to save XML file", ex);
            }
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
