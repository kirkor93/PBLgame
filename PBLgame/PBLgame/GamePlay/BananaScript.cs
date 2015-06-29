using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PBLgame.Engine;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.GUI;
using PBLgame.Engine.Singleton;
using PBLgame.Engine.Physics;

namespace PBLgame.GamePlay
{
    public class BananaScript : CharacterHandler
    {
        #region Variables
        public GameObject Player;

        private bool _activated;
        private Vector3 _startPosition;
        private Vector3 _direction;
        private float _speed = 1.5f;
        private float _activeTimer = 0.0f;
        private float _activeMaxTime = 1.0f;
        #endregion

        #region Properites
        public bool Activated
        {
            get { return _activated; }
            set
            {
                _activated = value;
                if(_activated) _activeTimer = 0.0f;
                SetLookVectorInstantly(_direction);
                gameObject.collision.Enabled = value;
                gameObject.renderer.Enabled = value;
            }
        }
        public Vector3 StartPosition
        {
            get { return _startPosition; }
            set 
            { 
                _startPosition = value;
                _startPosition.Y = 10.0f;
                _gameObject.transform.Position = _startPosition;
            }
        }
        public Vector3 Direction
        {
            get { return _direction; }
            set 
            { 
                _direction = value;
                _direction.Y = 0.0f;
            }
        }
        #endregion

        #region Methods
        public BananaScript(GameObject go) : base(go)
        {
            _startPosition = new Vector3();
            _direction = new Vector3();
            _activated = false;
        }

        public override Component Copy(GameObject go)
        {
            return new BananaScript(go);
        }

        public override void Initialize(bool editor)
        {
            base.Initialize(editor);
            gameObject.renderer.Enabled = false;
            gameObject.collision.Enabled = false;
            gameObject.collision.OnTrigger += GetHit;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(_activated)
            {
                //SetLookVector(_direction);
                gameObject.transform.Position += _direction * _speed;
                _activeTimer += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                foreach (GameObject go in PhysicsSystem.CollisionObjects)
                {
                    if (gameObject != go && go != gameObject.parent && go.collision.Enabled && gameObject.collision.MainCollider.Contains(go.collision.MainCollider) != ContainmentType.Disjoint)
                    {
                        gameObject.collision.ChceckCollisionDeeper(go);
                    }
                }
                if(_activeTimer > _activeMaxTime)
                {
                    Activated = false;
                }

            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        private void GetHit(Object obj, ColArgs args)
        {
            if (args.EnemyCollider != null)
            {
                String tag = args.EnemyCollider.Owner.gameObject.Tag; 
                if (!args.EnemyCollider.Trigger && tag != "FOV" && tag != "EnemyWeapon" && tag != "EnemyWeaponCB" && tag != "Enemy" && tag != "Weapon" && tag != "Player" && tag != "Potion")
                {
                    Activated = false;
                }
            }

        }
        #endregion
    }
}
