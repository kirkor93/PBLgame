using System;
using AnimationAux;
using Microsoft.Xna.Framework;
using PBLgame.Engine;
using PBLgame.Engine.Components;
using PBLgame.GamePlay;

namespace PBLgame.GamePlay
{
    public abstract class Avatar
    {
        protected Animator _animator;
        protected AnimationClip _ouch;

        protected Avatar(Animator animator)
        {
            _animator = animator;
        }

        public static Avatar CreateAvatar(Animator animator)
        {
            Avatar avatar = PrepareAvatar(animator);
            if (animator == null) return avatar;

            avatar._ouch = animator.GetClip("Ouch");

            return avatar;
        }

        private static Avatar PrepareAvatar(Animator animator)
        {
            if (animator == null) return new StillAvatar();

            AnimationClip idle = animator.GetClip("Idle");
            AnimationClip forward = animator.GetClip("Walk");
            AnimationClip back = animator.GetClip("WalkBack");
            AnimationClip left = animator.GetClip("WalkLeft");
            AnimationClip right = animator.GetClip("WalkRight");

            if (idle == null) return new StillAvatar();
            if (forward == null) return new IdleAvatar(animator, idle);
            if (back == null) return new ForwardAvatar(animator, idle, forward);
            if (left == null || right == null) return new FwdBackAvatar(animator, idle, forward, back);
            return new QuadAvatar(animator, idle, forward, back, left, right);
        }


        public virtual void Update(Vector2 velocity, float lookAngle)
        {
        }

        public virtual void Ouch(Action onFinish = null)
        {
            if (_ouch == null)
            {
                if (onFinish != null) onFinish();
                return;
            }
            _animator.PlayAnimation(_ouch, false, 1f, 0.2f);
            _animator.OnAnimationFinish += delegate
            {
                _animator.Idle();
                if (onFinish != null) onFinish();
            };
        }
    }


    public class IdleAvatar : Avatar
    {
        protected readonly AnimationClip _idle;

        public IdleAvatar(Animator animator, AnimationClip idle) : base(animator)
        {
            _idle = idle;
        }
    }

    public class StillAvatar : Avatar
    {
        public StillAvatar() : base(null) { }
        public override void Update(Vector2 velocity, float lookAngle) { }
    }


    public class ForwardAvatar : IdleAvatar
    {
        protected readonly AnimationClip _forward;

        public ForwardAvatar(Animator animator, AnimationClip idle, AnimationClip forward) : base(animator, idle)
        {
            _forward = forward;
        }

        public override void Update(Vector2 velocity, float lookAngle)
        {
            float v = velocity.Length();
            if (v < 0.0001)
            {
                if(_animator.Clip.Type.StartsWith("Walk")) _animator.PlayAnimation(_idle);
            }
            else
            {
                if (_animator.Clip == _forward)
                {
                    _animator.Speed = v;
                }
                else
                {
                    _animator.PlayAnimation(_forward, true, v);
                }
            }
        }
    }


    public class FwdBackAvatar : ForwardAvatar
    {
        protected readonly AnimationClip _back;

        public FwdBackAvatar(Animator animator, AnimationClip idle, AnimationClip forward, AnimationClip back) : base(animator, idle, forward)
        {
            _back = back;
        }

        public override void Update(Vector2 velocity, float lookAngle)
        {
            float v = velocity.Length();
            if (v < 0.0001)
            {
                if (_animator.Clip.Type.StartsWith("Walk")) _animator.PlayAnimation(_idle);
            }
            else
            {
                float velocityAngle = Extensions.CalculateDegrees(velocity);
                float diff = Extensions.NormalizeAngle180(lookAngle - velocityAngle);
                AnimationClip clip = ChooseWalkClip(diff);

                if (_animator.Clip == clip)
                {
                    _animator.Speed = v;
                }
                else
                {
                    _animator.PlayAnimation(clip, true, v);
                }
            }
        }

        protected virtual AnimationClip ChooseWalkClip(float diff)
        {
            return (Math.Abs(diff) < 90) ? _forward : _back;
        }
    }


    public class QuadAvatar : FwdBackAvatar
    {
        private readonly AnimationClip _left;
        private readonly AnimationClip _right;

        public QuadAvatar(Animator animator, AnimationClip idle, AnimationClip forward, AnimationClip back, AnimationClip left, AnimationClip right)
            : base(animator, idle, forward, back)
        {
            _left = left;
            _right = right;
        }

        protected override AnimationClip ChooseWalkClip(float diff)
        {
            if (diff >= -45f && diff <= 45f) return _forward;
            if (diff > -135f && diff < -45f) return _left;
            if (diff >   45f && diff < 135f) return _right;
            return _back;
        }

    }
}