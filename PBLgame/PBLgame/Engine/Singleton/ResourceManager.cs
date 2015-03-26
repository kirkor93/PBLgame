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


            _meshes = content.Meshes;
            _textures = content.Textures;

            
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
            IEnumerable<Texture2D> list =
            from texture in _textures
            where texture.Name == path
            select texture;

            if (list.Any())
            {
                return list.First();
            }
            return null;
        }


        #endregion

    }

    #region XML serialization

    public class XmlContent : IXmlSerializable
    {
        public IList<Mesh> Meshes { get; set; }
        public IList<Texture2D> Textures { get; set; }
        public IList<MeshMaterial> Materials { get; set; } 

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Textures = new List<Texture2D>();
            Meshes = new List<Mesh>();
            Materials = new List<MeshMaterial>();

            reader.MoveToContent();
            reader.ReadStartElement();

            do
            {
                if (reader.Name == "Texture")
                {
                    string path = reader.GetAttribute("Path");
                    Texture2D texture = LoadTexture(path);
                    texture.Name = path;
                    Textures.Add(texture);
                }
                    else if (reader.Name == "Material")
                {
                    int id = Convert.ToInt32(reader.GetAttribute("Id"));
                    string diffuseTex = reader.GetAttribute("Diffuse");
                    string normalTex = reader.GetAttribute("Normal");
                    string specularTex = reader.GetAttribute("Specular");
                    string emissiveTex = reader.GetAttribute("Emissive");
                    int shaderId = Convert.ToInt32(reader.GetAttribute("ShaderId"));
                    
                    Materials.Add(new MeshMaterial(id,
                                                    FindTexture(diffuseTex),
                                                    FindTexture(normalTex),
                                                    FindTexture(specularTex),
                                                    FindTexture(emissiveTex),
                                                    shaderId));
                }
                else if (reader.Name == "Mesh")
                {
                    int id = Convert.ToInt32(reader.GetAttribute("Id"));
                    string path = reader.GetAttribute("Path");
                    int materialId = Convert.ToInt32(reader.GetAttribute("MaterialId"));
                    Model model = LoadModel(path);
                    Mesh mesh = new Mesh(id, path, model, materialId);
                    mesh.AssignMaterial(FindMaterial(materialId));
                    Meshes.Add(mesh);
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

        private MeshMaterial FindMaterial(int id)
        {
            foreach (MeshMaterial meshMaterial in Materials)
            {
                if (meshMaterial.Id == id)
                {
                    return meshMaterial;
                }
            }
            return null;
        }

        private Texture2D FindTexture(string path)
        {
            foreach (Texture2D texture2D in Textures)
            {
                if (texture2D.Name == path)
                {
                    return texture2D;
                }
            }
            return null;
        }
    }


    #endregion
}