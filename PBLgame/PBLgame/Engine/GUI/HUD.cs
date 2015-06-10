using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Singleton;
using PBLgame.GamePlay;

namespace PBLgame.Engine.GUI
{
    public class HUD : Singleton<HUD>, IXmlSerializable
    {
        #region Variables

        public const string GuiSavePath = @"GuiLayout.xml";

        private SpriteBatch _spriteBatch;
        private List<GUIObject> _guiObjects;
        private Vector2 _referenceWindowSize;
        private Vector2 _currentWindowSize;
        private TalentWindowManager _talentWindowManager;
        private PlayerStatistics _playerStats;
        private GUIObject _levelUpText;
        private TimeSpan _levelUpStartTime;
        private TimeSpan _currentTime;
        private GUIObject _lowHpIndicator;

        #endregion

        #region Properties

        public SpriteBatch Batch
        {
            get { return _spriteBatch; }
            set { _spriteBatch = value; }
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

        public TalentWindowManager TalentWindowManager
        {
            get { return _talentWindowManager; }
            private set { _talentWindowManager = value; }
        }

        #endregion

        #region Methods
        public HUD()
        {
            ReferenceWindowSize = Vector2.Zero;
            _guiObjects = new List<GUIObject>();
        }

        public void AssignPlayerStatisticsScript(PlayerStatistics stats)
        {
            _playerStats = stats;
            (_playerStats.Experience as ExperienceStat).OnLevelUp += OnLevelUp;
            if (_talentWindowManager != null)
            {
                _talentWindowManager.AssignPlayerStatisticsScript(stats);
            }
        }

        private void OnLevelUp(object sender, EventArgs eventArgs)
        {
            _levelUpStartTime = _currentTime;
            _levelUpText.Enabled = true;
        }

        public void AddGuiObject(GUIObject obj)
        {
            while (_guiObjects.Exists(guiObject => guiObject.Id == obj.Id))
            {
                obj.Id ++;
            }
            _guiObjects.Add(obj);
        }

        public GUIObject GetGuiObject(int id)
        {
            return _guiObjects.Find(obj => obj.Id == id);
        }

        public GUIObject GetGuiObject(string name)
        {
            return _guiObjects.Find(obj => obj.Name == name);
        }

        public List<T> GetGuiObjects<T>() where T : GUIObject
        {
            IEnumerable<GUIObject> list =
            from guiObject in _guiObjects
            where guiObject.GetType() == typeof (T)
            select guiObject;

            return list.Cast<T>().ToList();
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
                foreach (GUIObject guiObject in _guiObjects)
                {
                    guiObject.Position *= scaleFactor1;
                    guiObject.Scale *= scaleFactor1;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            _currentTime = gameTime.TotalGameTime;
            if (_levelUpStartTime + new TimeSpan(0, 0, 3) < gameTime.TotalGameTime)
            {
                _levelUpText.Enabled = false;
            }
            if (_lowHpIndicator == null) return;
            _lowHpIndicator.Enabled = _playerStats.Health.Value/_playerStats.Health.MaxValue < 0.2f;
        }

        public void Draw()
        {
            foreach (GUIObject guiObject in _guiObjects)
            {
                guiObject.Draw(Batch);
            }
        }

        public void Load(string path = GuiSavePath)
        {
            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HUD));
                HUD hud = (HUD) serializer.Deserialize(file);
                _referenceWindowSize = hud.ReferenceWindowSize;
                _guiObjects = hud._guiObjects;
            }

            _levelUpText = GetGuiObject(@"Level_up_text");
            _lowHpIndicator = GetGuiObject(@"Low_hp_indicator");

            //finding buttons' neighbours
            List<Button> buttons = GetGuiObjects<Button>();

