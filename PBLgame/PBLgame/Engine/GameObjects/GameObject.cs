using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PBLgame.Engine.Components;

namespace PBLgame.Engine.GameObjects
{
    public class GameObject : IXmlSerializable
    {
        #region Variables
            #region Public
            public String Name;
            public String Tag;
            public int ID;
            #endregion
            #region Private
            GameObject _parent = null;
            //Components list attached to game object
            List<Component> _components = new List<Component>();
            //Most common _components             
            private Transform _transform;
            private Renderer _renderer;
            private Collision _collision;
            private Animator _animator;
            private ParticleSystem _particleSystem;
            private AudioSource _audioSource;
            #endregion
        #endregion

        #region Properties
        public GameObject parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
        public Transform transform
        {
            get
            {
                return _transform;
            }
            set
            {
                _transform = value;
            }
        }
        public Renderer renderer
        {
            get
            {
                return _renderer;
            }
            set
            {
                _renderer = value;
            }
        }
        public Collision collision
        {
            get
            {
                return _collision;
            }
            set
            {
                _collision = value;
            }
        }
        public Animator animator
        {
            get
            {
                return _animator;
            }
            set
            {
                _animator = value;
            }
        }
        public ParticleSystem particleSystem
        {
            get
            {
                return _particleSystem;
            }
            set
            {
                _particleSystem = value;
            }
        }
        public AudioSource audioSource
        {
            get
            {
                return _audioSource;
            }
            set
            {
                _audioSource = value;
            }
        }
        #endregion  

        #region Methods

        public GameObject()
        {
            _transform = new Transform(this);
        }

        public void Update()
        {
            if (transform != null)
            {
                transform.Update();
            }
            if (renderer != null)
            {
                renderer.Update();
            }
            if (collision != null)
            {
                collision.Update();
            }
            if (animator != null)
            {
                animator.Update();
            }
            if (particleSystem != null)
            {
                particleSystem.Update();
            }
            if (audioSource != null)
            {
                audioSource.Update();
            }

            foreach (Component component in _components)
            {
                component.Update();
            }
        }

        public void Draw()
        {
            if (transform != null)
            {
                transform.Draw();
            }
            if (renderer != null)
            {
                renderer.Draw();
            }
            if (collision != null)
            {
                collision.Draw();
            }
            if (animator != null)
            {
                animator.Draw();
            }
            if (particleSystem != null)
            {
                particleSystem.Draw();
            }
            if (audioSource != null)
            {
                audioSource.Draw();
            }

            foreach (Component component in _components)
            {
                component.Draw();
            }
        }

        public void AddComponent<T>(T component) where T : Component
        {
            if (typeof(T) == typeof(Transform))
            {
                _transform = component as Transform;
                return;
            }
            if(typeof(T) == typeof (Renderer))
            {
                _renderer = component as Renderer;
                return;
            }
            if (typeof(T) == typeof(Collision))
            {
                _collision = component as Collision;
                return;
            }
            if (typeof(T) == typeof(Animator))
            {
                _animator = component as Animator;
                return;
            }
            if (typeof(T) == typeof(ParticleSystem))
            {
                _particleSystem = component as ParticleSystem;
                return;
            }
            if (typeof(T) == typeof(AudioSource))
            {
                _audioSource = component as AudioSource;
                return;
            }

            _components.Add(component);
        }

        public T GetComponent<T>() where T : Component
        {
            //I know it's ugly, but I have no idea how to implement it better
            if (typeof (T) == typeof (Transform))
            {
                return _transform as T;
            }
            if (typeof(T) == typeof(Renderer))
            {
                return _renderer as T;
            }
            if (typeof(T) == typeof(Collision))
            {
                return _collision as T;
            }
            if (typeof(T) == typeof(Animator))
            {
                return _animator as T;
            }
            if (typeof(T) == typeof(ParticleSystem))
            {
                return _particleSystem as T;
            }
            if (typeof(T) == typeof(AudioSource))
            {
                return _audioSource as T;
            }

            IEnumerable<Component> list = 
                from component in _components
                where component.GetType() == typeof(T)
                select component;

            if (list.Any())
            {
                return list.First() as T;
            }

            return null;
        }

        #region XML serialization

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Tag", Tag);
            writer.WriteAttributeString("Id", ID.ToString());
            if (parent != null)
            {
                writer.WriteAttributeString("Parent", parent.ID.ToString());
            }

            if (transform != null)
            {
                writer.WriteStartElement("Transform");
                (transform as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
            if (renderer != null)
            {
                writer.WriteStartElement("Renderer");
                (renderer as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
            if (collision != null)
            {
                writer.WriteStartElement("Collision");
                (collision as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
            if (animator != null)
            {
                writer.WriteStartElement("Animator");
                (animator as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
            if (particleSystem != null)
            {
                writer.WriteStartElement("ParticleSystem");
                (particleSystem as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
            if (audioSource != null)
            {
                writer.WriteStartElement("AudioSource");
                (audioSource as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (Component component in _components)
            {
                writer.WriteStartElement(component.GetType().ToString());
                (component as IXmlSerializable).WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        #endregion
        #endregion
    }
}
