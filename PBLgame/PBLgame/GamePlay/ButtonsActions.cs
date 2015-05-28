using System;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace PBLgame.GamePlay
{
    public class ButtonsActions
    {
        public void UpgradeStrengthButton()
        {
            Console.WriteLine("Strength upgraded");
        }

        public void UpgradeManaButton()
        {
            Console.WriteLine("Mana upgraded");
        }
        public void UpgradeFastAttackButton()
        {
            Console.WriteLine("Fast attack upgraded");
        }

        public void UpgradeStrongAttackButton()
        {
            Console.WriteLine("Strong attack upgraded");
        }

        public void UpgradeTelekineticPushButton()
        {
            Console.WriteLine("Telekinetic push upgraded");
        }

        public void UpgradeShieldButton()
        {
            Console.WriteLine("Shield upgraded");
        }

        public void UpgradeShootingButton()
        {
            Console.WriteLine("Shooting upgraded");
        }
    }
}