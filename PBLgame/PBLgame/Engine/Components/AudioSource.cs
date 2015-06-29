using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.Components
{
    /// <summary>
    /// XACT allows Cue to be played once, so this class is so fucked up
    /// </summary>
    public class AudioSource : Component
    {
        #region Variables
            #region Public
            #endregion
            #region Private
            private AudioEmitter _emitter;
            private List<Cue> _trackCues = new List<Cue>();
            private const int MAX_CUES = 5;
            #endregion
        #endregion

        #region Properties
        public AudioEmitter Emitter
        {
            get
            {
                //_emitter.Position = gameObject.transform.Position;
                return _emitter;
            }
            set
            {
                _emitter = value;
            }
        }
        public List<Cue> TrackCues
        {
            get
            {
                return _trackCues;
            }
        }
        #endregion

        #region Mehtods
        public AudioSource(GameObject owner) : base(owner)
        {
            _emitter = new AudioEmitter();
        }
        public AudioSource(Cue cue, GameObject owner) : base(owner)
        {
            _emitter = new AudioEmitter();
            _trackCues.Add(cue);
        }

        public AudioSource(AudioSource src, GameObject owner) : base(owner)
        {
            _emitter = new AudioEmitter();
            foreach (Cue cue in src._trackCues)
            {
                _trackCues.Add(ResourceManager.Instance.GetAudioCue(cue.Name));
            }
        }

        public override void Update(GameTime gameTime)
        {
            _emitter.Position = gameObject.transform.World.Translation;
            AudioListener listener = Camera.MainCamera.audioListener;
            foreach (Cue cue in _trackCues)
            {
                Set3D(cue, listener);
            }
        }

        public override void Draw(GameTime gameTime)
        {

        }

        public override Component Copy(GameObject newOwner)
        {
            return new AudioSource(this, newOwner);
        }


        private void Set3D(Cue cue)
        {
            Set3D(cue, Camera.MainCamera.audioListener);
        }

        public void Set3D(Cue cue, AudioListener listener)
        {
            cue.Apply3D(listener, _emitter);
        }


        public void Play(int id)
        {
            Set3D(_trackCues[id]);
            _trackCues[id].Play();
        }

        public void Play(string name)
        {
            if (name == null)  return;
            Cue cue = ResourceManager.Instance.GetAudioCue(name);
            EnqueueCue(cue);
            Set3D(cue);
            cue.Play();
        }

        private void EnqueueCue(Cue cue)
        {
            while (_trackCues.Count >= MAX_CUES) _trackCues.RemoveAt(0);
            _trackCues.Add(cue);
        }

        //public void Play(Cue cue)
        //{
        //    bool contains = _trackCues.Contains(cue);
        //    if(!contains) _trackCues.Add(cue);
        //    Set3D(cue);
        //    cue.Play();
        //}

        public void StopAll()
        {
            foreach (Cue cue in _trackCues)
            {
                cue.Stop(AudioStopOptions.AsAuthored);
            }
        }

        #region XML Serialization

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            string cue = reader.GetAttribute("Cue");
            TrackCues.Add(ResourceManager.Instance.GetAudioCue(cue));
            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("Cue", TrackCues[0].Name);
        }

        #endregion
        #endregion
    }
}
