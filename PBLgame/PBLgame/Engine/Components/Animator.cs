using System;
using System.Collections.Generic;
using AnimationAux;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using PBLgame.Engine.GameObjects;


namespace PBLgame.Engine.Components
{
    /// <summary>
    /// Animation clip player. It maps an animation clip onto a model
    /// </summary>
    public class Animator : Component
    {
        /// <summary>
        /// 
        /// </summary>
        private class AnimationState
        {
            public AnimationClip Clip;

            /// <summary>
            /// We maintain a BoneInfo class for each bone. This class does
            /// most of the work in playing the animation.
            /// </summary>
            public BoneInfo[] BoneInfos;

            /// <summary>
            /// The number of bones
            /// </summary>
            public int BoneCnt;

            /// <summary>
            /// Current position in time in the clip
            /// </summary>
            public float Position
            {
                get { return _position; }
                set
                {
                    if (value > Duration)
                        value = Duration;

                    _position = value;
                    foreach (BoneInfo bone in BoneInfos)
                    {
                        bone.SetPosition(_position);
                    }
                }
            }

            public void ApplyBones()
            {
                foreach (BoneInfo bone in BoneInfos)
                {
                    bone.AssignToBone();
                }
            }

            /// <summary>
            /// Blends current bones with other using given factor. Applies blending into bones.
            /// </summary>
            /// <param name="prevState">previous animation state</param>
            /// <param name="amount">How much previous state is shown more than this state [0.0 .. 1.0]</param>
            public void Blend(AnimationState prevState, float amount)
            {
                for (int i = 0; i < BoneInfos.Length; i++)
                {
                    if (BoneInfos[i].ClipBone.Name != prevState.BoneInfos[i].ClipBone.Name)
                    {
                        // hotfix when different bone indices
                        string name = BoneInfos[i].ClipBone.Name;
                        for (int bone = 0; bone < BoneCnt; bone++)
                        {
                            if (prevState.BoneInfos[bone].ClipBone.Name == name)
                            {
                                BoneInfo tmp = prevState.BoneInfos[bone];
                                prevState.BoneInfos[bone] = prevState.BoneInfos[i];
                                prevState.BoneInfos[i] = tmp;
                                break;
                            }
                        }
                    }
                    BoneInfos[i].rotation = Quaternion.Slerp(BoneInfos[i].rotation, prevState.BoneInfos[i].rotation, amount);
                    BoneInfos[i].translation = Vector3.Lerp(BoneInfos[i].translation, prevState.BoneInfos[i].translation, amount);
                    BoneInfos[i].AssignToBone();
                }
            }

            /// <summary>
            /// The clip duration
            /// </summary>
            public float Duration { get { return (float) Clip.Duration; } }

            /// <summary>
            /// The looping option
            /// </summary>
            public bool Looping = true;

            /// <summary>
            /// Additional speed multiplier.
            /// </summary>
            public float Speed = 1f;

            private float _position = 0f;

            public AnimationState(AnimationClip clip, AnimatedMesh mesh)
            {
                Clip = clip;

                // Create the bone information classes
                BoneCnt = clip.Bones.Count;
                BoneInfos = new BoneInfo[BoneCnt];

                for (int b = 0; b < BoneInfos.Length; b++)
                {
                    // Create it
                    BoneInfos[b] = new BoneInfo(clip.Bones[b]);

                    // Assign it to a model bone
                    BoneInfos[b].SetModel(mesh);
                }

                Position = 0;
                //ApplyBones();
            }
        }

        #region Fields

        /// <summary>
        /// The clip we are playing
        /// </summary>
        private AnimationState _currentAnimation;
        
        /// <summary>
        /// Previous clip for blending.
        /// </summary>
        private AnimationState _prevAnimation;

        private AnimationType _currentType = AnimationType.Other;

        private float _blendingFactor;
        private float _blendingTime;

        public event Action OnAnimationFinish;

        #endregion

        #region Properties

