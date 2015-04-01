using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using PBLgame.Engine.Scene;
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
        public ContentManager GameContentManager;

        #endregion

        #endregion


        #region Properties
        public string ContentFile { get; private set; }
        public string SceneFile { get; private set; }

        public string ContentSimpleName
        {
            get { return Path.GetFileName(this.ContentFile); }
        }

        public string SceneSimpleName
        {
            get { return Path.GetFileName(this.SceneFile); }
        }

        public XmlContent XmlContent { get; private set; }
        public Scene CurrentScene { get; private set; }

        #endregion
        

        #region Methods
        public EditorLogic()
        {
            _resourceManager = ResourceManager.Instance;
            _serializer = _resourceManager.Serializer;
            History = new HistoryManager(this);
        }

        public void LoadContent(string path)
        {
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Open))
                {
                    XmlContent = (XmlContent)_serializer.Deserialize(new GameXmlReader(stream, GameContentManager));
                    path = Path.GetFullPath(path);
                    this.ContentFile = path;
                    //this.History.Clear();
                    this.Logger.Log(LoggerLevel.Info, string.Format("Loaded content {0}", path));
                }
            }
            catch (Exception ex)
            {
                throw new EditorException("Error while loading content XML file", ex);
            }
        }

        /// <summary>
        /// Saves content with a new name.
        /// </summary>
        /// <param name="path">Path to destination file</param>
        /// <param name="bar"></param>
        public void SaveContent(string path)
        {
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Create))
                {
                    _serializer.Serialize(stream, XmlContent);
                    path = Path.GetFullPath(path);
                    this.ContentFile = path;
                    this.Logger.Log(LoggerLevel.Info, string.Format("Saved content {0}", path));
                }
            }
            catch (Exception ex)
            {
                throw new EditorException("Failed to save content XML file", ex);
            }
        }

        /// <summary>
        /// Saves content without changing name
        /// </summary>
        public void SaveContent()
        {
            SaveContent(this.ContentFile);
        }



        public void LoadScene(string path)
        {
            try
            {
                // TODO load scene
                //CurrentScene = new Scene();
                //this.SceneFile = path;
                //this.History.Clear();
                //this.Logger.Log(LoggerLevel.Info, string.Format("Loaded scene {0}", path));
                this.Logger.Log(LoggerLevel.Warning, string.Format("Scene loading not implemented"));
            }
            catch (Exception ex)
            {
                throw new EditorException("Error while loading scene file", ex);
            }
        }

        /// <summary>
        /// Saves scene with a new name.
        /// </summary>
        /// <param name="path">Path to destination file</param>
        public void SaveScene(string path)
        {
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Create))
                {
                    _serializer.Serialize(stream, XmlContent);
                    path = Path.GetFullPath(path);
                    this.SceneFile = path;
                    this.History.SetSavedPoint();
                    this.Logger.Log(LoggerLevel.Info, string.Format("Saved scene {0}", path));
                }
            }
            catch (Exception ex)
            {
                throw new EditorException("Failed to save scene XML", ex);
            }
        }

        /// <summary>
        /// Saves scene without changing name
        /// </summary>
        public void SaveScene()
        {
            SaveScene(this.SceneFile);
        }


        #endregion

    }
}
