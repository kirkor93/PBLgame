using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class Animator : Component
    {
        public Animator(GameObject owner) : base(owner)
        {

        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
