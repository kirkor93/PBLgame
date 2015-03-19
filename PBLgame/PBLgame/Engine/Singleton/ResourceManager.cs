using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Components;

namespace PBLgame.Engine.Singleton
{

    public class ResourceManager : Singleton<ResourceManager>
    {
        #region Variables
        #region Private

        private const string CONTENT_LIST_PATH = @"..\..\..\content.xml";

        private IList<Mesh> _meshes;
        private IList<Texture2D> _textures;

        #endregion
        #region Protected



        #endregion
        #region Public



        #endregion
        #endregion

        #region Properties

        #endregion

        #region Methods
        #region Constructors

        public ResourceManager()
        {
            _meshes = new List<Mesh>();
            _textures = new List<Texture2D>();
            
        }

        #endregion

        public void LoadContent()
        {
            XmlContent content;

            XmlSerializer serializer = new XmlSerializer(typeof(XmlContent), new XmlRootAttribute("Content"));
            using (FileStream file = new FileStream(CONTENT_LIST_PATH, FileMode.Open))
            {
                content = (XmlContent) serializer.Deserialize(file);
            }


            this._meshes = content.Meshes;
            this._textures = content.Textures;

        }

        public Mesh GetMesh(string path)
        {
            IEnumerable<Mesh> list =
                from mesh in _meshes
                where mesh.Path == path
                select mesh;

            if (list.Any())
            {
                return list.First();
            }
            return null;
        }

        public Mesh GetMesh(int id)
        {
            IEnumerable<Mesh> list =
                from mesh in _meshes
                where mesh.Id == id
                select mesh;

            if (list.Any())
            {
                return list.First();
            }
            return null;
        }

        public Texture2D GetTexture(string path)
        {
//start here
            return null;
        }


        #endregion

    }

    #region XML serialization

    public class XmlContent : IXmlSerializable
    {
        public IList<Mesh> Meshes { get; set; }
        public IList<Texture2D> Textures { get; set; } 

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Textures = new List<Texture2D>();
            Meshes = new List<Mesh>();

            reader.MoveToContent();
            reader.ReadStartElement();

            //reading the XML loop
            do
            {
                if (reader.Name == "Mesh")
                {
                    int id = Convert.ToInt32(reader.GetAttribute("Id"));
                    string path = reader.GetAttribute("Path");
                    Model model = LoadModel(path);
                    Meshes.Add(new Mesh(id, path, model));
                }
                else if (reader.Name == "Texture")
                {
                    int id = Convert.ToInt32(reader.GetAttribute("Id"));
                    string path = reader.GetAttribute("Path");
                    Texture2D texture = LoadTexture(path);
                    Textures.Add(texture);
                }

            } while (reader.Read());
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        private Model LoadModel(string path)
        {
            return Game.Instance.Content.Load<Model>(path);
        }

        private Texture2D LoadTexture(string path)
        {
            return Game.Instance.Content.Load<Texture2D>(path);
        }
    }


    #endregion
}