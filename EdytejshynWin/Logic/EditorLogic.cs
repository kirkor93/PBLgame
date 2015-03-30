using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using PBLgame.Engine.Singleton;

namespace Edytejshyn.Logic
{
    public class EditorLogic
    {
        #region Variables
        #region Private

        private ResourceManager _resourceManager;
        private readonly XmlSerializer _serializer;

        #endregion

        #region Public
        public readonly EditorLogger   Logger  = new EditorLogger();
        public readonly HistoryManager History;
        public ContentManager GameContent;

        #endregion

        #endregion


        #region Properties
        public string FilePath { get; private set; }

        public string SimpleFileName
        {
            get { return Path.GetFileName(this.FilePath); }
        }

        public XmlContent Content { get; private set; }

        #endregion
        

        #region Methods
        public EditorLogic()
        {
            _resourceManager = ResourceManager.Instance;
            _serializer = _resourceManager.Serializer;
            History = new HistoryManager(this);
        }

        public void LoadFile(string path)
        {
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Open))
                {
                    Content = (XmlContent)_serializer.Deserialize(new GameXmlReader(stream, GameContent));
                    this.FilePath = path;
                    this.History.Clear();
                    this.Logger.Log(LoggerLevel.Info, string.Format("Loaded file {0}", path));
                }
            }
            catch (Exception ex)
            {
                throw new EditorException("Error while loading XML file", ex);
            }
        }

        /// <summary>
        /// Saves file with new name.
        /// </summary>
        /// <param name="path">Path to destination file</param>
        public void SaveFile(string path)
        {
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Create))
                {
                    _serializer.Serialize(stream, Content);
                    this.FilePath = path;
                    this.History.SetSavedPoint();
                    this.Logger.Log(LoggerLevel.Info, string.Format("Saved file {0}", path));
                }
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
