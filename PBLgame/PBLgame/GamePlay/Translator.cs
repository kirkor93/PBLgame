using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace PBLgame.GamePlay
{
    class Translator : Component
    {
        public bool IsTriggered = false;
        public int TranslationTicks = 300;
        public Translator(Translator source, GameObject newOwner) : base(newOwner)
        {

        }

        public Translator(GameObject owner) : base(owner)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (!IsTriggered || TranslationTicks <= 0)
            {
                return;
            }

            TranslationTicks--;
            gameObject.transform.Translate(new Vector3(0.0f, 0.0f, -0.1f));
            base.Update(gameTime);
        }

        public override Component Copy(GameObject newOwner)
        {
            return new Translator(this, newOwner);
        }
    }
}