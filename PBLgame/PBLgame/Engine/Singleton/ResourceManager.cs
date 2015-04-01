using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
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
        private IList<MeshMaterial> _materials;
        private IList<Effect> _shaderEffects; 
        private SoundBank _soundBank;

        private readonly XmlSerializer _serializer;

        #endregion
        #region Protected



        #endregion
        #region Public



        #endregion
        #endregion

        #region Properties
        public XmlSerializer Serializer
        {
            get { return _serializer; }
        }
        #endregion

        #region Methods
        #region Constructors

        public ResourceManager()
        {
            _meshes = new List<Mesh>();
            _textures = new List<Texture2D>();
            _shaderEffects = new List<Effect>();
            _serializer = new XmlSerializer(typeof(XmlContent), new XmlRootAttribute("XmlContent"));
            _soundBank = null;
        }

        #endregion

        /// <summary>
        /// Method used to give ResourceManager access to all avilable in game game sounds
        /// </summary>
        /// <param name="bank">Bank object</param>
        public void AssignAudioBank(SoundBank bank)
        {
            _soundBank = bank;
        }

        /// <summary>
        /// Loads all content saved in XML file 
        /// </summary>
        /// <param name="path">Path to content XML file</param>
        public void LoadContent(string path = CONTENT_LIST_PATH)
        {
            XmlContent content;

            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                content = (XmlContent) _serializer.Deserialize(new GameXmlReader(file, Game.Instance.Content));
            }


            _meshes = content.Meshes;
            _textures = content.Textures;
            _materials = content.Materials;
            _shaderEffects = content.ShaderEffects;

        }

        /// <summary>
        /// Saves all content in XML file
        /// </summary>
        /// <param name="path">Path to content XML file</param>
        public void SaveContent(string path = CONTENT_LIST_PATH)
        {
            XmlContent content = new XmlContent 
            {
                Materials = _materials, 
                Meshes = _meshes, 
                Textures = _textures, 
                ShaderEffects = _shaderEffects
            };

            using (FileStream writer = new FileStream(path, FileMode.Create))
            {
                _serializer.Serialize(writer, content);
            }
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

        public MeshMaterial GetMaterial(int id)
        {
            IEnumerable<MeshMaterial> list =
                from meshMaterial in _materials
                where meshMaterial.Id == id
                select meshMaterial;

            if (list.Any())
            {
                return list.First();
            }

            return null;
        }

        public Cue GetAudioCue(string audioName)
        {
            return _soundBank.GetCue(audioName);
        }

        #endregion

    }

    #region XML serialization

    public class GameXmlReader : XmlTextReader
    {
        public readonly ContentManager UsedContentManager;

        public GameXmlReader(Stream stream, ContentManager contentManager) : base(stream)
        {
            UsedContentManager = contentManager;
        }
    }

    public class XmlContent : IXmlSerializable
    {
        public IList<Mesh> Meshes { get; set; }
        public IList<Texture2D> Textures { get; set; }
        public IList<MeshMaterial> Materials { get; set; }
        public IList<Effect> ShaderEffects { get; set; } 

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            ContentManager content = ((GameXmlReader) reader).UsedContentManager;
            Textures = new List<Texture2D>();
            Meshes = new List<Mesh>();
            Materials = new List<MeshMaterial>();
            ShaderEffects = new List<Effect>();

            reader.MoveToContent();
            reader.ReadStartElement();

            do
            {
                if (reader.Name == "Texture")
                {
                    string path = reader.GetAttribute("Path");
                    Texture2D texture = LoadTexture(path, content);
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
                    string shaderPath = reader.GetAttribute("ShaderPath");
                    
                    Materials.Add(new MeshMaterial(id,
                                                    FindTexture(diffuseTex),
                                                    FindTexture(normalTex),
                                                    FindTexture(specularTex),
                                                    FindTexture(emissiveTex),
                                                    shaderPath));
                }
                else if (reader.Name == "Mesh")
                {
                    int id = Convert.ToInt32(reader.GetAttribute("Id"));
                    string path = reader.GetAttribute("Path");
                    Model model = LoadModel(path, content);
                    Mesh mesh = new Mesh(id, path, model);
                    Meshes.Add(mesh);
                }
                else if(reader.Name == "ShaderEffect")
                {
                    string path = reader.GetAttribute("Path");
                    Effect effect = LoadShaderEffect(path, content);
                    effect.Name = path;
                    ShaderEffects.Add(effect);
                }
                
            } while (reader.Read());
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Metadata");
            writer.WriteEndElement();

            writer.WriteStartElement("Content");

            writer.WriteStartElement("ShaderEffects");
            foreach (Effect shaderEffect in ShaderEffects)
            {
                writer.WriteStartElement("ShaderEffect");
                writer.WriteAttributeString("Path", shaderEffect.Name);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Textures");
            foreach (Texture2D texture2D in Textures)
            {
                writer.WriteStartElement("Texture");
                writer.WriteAttributeString("Path", texture2D.Name);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Materials");
            foreach (MeshMaterial meshMaterial in Materials)
            {
                writer.WriteStartElement("Material");
                writer.WriteAttributeString("Id", meshMaterial.Id.ToString());
                writer.WriteAttributeString("Diffuse", meshMaterial.Diffuse.Name);
                writer.WriteAttributeString("Normal", meshMaterial.Normal.Name);
                writer.WriteAttributeString("Specular", meshMaterial.Specular.Name);
                writer.WriteAttributeString("Emissive", meshMaterial.Emissive.Name);
                writer.WriteAttributeString("ShaderPath", meshMaterial.ShaderName);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Meshes");
            foreach (Mesh mesh in Meshes)
            {
                writer.WriteStartElement("Mesh");
                writer.WriteAttributeString("Id", mesh.Id.ToString());
                writer.WriteAttributeString("Path", mesh.Path);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private Model LoadModel(string path, ContentManager content)
        {
            return content.Load<Model>(path);
        }

        private Texture2D LoadTexture(string path, ContentManager content)
        {
            return content.Load<Texture2D>(path);
        }

        private Effect LoadShaderEffect(string path, ContentManager content)
        {
            return content.Load<Effect>(path);
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