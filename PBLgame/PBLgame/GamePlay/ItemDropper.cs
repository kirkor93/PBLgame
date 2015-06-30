using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Physics;
using PBLgame.Engine.Singleton;

namespace PBLgame.GamePlay
{
    public static class ItemDropper
    {
        private static readonly Random Rand;
        private static double DropProbability = 0.5;
        private static double HPProbability = 0.5;

        static ItemDropper()
        {
            Rand = new Random();
        }

        public static bool DropPotion(GameObject dropper, GameObject player)
        {
            if (Rand.NextDouble() < DropProbability) return false;
            PotionType type = (Rand.NextDouble() >= HPProbability) ? PotionType.Health : PotionType.Energy;

            GameObject potion = new GameObject(dropper.Scene)
            {
                Name = type.ToString(),
                Tag = "Potion",
                parent = dropper
            };
            potion.transform = new Transform(potion)
            {
                Position = new Vector3(0f, 6f, 0f),
                Scale = new Vector3(3f)
            };
            MeshMaterial material = ResourceManager.Instance.GetMaterial((type == PotionType.Health) ? 102 : 103);
            potion.renderer = new Renderer(potion)
            {
                EmissiveValue = 1.0f,
                MyMesh = ResourceManager.Instance.GetMesh(@"Models\Items\Potion"),
                Material = material,
                MyEffect = material.ShaderEffect
            };
            potion.collision = new Collision(potion)
            {
                Rigidbody = false,
                Static = false,
                Enabled = true
            };
            potion.collision.MainCollider = new SphereCollider(potion.collision, 3f, true);
            potion.collision.OnTrigger += PlayerGrab;
            potion.AddComponent(new PotionAnimationComponent(potion));

            dropper.Scene.AddTemporary(potion);
            return true;
        }

        private static void PlayerGrab(object sender, ColArgs args)
        {
            Collider playerCollider = args.EnemyCollider;
            GameObject potion = ( args.MyCollider ).Owner.gameObject;
            PlayerScript player = playerCollider.Owner.gameObject.GetComponent<PlayerScript>();
            if (player == null) return;

            PotionType type;
            if (!Enum.TryParse(potion.Name, out type)) return;
            switch (type)
            {
                 case PotionType.Health:
                    player.Stats.HealthPotions.Increase(1);
                    break;

                case PotionType.Energy:
                    player.Stats.EnergyPotions.Increase(1);
                    break;

                default:
                    return;
            }
            player.gameObject.audioSource.Play("PotionGrab");
            potion.Enabled = false;
            potion.RemoveFromScene();
        }
    }

    public class PotionAnimationComponent : Component
    {
        private float _speed;
        private float _position;
        private float _t;
        private readonly float _period;
        private Vector3 _initPos;

        public PotionAnimationComponent(GameObject owner) : base(owner)
        {
            _t = 0f;
            _speed = 6.0f;
            _position = 0f;
            _period = MathHelper.TwoPi;
            _initPos = _gameObject.transform.Position;
        }

        public override void Update(GameTime gameTime)
        {
            _t += (float) gameTime.ElapsedGameTime.TotalSeconds * _speed;
            while (_t >= _period) _t -= _period;
            float val = (float) (0.5 * Math.Sin(_t) + 0.5);
            _gameObject.renderer.EmissiveValue = val;
            _gameObject.transform.Position = new Vector3(_initPos.X, _initPos.Y + 2 * val, _initPos.Z);
        }

        public override Component Copy(GameObject newOwner)
        {
            return new PotionAnimationComponent(newOwner);
        }
    }
}