        /// <summary>
        /// The position in the current animation
        /// </summary>
        [Browsable(false)]
        public float Position
        {
            get { return _currentAnimation.Position; }
            set { _currentAnimation.Position = value; _currentAnimation.ApplyBones(); }
        }

        /// <summary>
        /// The associated animation clip
        /// </summary>
        [Browsable(false)]
        public AnimationClip Clip { get { return _currentAnimation.Clip; } }

        /// <summary>
        /// The clip duration
        /// </summary>
        [Browsable(false)]
        public float Duration { get { return _currentAnimation.Duration; } }

        /// <summary>
        /// The looping option. Set to true if you want the animation to loop
        /// back at the end
        /// </summary>
        public bool Looping { get { return _currentAnimation.Looping; } set { _currentAnimation.Looping = value; } }

        /// <summary>
        /// Additional speed multiplier. For example to synchronize walking speed with animation.
        /// </summary>
        public float Speed { get { return _currentAnimation.Speed; } set { _currentAnimation.Speed = value; } }

        public AnimatedMesh AnimMesh { get { return (AnimatedMesh) _gameObject.renderer.MyMesh; } }

        public Matrix[] BoneTransforms { get; private set; }
        public Matrix[] SkeletonMatrix { get; private set; }

        #endregion


        public Animator(GameObject owner) : base(owner)
        {
            // don't forget to implement copy constructor below
        }

        public Animator(Animator src, GameObject owner) : base(owner)
        {
            Initialize(false);
        }

        //public void ApplyBones()
        //{
        //    _currentAnimation.Position = _currentAnimation.Position;
        //    _currentAnimation.ApplyBones();
        //    AnimMesh.UpdateBonesMatrices();
        //    AttachSlot slot = gameObject.GetComponent<AttachSlot>();
        //    if(slot != null) slot.Update(Game.Instance.Time);
        //}

        /// <summary>
        /// Set new animation for playing. Allows blending with previous animation.
        /// </summary>
        /// <param name="clip">clip to animate</param>
        /// <param name="loop">loop animation</param>
        /// <param name="speed">speed multiplier</param>
        /// <param name="blendTime">blending time in seconds (use 0 to disable blending)</param>
        public void PlayAnimation(AnimationClip clip, bool loop = true, float speed = 1.0f, float blendTime = 0.3f)
        {
            _prevAnimation = _currentAnimation;
            _currentAnimation = new AnimationState(clip, AnimMesh) {Looping = loop, Speed = speed};

            if (blendTime == 0f)
            {
                _prevAnimation = null;
            }
            else
            {
                _blendingFactor = 1f;
                _blendingTime = blendTime;
            }
            UpdateBoneMatrices();
        }

        public void Walk(float velocity)
        {
            if (_currentType == AnimationType.Walk)
            {
                Speed = velocity;
            }
            else
            {
                _currentType = AnimationType.Walk;
                PlayAnimation(AnimMesh.Skeleton.Walk, true, velocity);
            }
        }

        public void Idle()
        {
            if (_currentType != AnimationType.Idle)
            {
                _currentType = AnimationType.Idle;
                AnimationClip idleClip = AnimMesh.Skeleton.Idle;
                if (idleClip != null) PlayAnimation(idleClip, true, 1.0f);
                else
                {
                    // TODO this is a temporary workaround
                    // fade to beginning
                    PlayAnimation(Clip, false, 0);
                }
            }
        }

        public void Attack(string type = "")
        {
            if (_currentType != AnimationType.Attack)
            {
                _currentType = AnimationType.Attack;
                PlayAnimation(GetClip("Attack" + type), false);
                OnAnimationFinish += Idle;
            }
        }

        public void Death()
        {
            if (_currentType != AnimationType.Death)
            {
                _currentType = AnimationType.Death;
                PlayAnimation(GetClip("Death"), false);
                OnAnimationFinish = null;
            }
        }

        public AnimationClip GetClip(string type)
        {
            return AnimMesh.Skeleton.FromType(type);
        }

