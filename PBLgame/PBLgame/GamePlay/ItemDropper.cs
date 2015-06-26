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
        private static double DropProbability = 0.5f;
        private static double HPProbability = 0.5f;

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
            potion.particleSystem = new Billboard(potion, new Vector2(3f, 3f), 10f)
            {
                Material = ResourceManager.Instance.GetMaterial((type == PotionType.Health) ? 100 : 101),
                Alpha = 0.5f
            };
            potion.collision = new Collision(potion)
            {
                Rigidbody = false,
                Static = false,
                Enabled = true
            };
            potion.collision.MainCollider = new SphereCollider(potion.collision, 10f, true);
            potion.collision.OnTrigger += PlayerGrab;

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

            potion.Enabled = false;
            potion.RemoveFromScene();
        }
    }
}