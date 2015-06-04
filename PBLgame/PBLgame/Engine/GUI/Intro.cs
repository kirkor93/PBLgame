﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using  PBLgame.Engine.Singleton;

namespace PBLgame.Engine.GUI
{
    public class Intro : Singleton<Intro>, IXmlSerializable
    {
        #region Variables
        private List<IntroScene> _scenes;
        private TimeSpan _lastChangeTime;
        private List<IntroScene>.Enumerator _currentScene;
        private SpriteBatch _batch;
        private Vector2 _referenceWindowSize;
        private Vector2 _currentWindowSize;

        private const string IntroFilePath = "Intro.xml";
        #endregion
        #region Events

        public event EventHandler<EventArgs> OnIntroFinished;

        #endregion
        #region Properties

        public SpriteBatch Batch
        {
            get { return _batch; }
            set { _batch = value; }
        }

        public Vector2 ReferenceWindowSize
        {
            get { return _referenceWindowSize; }
            set { _referenceWindowSize = value; }
        }

        public Vector2 CurrentWindowSize
        {
            get { return _currentWindowSize; }
            set
            {
                _currentWindowSize = value;
                Rescale(value);
            }
        }

        #endregion
        #region Methods

        public Intro()
        {
            _scenes = new List<IntroScene>();
        }

        public void Start()
        {
            if(_scenes != null && _scenes.Count != 0)
            {
                _currentScene = _scenes.GetEnumerator();
                _currentScene.MoveNext();
            }
        }

        public void Update(GameTime gameTime)
        {
            if(_lastChangeTime + _currentScene.Current.Length < gameTime.TotalGameTime)
            {
                if (_currentScene.MoveNext())
                {
                    _lastChangeTime = gameTime.TotalGameTime;
                }
                else
                {
                    if (OnIntroFinished != null)
                    {
                        OnIntroFinished(this, EventArgs.Empty);
                    }
                    else
                    {
                        throw new Exception("OnIntroFInished has to have handlers for game to work properly");
                    }
                }
            }
        }

        public void Draw()
        {
            if (_currentScene.Current != null)
            {
                _currentScene.Current.Draw(Batch);
            }
        }

        private void Rescale(Vector2 windowSize)
        {
            float scaleFactor1 = CurrentWindowSize.X / ReferenceWindowSize.X;
            float scaleFactor2 = CurrentWindowSize.Y / ReferenceWindowSize.Y;
            if (scaleFactor1 != scaleFactor2)
            {
                scaleFactor1 = Math.Min(scaleFactor1, scaleFactor2);
            }

            if (scaleFactor1 != 1.0f)
            {
                foreach (IntroScene scene in _scenes)
                {
                    scene.Position *= scaleFactor1;
                    scene.Scale *= scaleFactor1;
                }
            }
        }

        public void Load(string path = IntroFilePath)
        {
            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Intro));
                Intro intro = (Intro)serializer.Deserialize(file);
                _referenceWindowSize = intro.ReferenceWindowSize;
                _scenes = intro._scenes;
            }
        }

        public void Save(string path = IntroFilePath)
        {
            IntroScene s = new IntroScene()
            {
                Enabled = true,
                Id = 1,
                Length = new TimeSpan(0, 0, 10),
                Name = @"Scene 1",
                Pivot = PivotPoint.UpperLeft,
                Position = Vector2.Zero,
                Scale = Vector2.One
            };
            s.Text = new GUIText();
            s.Text.Text = "Test";
            s.Text.Font = ResourceManager.Instance.GetFont(@"Fonts\Default");
            s.Texture = ResourceManager.Instance.GetTexture(@"Textures\Intro\scene1");
            _scenes.Add(s);


            using (FileStream writer = new FileStream(path, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Intro));
                serializer.Serialize(writer, this);
            }
        }

        #region Xml Serialization
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            reader.MoveToContent();
            reader.ReadStartElement();
            while (reader.Name != "Intro")
            {
                if (reader.Name == "ReferenceWindowSize")
                {
                    Vector2 tmp = Vector2.Zero;
                    tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                    tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                    ReferenceWindowSize = tmp;
                    reader.Read();
                }
                while (reader.Name != "IntroScenes")
                {
                    string readerName = reader.Name;
                    Type type = Type.GetType(readerName);
                    ConstructorInfo ctor = type.GetConstructor(new Type[] { });

                    IntroScene introScene = ctor.Invoke(new object[] { }) as IntroScene;
                    introScene.ReadXml(reader);
                    _scenes.Add(introScene);
                    if (reader.Name == readerName)
                    {
                        reader.Read();
                    }
                }
                reader.Read();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            writer.WriteStartElement("ReferenceWindowSize");
            writer.WriteAttributeString("x", ReferenceWindowSize.X.ToString(culture));
            writer.WriteAttributeString("y", ReferenceWindowSize.Y.ToString(culture));
            writer.WriteEndElement();
            writer.WriteStartElement("IntroScenes");
            foreach (IntroScene introScene in _scenes)
            {
                string name = introScene.GetType().ToString();
                writer.WriteStartElement(name);
                introScene.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        #endregion
        #endregion
    }
}