        public enum AnimationType
        {
            Idle, Walk, Other,
            Attack,
            Death
        }

        #region Update and Transport Controls


        /// <summary>
        /// Reset back to time zero.
        /// </summary>
        public void Rewind()
        {
            Position = 0;
        }


        public override void Initialize(bool editor)
        {
            if (_currentAnimation == null)
            {
                PlayAnimation(AnimMesh.Skeleton.Idle ?? AnimMesh.Skeleton.Walk ?? AnimMesh.Clips[0]);
            }
        }

        public override Component Copy(GameObject newOwner)
        {
            return new Animator(this, newOwner);
        }

        /// <summary>
        /// Update the clip position. Also updates bones in the associated model.
        /// </summary>
        /// <param name="gameTime">time</param>
        public override void Update(GameTime gameTime)
        {
            UpdatePosition(_currentAnimation, gameTime);
            _currentAnimation.ApplyBones();
            if (_prevAnimation != null)
            {
                _blendingFactor -= (float) gameTime.ElapsedGameTime.TotalSeconds / _blendingTime;
                if (_blendingFactor > 0)
                {
                    //UpdatePosition(_prevAnimation, gameTime);
                    _currentAnimation.Blend(_prevAnimation, _blendingFactor);
                }
                else
                {
                    _prevAnimation = null;
                    _blendingFactor = 0f;
                }
            }
            else
            {
                _currentAnimation.ApplyBones();
            }
            UpdateBoneMatrices();
        }

        public void UpdateBoneMatrices()
        {
            AnimMesh.UpdateBonesMatrices();
            BoneTransforms = AnimMesh.BonesTransorms;
            SkeletonMatrix = AnimMesh.SkeletonMatrix;
        }

        private void UpdatePosition(AnimationState animation, GameTime gameTime)
        {
            float newPosition = animation.Position + (float) gameTime.ElapsedGameTime.TotalSeconds * animation.Clip.Speed * animation.Speed;

            if (animation.Looping && animation.Duration > 0.0)
            {
                while (newPosition >= animation.Duration)
                {
                    newPosition -= animation.Duration;
                }
                while (newPosition < 0)
                {
                    newPosition = newPosition + animation.Duration;
                }
            }
            else if(!animation.Looping && newPosition >= animation.Duration)
            {
                if (OnAnimationFinish != null)
                {
                    OnAnimationFinish();
                    OnAnimationFinish = null;
                }
            }
            animation.Position = newPosition;
        }

        #endregion

        #region BoneInfo class


        /// <summary>
        /// Information about a bone we are animating. This class connects a bone
        /// in the clip to a bone in the model.
        /// </summary>
        private class BoneInfo
        {
            #region Fields

            /// <summary>
            /// The current keyframe. Our position is a time such that the 
            /// we are greater than or equal to this keyframe's time and less
            /// than the next keyframes time.
            /// </summary>
            private int currentKeyframe = 0;

            /// <summary>
            /// Bone in a model that this keyframe bone is assigned to
            /// </summary>
            public Bone assignedBone = null;

            /// <summary>
            /// We are not valid until the rotation and translation are set.
            /// If there are no keyframes, we will never be valid
            /// </summary>
            public bool valid = false;

            /// <summary>
            /// Current animation rotation
            /// </summary>
            public Quaternion rotation;

            /// <summary>
            /// Current animation translation
            /// </summary>
            public Vector3 translation;

            /// <summary>
            /// We are at a location between Keyframe1 and Keyframe2 such 
            /// that Keyframe1's time is less than or equal to the current position
            /// </summary>
            public AnimationClip.Keyframe Keyframe1;

            /// <summary>
            /// Second keyframe value
            /// </summary>
            public AnimationClip.Keyframe Keyframe2;

            #endregion

            #region Properties

            /// <summary>
            /// The bone in the actual animation clip
            /// </summary>
            public AnimationClip.Bone ClipBone { get; set; }

