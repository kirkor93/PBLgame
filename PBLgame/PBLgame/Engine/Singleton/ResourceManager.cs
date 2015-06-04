using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using AnimationAux;
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

        private const string CONTENT_LIST_PATH = @"content.xml";

        private IList<Mesh> _meshes;
        private IList<Texture2D> _textures;
        private IList<MeshMaterial> _materials;
        private IList<Effect> _shaderEffects;
        private IDictionary<int, Skeleton> _skeletons;
        private IDictionary<string, SpriteFont> _fonts; 
        private SoundBank _soundBank;

        private readonly XmlSerializer _serializer;

        #endregion
        #endregion

        #region Properties
        public XmlSerializer Serializer
        {
            get { return _serializer; }
        }

        public IList<Mesh> Meshes
        {
            get { return _meshes; }
        }

        public IList<Texture2D> Textures
        {
            get { return _textures; }
        }

        public IList<MeshMaterial> Materials
        {
            get { return _materials; }
        }

        public IList<Effect> ShaderEffects
        {
            get { return _shaderEffects; }
        }

        public SoundBank SoundBank
        {
            get { return _soundBank; }
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
        /// <param name="manager">ContentManager which load data into. Game default Content by default</param>
        public void LoadContent(string path = CONTENT_LIST_PATH, ContentManager manager = null)
        {
            if (manager == null) manager = Game.Instance.Content;
            XmlContent content;

            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                content = (XmlContent) _serializer.Deserialize(new GameXmlReader(file, manager));
            }

            _meshes = content.Meshes;
            _textures = content.Textures;
            _materials = content.Materials;
            _shaderEffects = content.ShaderEffects;
            _skeletons = content.Skeletons;
            _fonts = content.Fonts;
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
                ShaderEffects = _shaderEffects,
                Skeletons = _skeletons
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
                where (mesh.Path == path || mesh.Path == @"Models\" + path)
                select mesh;

            return list.FirstOrDefault();
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

        public SpriteFont GetFont(string name)
        {
            return _fonts[name];
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
        public IDictionary<int, Skeleton> Skeletons { get; set; }
        public IDictionary<string, SpriteFont> Fonts { get; set; } 

        private Dictionary<AnimatedMesh, int> _meshSkeletons = new Dictionary<AnimatedMesh, int>();

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
            Skeletons = new Dictionary<int, Skeleton>();
            Fonts = new Dictionary<string, SpriteFont>();

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
                    string name        = reader.GetAttribute("Name");
                    string diffuseTex  = reader.GetAttribute("Diffuse");
                    string normalTex   = reader.GetAttribute("Normal");
                    string specularTex = reader.GetAttribute("Specular");
                    string emissiveTex = reader.GetAttribute("Emissive");
                    string shaderPath  = reader.GetAttribute("ShaderPath");
                    
                    Materials.Add(
                        new MeshMaterial(id, name,
                                FindTexture(diffuseTex),
                                FindTexture(normalTex),
                                FindTexture(specularTex),
                                FindTexture(emissiveTex),
                                FindShaderEffect(shaderPath)
                        )
                    );
                }
                else if (reader.Name == "Mesh")
                {
                    Mesh mesh;
                    int id = Convert.ToInt32(reader.GetAttribute("Id"));
                    string path = reader.GetAttribute("Path");
                    Model model = LoadModel(path, content);

                    string skeletonIDstr = reader.GetAttribute("Skeleton");
                    if (skeletonIDstr != null)
                    {
                        int skeletonID = Convert.ToInt32(skeletonIDstr);
                        AnimatedMesh animMesh = new AnimatedMesh(id, path, model);
                        _meshSkeletons[animMesh] = skeletonID;
                        mesh = animMesh;
                    }
                    else
                    {
                        mesh = new Mesh(id, path, model);
                    }
                    Meshes.Add(mesh);
                }
                else if(reader.Name == "ShaderEffect")
                {
                    string path = reader.GetAttribute("Path");
                    Effect effect = LoadShaderEffect(path, content);
                    effect.Name = path;
                    ShaderEffects.Add(effect);
                }
                else if (reader.Name == "Animation")
                {
                    int id = Convert.ToInt32(reader.GetAttribute("Id"));
                    string path = reader.GetAttribute("Path");
                    int skeletonID = Convert.ToInt32(reader.GetAttribute("Skeleton"));
                    string type = reader.GetAttribute("Type");
                    float speed = Convert.ToSingle(reader.GetAttribute("Speed") ?? "1.0", CultureInfo.InvariantCulture);
                    Model model = LoadModel(path, content);
                    ModelExtra extra = model.Tag as ModelExtra;
                    AnimationClip animation = extra.Clips[0];
                    animation.Id = id;
                    animation.Path = path;
                    animation.Type = type;
                    animation.Speed = speed;
                    AddSkeletonAnimation(skeletonID, animation);
                }
                else if (reader.Name == "SpriteFont")
                {
                    string path = reader.GetAttribute("Path");
                    SpriteFont font = content.Load<SpriteFont>(path);
                    Fonts.Add(path, font);
                }
            } while (reader.Read());

            // join animated meshes with skeletons
            foreach (KeyValuePair<AnimatedMesh, int> pair in _meshSkeletons)
            {
                pair.Key.Skeleton = Skeletons[pair.Value];
            }
        }

        private void AddSkeletonAnimation(int id, AnimationClip animation)
        {
            Skeleton skeleton;
            bool exists = Skeletons.TryGetValue(id, out skeleton);
            if(!exists)
            {
                skeleton = new Skeleton(id);
                Skeletons.Add(id, skeleton);
            }
            skeleton.AddClip(animation);
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
                writer.WriteAttributeString("Name", meshMaterial.Name);
                writer.WriteAttributeString("Diffuse", meshMaterial.Diffuse.Name);
                WriteTextureAttribute(writer, "Normal",   meshMaterial.Normal);
                WriteTextureAttribute(writer, "Specular", meshMaterial.Specular);
                WriteTextureAttribute(writer, "Emissive", meshMaterial.Emissive);
                writer.WriteAttributeString("ShaderPath", meshMaterial.ShaderEffect.Name);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Meshes");
            foreach (Mesh mesh in Meshes)
            {
                writer.WriteStartElement("Mesh");
                writer.WriteAttributeString("Id", mesh.Id.ToString());
                writer.WriteAttributeString("Path", mesh.Path);
                AnimatedMesh animMesh = mesh as AnimatedMesh;
                if(animMesh != null)
                {
                    writer.WriteAttributeString("Skeleton", animMesh.Skeleton.Id.ToString());
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Animations");
            foreach (Skeleton skeleton in Skeletons.Values)
            {
                foreach (AnimationClip animation in skeleton.Clips)
                {
                    writer.WriteStartElement("Animation");
                    writer.WriteAttributeString("Id", animation.Id.ToString());
                    writer.WriteAttributeString("Path", animation.Path);
                    writer.WriteAttributeString("Skeleton", skeleton.Id.ToString());
                    if (animation.Type != null) writer.WriteAttributeString("Type", animation.Type);
                    if (animation.Speed != 1.0f) writer.WriteAttributeString("Speed", animation.Speed.ToString("G", CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteStartElement("SpriteFonts");
            foreach (KeyValuePair<string, SpriteFont> font in Fonts)
            {
                writer.WriteStartElement("SpriteFont");
                writer.WriteAttributeString("Path", font.Key);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void WriteTextureAttribute(XmlWriter writer, string name, Texture2D value)
        {
            if(value != null) writer.WriteAttributeString(name, value.Name);
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

        private Effect FindShaderEffect(string path)
        {
            foreach (Effect shaderEffect in ShaderEffects)
            {
                if (shaderEffect.Name == path)
                {
                    return shaderEffect;
                }
            }
            return null;
        }
    }


    #endregion
}