﻿using System.Collections.Generic;
using AnimationAux;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    /// <summary>
    /// An encloser for an XNA model that we will use that includes support for
    /// bones, animation, and some manipulations.
    /// </summary>
    public class AnimatedMesh : Mesh
    {
        #region Fields

        /// <summary>
        /// Extra data associated with the XNA model
        /// </summary>
        private ModelExtra _modelExtra = null;

        /// <summary>
        /// The model bones
        /// </summary>
        private List<Bone> _bones = new List<Bone>();

        private Matrix[] _skeleton;

        #endregion

        #region Properties

        /// <summary>
        /// The underlying bones for the model
        /// </summary>
        public List<Bone> Bones { get { return _bones; } }

        /// <summary>
        /// The model animation clips
        /// </summary>
        public List<AnimationClip> Clips { get { return _modelExtra.Clips; } }

        public override Matrix[] BonesTransorms
        {
            get
            {
                return _boneTransforms;
            }
        }

        public Matrix[] Skeleton
        {
            get
            {
                return _skeleton;
            }
        }

        #endregion

        public void UpdateBonesMatrices()
        {   
            // TODO check whether can be optimized
            _skeleton = new Matrix[_modelExtra.Skeleton.Count];
            for (int s = 0; s < _modelExtra.Skeleton.Count; s++)
            {
                Bone bone = _bones[_modelExtra.Skeleton[s]];
                _skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
            }

            _boneTransforms = new Matrix[_bones.Count];

            for (int i = 0; i < _bones.Count; i++)
            {
                Bone bone = _bones[i];
                bone.ComputeAbsoluteTransform();

                _boneTransforms[i] = bone.AbsoluteTransform;
            }
        }

        #region Construction and Loading

        /// <summary>
        /// Constructor. Creates the model from an XNA model
        /// </summary>
        public AnimatedMesh(int id, string path, Model model) : base(id, path, model)
        {
            _modelExtra = model.Tag as ModelExtra;
            ObtainBones();
        }

        #endregion

        #region Bones Management

        /// <summary>
        /// Get the bones from the model and create a bone class object for
        /// each bone. We use our bone class to do the real animated bone work.
        /// </summary>
        private void ObtainBones()
        {
            _bones.Clear();
            foreach (ModelBone bone in _model.Bones)
            {
                // Create the bone object and add to the heirarchy
                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? _bones[bone.Parent.Index] : null);

                // Add to the bones for this model
                _bones.Add(newBone);
            }
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            foreach (Bone bone in Bones)
            {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }

        #endregion
        

    }
}