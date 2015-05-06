using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AnimationData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.Components
{
    public class Animator : Component
    {
        private List<AnimationClip> _animationClips;
        private AnimationPlayer _animationPlayer;

        private bool _isAnimating = true;

        #region Methods
        public Animator(GameObject owner) : base(owner)
        {
            _animationClips = new List<AnimationClip>();
            _animationPlayer = new AnimationPlayer(gameObject.renderer.MyMesh.Model.Tag as SkinningData);
            
            
            _animationClips.Add(ResourceManager.Instance.GetAnimationClip(1));
            StartAnimation(_animationClips.First());
        }

        public override void Update(GameTime gameTime)
        {
            if (_isAnimating)
            {
                _animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
                gameObject.renderer.MyMesh.BonesTransorms = _animationPlayer.GetSkinTransforms();
            }
        }

        public override void Draw(GameTime gameTime)
        {
//            foreach (ModelMesh mesh in gameObject.renderer.MyMesh.Model.Meshes)
//            {
//                foreach (SkinnedEffect skinnedEffect in mesh.Effects)
//                {
//                    skinnedEffect.SetBoneTransforms(_animationPlayer.GetSkinTransforms());
//                }
//            }
        }

        public void StartAnimation(string name)
        {
            AnimationClip animation = _animationClips.Find(anim => anim.Path == name);
            StartAnimation(animation);
        }

        public void StartAnimation(int id)
        {
            AnimationClip animation = _animationClips.Find(anim => anim.Id == id);
            StartAnimation(animation);
        }

        private void StartAnimation(AnimationClip clip)
        {
            _animationPlayer.StartClip(clip);
        }

        public Matrix[] GetSkinTransforms()
        {
            return _animationPlayer.GetSkinTransforms();
        }

        #region XML Serialization

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
        }

        #endregion
        #endregion
    }
}
