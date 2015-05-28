using System;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLgame.Engine.Components;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.GUI
{
    public class Button : GUIObject
    {
        #region Variables
        private Texture2D _enabledTexture;
        private Texture2D _disabledTexture;
        private Texture2D _selectedTexture;
        private Texture2D _pressedTexture;
        private object _onClickActionScript;
        private string _onClickMethod;
        private MethodInfo _method;

        private Button[] _neighbours = new Button[4];
        #endregion
        #region Properties
        public Texture2D EnabledTexture
        {
            get { return _enabledTexture; }
            set { _enabledTexture = value; }
        }

        public Texture2D DisabledTexture
        {
            get { return _disabledTexture; }
            set { _disabledTexture = value; }
        }

        public Texture2D SelectedTexture
        {
            get { return _selectedTexture; }
            set { _selectedTexture = value; }
        }

        public Texture2D PressedTexture
        {
            get { return _pressedTexture; }
            set { _pressedTexture = value; }
        }

        public object OnClickActionScript
        {
            get { return _onClickActionScript; }
            set
            {
                _onClickActionScript = value;
                _onClickMethod = String.Empty;
                _method = null;
            }
        }

        public string OnClickMethod
        {
            get { return _onClickMethod; }
            set
            {
                _onClickMethod = value;
                if (OnClickActionScript != null)
                {
                    FindOnClickMethod();
                }
            }
        }

        public Button RightNeighbour
        {
            get { return _neighbours[0]; }
            set { _neighbours[0] = value; }
        }

        public Button LeftNeighbour
        {
            get { return _neighbours[1]; }
            set { _neighbours[1] = value; }
        }

        public Button UpNeighbour
        {
            get { return _neighbours[2]; }
            set { _neighbours[2] = value; }
        }

        public Button DownNeighbour
        {
            get { return _neighbours[3]; }
            set { _neighbours[3] = value; }
        }

        #endregion

        #region Methods

        public Button() : base()
        {
            OnClickMethod = String.Empty;
        }

        public Button(string name, Vector2 position, Vector2 scale, PivotPoint pivotPoint, Texture2D enabledTexture, Texture2D disabledTexture, Texture2D selectedTexture, Texture2D pressedTexture, object onClickActionScript, string onClickMethod)
            : base(name, enabledTexture, position, scale, pivotPoint)
        {
            EnabledTexture = enabledTexture;
            DisabledTexture = disabledTexture;
            SelectedTexture = selectedTexture;
            PressedTexture = pressedTexture;
            _onClickActionScript = onClickActionScript;
            _onClickMethod = onClickMethod;

            FindOnClickMethod();
        }

        private void FindOnClickMethod()
        {
            Type type = OnClickActionScript.GetType();
            MethodInfo[] info = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            _method = Array.Find(info, i => (i.Name == OnClickMethod && i.IsPublic));
        }

        #region OnClick event handlers
        public void OnClick()
        {
            if (_method != null)
            {
                _method.Invoke(OnClickActionScript, new object[0]);
            }
        }


        #endregion

        #region Xml Serialization

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            EnabledTexture = ResourceManager.Instance.GetTexture(reader.GetAttribute("EnabledTexture"));
            DisabledTexture = ResourceManager.Instance.GetTexture(reader.GetAttribute("DisabledTexture"));
            PressedTexture = ResourceManager.Instance.GetTexture(reader.GetAttribute("PressedTexture"));
            SelectedTexture = ResourceManager.Instance.GetTexture(reader.GetAttribute("SelectedTexture"));

            string scriptName = reader.GetAttribute("OnClickActionScript");
            if (scriptName != null)
            {
                Type t = Type.GetType(scriptName);
                ConstructorInfo ctor = t.GetConstructor(new Type[0]);

                OnClickActionScript = ctor.Invoke(new object[0]);
            }
            OnClickMethod = reader.GetAttribute("OnClickMethod");

            for (int i = 0; i < 4; i++)
            {
                int id = Convert.ToInt32(reader.GetAttribute("Neighbour_" + i.ToString("G", CultureInfo.InvariantCulture)));
                if (id != -1)
                {
                    _neighbours[i] = new Button()
                    {
                        Id = id
                    };
                }
            }
            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteStartElement("Button");
            if (EnabledTexture != null)
            {
                writer.WriteAttributeString("EnabledTexture", EnabledTexture.Name);
            }
            if (DisabledTexture != null)
            {
                writer.WriteAttributeString("DisabledTexture", DisabledTexture.Name);
            }
            if (SelectedTexture != null)
            {
                writer.WriteAttributeString("SelectedTexture", SelectedTexture.Name);
            }
            if (PressedTexture != null)
            {
                writer.WriteAttributeString("PressedTexture", PressedTexture.Name);
            }
            if (OnClickActionScript != null)
            {
                writer.WriteAttributeString("OnClickActionScript", OnClickActionScript.GetType().ToString());
                writer.WriteAttributeString("OnClickMethod", OnClickMethod);
            }
            for (int i = 0; i < 4; i ++)
            {
                int id = -1;
                if (_neighbours[i] != null)
                {
                    id = _neighbours[i].Id;
                }
                writer.WriteAttributeString("Neighbour_" + i.ToString("G", CultureInfo.InvariantCulture), id.ToString("G", CultureInfo.InvariantCulture));
            }
            writer.WriteEndElement();
        }

        #endregion

        #endregion
    }
}