using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.Components
{
    public class AudioSource : Component
    {
        #region Variables
            #region Public
            #endregion
            #region Private
            private AudioEmitter _emitter;
            private Cue _trackCue;
            #endregion
        #endregion

        #region Properties
        public AudioEmitter Emitter
        {
            get
            {
                _emitter.Position = gameObject.transform.Position;
                return _emitter;
            }
            set
            {
                _emitter = value;
            }
        }
        public Cue TrackCue
        {
            get
            {
                return _trackCue;
            }
            set
            {
                _trackCue = value;
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
            _trackCue = cue;
        }

        public override void Update()
        {
            _emitter.Position = gameObject.transform.Position;
        }
        public override void Draw()
        {

        }

        public void Set3D(AudioListener listener)
        {
            _trackCue.Apply3D(listener, _emitter);
        }
        
        public void Play()
        {
            _trackCue.Play();
        }

        public void Stop()
        {
            _trackCue.Stop(AudioStopOptions.Immediate);
        }

        #region XML Serialization

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            base.ReadXml(reader);
            string cue = reader.GetAttribute("Cue");
            TrackCue = ResourceManager.Instance.GetAudioCue(cue);
            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("Cue", TrackCue.Name);
        }

        #endregion
        #endregion
    }
}
