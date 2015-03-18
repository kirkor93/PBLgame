using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class Collision : Component 
    {
        public Collision(GameObject owner) : base(owner)
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
