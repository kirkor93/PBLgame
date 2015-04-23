#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

namespace AnimationData
{
    /// <summary>
    /// The animation player is in charge of decoding bone position
    /// matrices from an animation clip.
    /// </summary>
    public class AnimationPlayer
    {
        #region Fields


        // Information about the currently playing animation clip.
        private AnimationClip _currentClipValue;
        private TimeSpan _currentTimeValue;
        private int _currentKeyframe;


        // Current animation transform matrices.
        private Matrix[] _boneTransforms;
        private Matrix[] _worldTransforms;
        private Matrix[] _skinTransforms;


        // Backlink to the bind pose and skeleton hierarchy data.
        private SkinningData _skinningDataValue;


        #endregion


        /// <summary>
        /// Constructs a new animation player.
        /// </summary>
        public AnimationPlayer(SkinningData skinningData)
        {
            if (skinningData == null)
                throw new ArgumentNullException("skinningData");

            _skinningDataValue = skinningData;

            _boneTransforms = new Matrix[skinningData.BindPose.Count];
            _worldTransforms = new Matrix[skinningData.BindPose.Count];
            _skinTransforms = new Matrix[skinningData.BindPose.Count];
        }


        /// <summary>
        /// Starts decoding the specified animation clip.
        /// </summary>
        public void StartClip(AnimationClip clip)
        {
            if (clip == null)
                throw new ArgumentNullException("clip");

            _currentClipValue = clip;
            _currentTimeValue = TimeSpan.Zero;
            _currentKeyframe = 0;

            // Initialize bone transforms to the bind pose.
            _skinningDataValue.BindPose.CopyTo(_boneTransforms, 0);
        }


        /// <summary>
        /// Advances the current animation position.
        /// </summary>
        public void Update(TimeSpan time, bool relativeToCurrentTime,
                           Matrix rootTransform)
        {
            UpdateBoneTransforms(time, relativeToCurrentTime);
            UpdateWorldTransforms(rootTransform);
            UpdateSkinTransforms();
        }


        /// <summary>
        /// Helper used by the Update method to refresh the BoneTransforms data.
        /// </summary>
        public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
        {
            if (_currentClipValue == null)
                throw new InvalidOperationException(
                            "AnimationPlayer.Update was called before StartClip");

            // Update the animation position.
            if (relativeToCurrentTime)
            {
                time += _currentTimeValue;

                // If we reached the end, loop back to the start.
                while (time >= _currentClipValue.Duration)
                    time -= _currentClipValue.Duration;
            }

            if ((time < TimeSpan.Zero) || (time >= _currentClipValue.Duration))
                throw new ArgumentOutOfRangeException("time");

            // If the position moved backwards, reset the keyframe index.
            if (time < _currentTimeValue)
            {
                _currentKeyframe = 0;
                _skinningDataValue.BindPose.CopyTo(_boneTransforms, 0);
            }

            _currentTimeValue = time;

            // Read keyframe matrices.
            IList<Keyframe> keyframes = _currentClipValue.Keyframes;

            while (_currentKeyframe < keyframes.Count)
            {
                Keyframe keyframe = keyframes[_currentKeyframe];

                // Stop when we've read up to the current time position.
                if (keyframe.Time > _currentTimeValue)
                    break;

                // Use this keyframe.
                _boneTransforms[keyframe.Bone] = keyframe.Transform;

                _currentKeyframe++;
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the WorldTransforms data.
        /// </summary>
        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            // Root bone.
            _worldTransforms[0] = _boneTransforms[0] * rootTransform;

            // Child bones.
            for (int bone = 1; bone < _worldTransforms.Length; bone++)
            {
                int parentBone = _skinningDataValue.SkeletonHierarchy[bone];

                _worldTransforms[bone] = _boneTransforms[bone] *
                                             _worldTransforms[parentBone];
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the SkinTransforms data.
        /// </summary>
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < _skinTransforms.Length; bone++)
            {
                _skinTransforms[bone] = _skinningDataValue.InverseBindPose[bone] *
                                            _worldTransforms[bone];
            }
        }


        /// <summary>
        /// Gets the current bone transform matrices, relative to their parent bones.
        /// </summary>
        public Matrix[] GetBoneTransforms()
        {
            return _boneTransforms;
        }


        /// <summary>
        /// Gets the current bone transform matrices, in absolute format.
        /// </summary>
        public Matrix[] GetWorldTransforms()
        {
            return _worldTransforms;
        }


        /// <summary>
        /// Gets the current bone transform matrices,
        /// relative to the skinning bind pose.
        /// </summary>
        public Matrix[] GetSkinTransforms()
        {
            return _skinTransforms;
        }


        /// <summary>
        /// Gets the clip currently being decoded.
        /// </summary>
        public AnimationClip CurrentClip
        {
            get { return _currentClipValue; }
        }


        /// <summary>
        /// Gets the current play position.
        /// </summary>
        public TimeSpan CurrentTime
        {
            get { return _currentTimeValue; }
        }
    }
}