            /// <summary>
            /// The bone this animation bone is assigned to in the model
            /// </summary>
            public Bone ModelBone { get { return assignedBone; } }

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bone"></param>
            public BoneInfo(AnimationClip.Bone bone)
            {
                this.ClipBone = bone;
                SetKeyframes();
                SetPosition(0);
            }


            #endregion

            #region Position and Keyframes

            /// <summary>
            /// Set the bone based on the supplied position value. 
            /// Call AssignToBone() to apply changes in bones.
            /// </summary>
            /// <param name="position"></param>
            public void SetPosition(float position)
            {
                if (ClipBone.Keyframes.Count == 0) return;

                // If our current position is less that the first keyframe
                // we move the position backward until we get to the right keyframe
                while (position < Keyframe1.Time && currentKeyframe > 0)
                {
                    // We need to move backwards in time
                    currentKeyframe--;
                    SetKeyframes();
                }

                // If our current position is greater than the second keyframe
                // we move the position forward until we get to the right keyframe
                while (position >= Keyframe2.Time && currentKeyframe < ClipBone.Keyframes.Count - 2)
                {
                    // We need to move forwards in time
                    currentKeyframe++;
                    SetKeyframes();
                }

                if (Keyframe1 == Keyframe2)
                {
                    // Keyframes are equal
                    rotation = Keyframe1.Rotation;
                    translation = Keyframe1.Translation;
                }
                else
                {
                    // Interpolate between keyframes
                    float t = (float)((position - Keyframe1.Time) / (Keyframe2.Time - Keyframe1.Time));
                    rotation = Quaternion.Slerp(Keyframe1.Rotation, Keyframe2.Rotation, t);
                    translation = Vector3.Lerp(Keyframe1.Translation, Keyframe2.Translation, t);
                }

                valid = true;

                //AssignToBone();
            }

            public void AssignToBone()
            {
                if (assignedBone != null)
                {
                    // Send to the model
                    // Make it a matrix first
                    Matrix m = Matrix.CreateFromQuaternion(rotation);
                    m.Translation = translation;

                    assignedBone.SetCompleteTransform(m);
                }
            }


            /// <summary>
            /// Set the keyframes to a valid value relative to 
            /// the current keyframe
            /// </summary>
            private void SetKeyframes()
            {
                if (ClipBone.Keyframes.Count > 0)
                {
                    Keyframe1 = ClipBone.Keyframes[currentKeyframe];
                    if (currentKeyframe == ClipBone.Keyframes.Count - 1)
                        Keyframe2 = Keyframe1;
                    else
                        Keyframe2 = ClipBone.Keyframes[currentKeyframe + 1];
                }
                else
                {
                    // If there are no keyframes, set both to null
                    Keyframe1 = null;
                    Keyframe2 = null;
                }
            }

            /// <summary>
            /// Assign this bone to the correct bone in the model
            /// </summary>
            /// <param name="model"></param>
            public void SetModel(AnimatedMesh model)
            {
                // Find this bone
                assignedBone = model.FindBone(ClipBone.Name);

            }

            #endregion
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

    public class Skeleton
    {
        public int Id;
        public List<AnimationClip> Clips { get; private set; }
        public Dictionary<string, AnimationClip> ClipsDictionary { get; private set; } 

        public AnimationClip Idle { get { return Clips.FirstOrDefault(c => c.Type == "Idle"); } }
        public AnimationClip Walk { get { return Clips.FirstOrDefault(c => c.Type == "Walk"); } }

        public Skeleton(int id)
        {
            Id = id;
            Clips = new List<AnimationClip>();
            ClipsDictionary = new Dictionary<string, AnimationClip>();
        }

        public void AddClip(AnimationClip animation)
        {
            Clips.Add(animation);
            if (animation.Type == null) return;
            ClipsDictionary[animation.Type] = animation;
        }

        public AnimationClip FromType(string type)
        {
            AnimationClip clip;
            return ClipsDictionary.TryGetValue(type, out clip) ? clip : null;
        }
    }
}