            _talentWindowManager = new TalentWindowManager()
            {
                GuiButtons = buttons,
                Background = GetGuiObject("Upgrade_screen_BG"),
                Enabled = false
            };
            foreach (Button button in buttons)
            {
                if (button.DownNeighbour != null)
                {
                    button.DownNeighbour = GetGuiObject(button.DownNeighbour.Id) as Button;
                }
                if (button.UpNeighbour != null)
                {
                    button.UpNeighbour = GetGuiObject(button.UpNeighbour.Id) as Button;
                }
                if (button.LeftNeighbour != null)
                {
                    button.LeftNeighbour = GetGuiObject(button.LeftNeighbour.Id) as Button;
                }
                if (button.RightNeighbour != null)
                {
                    button.RightNeighbour = GetGuiObject(button.RightNeighbour.Id) as Button;
                }
                if (button.NextSkillProgressImage != null)
                {
                    button.NextSkillProgressImage = GetGuiObject(button.NextSkillProgressImage.Id) as ProgressImage;
                }
                if (button.ThisSkillProgressImage != null)
                {
                    button.ThisSkillProgressImage = GetGuiObject((button.ThisSkillProgressImage.Id)) as ProgressImage;
                }
//                ProgressImage tmp = new ProgressImage
//                {
//                    Enabled = true,
//                    MaxProgress = 5,
//                    Name = "ThisSkillProgressBar",
//                    Pivot = PivotPoint.Center,
//                    Position = button.Position,
//                    ProgressTextures = new Texture2D[]
//                    {
//                        ResourceManager.Instance.GetTexture(@"Textures\GUI\Empty"),
//                        ResourceManager.Instance.GetTexture(@"Textures\GUI\Button_upgrade_marker_1"),
//                        ResourceManager.Instance.GetTexture(@"Textures\GUI\Button_upgrade_marker_2"),
//                        ResourceManager.Instance.GetTexture(@"Textures\GUI\Button_upgrade_marker_3"),
//                        ResourceManager.Instance.GetTexture(@"Textures\GUI\Button_upgrade_marker_4"),
//                        ResourceManager.Instance.GetTexture(@"Textures\GUI\Button_upgrade_marker_5")
//                    },
//                };
//                tmp.SetProgress(0);
//                button.ThisSkillProgressImage = tmp;
//                AddGuiObject(tmp);
            }

            if (ReferenceWindowSize == Vector2.Zero)
            {
                throw new Exception("Reference window size for GUI can't be zero");
            }

//            Save();
        }

        public void Save(string path = GuiSavePath)
        {
            using (FileStream writer = new FileStream(path, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HUD));
                serializer.Serialize(writer, this);
            }
        }

        #region Xml Serialization
        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            reader.MoveToContent();
            reader.ReadStartElement();
            while (reader.Name != "HUD")
            {
                if (reader.Name == "ReferenceWindowSize")
                {
                    Vector2 tmp = Vector2.Zero;
                    tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                    tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                    ReferenceWindowSize = tmp;
                    reader.Read();
                }
                while (reader.Name != "GUIObjects")
                {
                    string readerName = reader.Name;
                    Type type = Type.GetType(readerName);
                    ConstructorInfo ctor = type.GetConstructor(new Type[] { });

                    GUIObject guiObject = ctor.Invoke(new object[] { }) as GUIObject;
                    guiObject.ReadXml(reader);
                    AddGuiObject(guiObject);
                    if (reader.Name == readerName)
                    {
                        reader.Read();
                    }
                }
                reader.Read();
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            writer.WriteStartElement("ReferenceWindowSize");
            writer.WriteAttributeString("x", ReferenceWindowSize.X.ToString(culture));
            writer.WriteAttributeString("y", ReferenceWindowSize.Y.ToString(culture));
            writer.WriteEndElement();
            writer.WriteStartElement("GUIObjects");
            foreach (GUIObject guiObject in _guiObjects)
            {
                string name = guiObject.GetType().ToString();
                writer.WriteStartElement(name);
                guiObject.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        #endregion
        #endregion
    }
}