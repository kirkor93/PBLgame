using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;
using PBLgame.Engine.Scenes;

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
            //Children list of this gameObject
            List<GameObject> _children = new List<GameObject>();
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

        public GameObject(GameObject source, GameObject parent)
        {
            ID = 0;
            Name = source.Name;
            Tag  = source.Tag;
            _parent = parent;

            if (source.transform != null)
            {
                transform = new Transform(source.transform, this);
            }
            if (source.renderer != null)
            {
                renderer = new Renderer(source.renderer, this);
            }
            if (source.collision != null)
            {
                collision = new Collision(source.collision, this);
            }
            if (source.animator != null)
            {
                animator = new Animator(source.animator, this);
            }
            if (source.particleSystem != null)
            {
                particleSystem = new ParticleSystem(source.particleSystem, this);
            }
            if (source.audioSource != null)
            {
                audioSource = new AudioSource(source.audioSource, this);
            }

            foreach (Component component in source._components)
            {
                AddComponent(component.Copy(this));
            }

            // recursive children duplication
            foreach (GameObject child in source._children)
            {
                _children.Add(child.Copy(this));
            }

        }

        public virtual GameObject Copy(GameObject sourceParent)
        {
            return new GameObject(this, sourceParent);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (transform != null)
            {
                transform.Update(gameTime);
            }
            if (animator != null)
            {
                animator.Update(gameTime);
            }
            if (particleSystem != null)
            {
                particleSystem.Update(gameTime);
            }
            if (audioSource != null)
            {
                audioSource.Update(gameTime);
            }

            foreach (Component component in _components)
            {
                component.Update(gameTime);
            }
            if (collision != null)
            {
                collision.Update(gameTime);
            }
            if (renderer != null)
            {
                renderer.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (transform != null)
            {
                transform.Draw(gameTime);
            }
            if (animator != null)
            {
                animator.Draw(gameTime);
            }
            if (particleSystem != null)
            {
                particleSystem.Draw(gameTime);
            }
            if (audioSource != null)
            {
                audioSource.Draw(gameTime);
            }

            foreach (Component component in _components)
            {
                component.Draw(gameTime);
            }
            if (collision != null)
            {
                collision.Draw(gameTime);
            }
            if (renderer != null)
            {
                renderer.Draw(gameTime);
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

        public void AddChild(GameObject child)
        {
            _children.Add(child);
            child.parent = this;
        }

        public GameObject[] GetChildren()
        {
            return _children.ToArray();
        }

        public GameObject GetChild(int id)
        {
           IEnumerable<GameObject> list =
                from child in _children
                where child.ID == id
                select child;

            if (list.Any())
            {
                return list.First() as GameObject;
            }

            return null;
        }

        public GameObject GetChild(string name)
        {
            IEnumerable<GameObject> list =
                from child in _children
                where child.Name == name
                select child;

            if (list.Any())
            {
                return list.First();
            }

            return null;
        }
        
        public void Reparent(GameObject newParent, int index)
        {
            if (_parent == newParent) return;
            if (_parent != null)
            {
                _parent._children.Remove(this);
            }
            if (newParent != null)
            {
                newParent._children.AddInsert(index, this);
            }
            _parent = newParent;
        }

        public void RemoveChild(GameObject child)
        {
            _children.Remove(child);
        }

        public T[] GetComponentInChildren<T>() where T : Component
        {
            List<T> components = new List<T>();
            foreach(GameObject child in _children)
            {
                if(child.GetComponent<T>() != null)
                {
                    components.Add(child.GetComponent<T>());
                }
            }

            if (components.Any())
            {
                return components.ToArray();
            }

            return null;
        }

        public T GetComponentInChild<T>(int id) where T : Component
        {
            GameObject tmp = GetChild(id);
            return tmp.GetComponent<T>();
        }

        public T GetComponentInChild<T>(string name) where T : Component
        {
            GameObject tmp = GetChild(name);
            return tmp.GetComponent<T>();
        }

        #region XML serialization

        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void WriteXml(XmlWriter writer)
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

        // ReSharper disable once FunctionComplexityOverflow
        public virtual void ReadXml(XmlReader reader)
        {
            Scene scene = ((Scene.SceneXmlReader) reader).Scene;
            reader.MoveToContent();
            Name = reader.GetAttribute("Name");
            Tag = reader.GetAttribute("Tag");
            ID = Convert.ToInt32(reader.GetAttribute("Id"));

            parent = null;
            int parentId = Convert.ToInt32(reader.GetAttribute("Parent"));
            if (parentId != 0)
            {
                parent = new GameObject {ID = parentId};
            } 

            reader.ReadStartElement();
            if (reader.Name == "Transform")
            {
                transform = new Transform(this);
                transform.ReadXml(reader);
            }

            if (reader.Name == "Renderer")
            {
                renderer = new Renderer(this, scene);
                renderer.ReadXml(reader);
            }

            if (reader.Name == "Collision")
            {
                collision = new Collision(this);
                collision.ReadXml(reader);
            }

            if (reader.Name == "Animator")
            {
                animator = new Animator(this);
                animator.ReadXml(reader);
            }

            if (reader.Name == "AudioSource")
            {
                audioSource = new AudioSource(this);
                audioSource.ReadXml(reader);
            }

            if (reader.Name == "ParticleSystem")
            {
                particleSystem = new ParticleSystem(this);
                particleSystem.ReadXml(reader);
            }

            while (reader.Name != "GameObject")
            {
                string readerName = reader.Name;
                Type type = Type.GetType(readerName);
                ConstructorInfo ctor = type.GetConstructor(new Type[] {typeof (GameObject)});

                Component component = ctor.Invoke(new object[] { this }) as Component;
                component.ReadXml(reader);
                _components.Add(component);
                if (reader.Name == readerName)
                {
                    reader.Read();
                }
            }
        }

        #endregion
        #endregion
    }
}
