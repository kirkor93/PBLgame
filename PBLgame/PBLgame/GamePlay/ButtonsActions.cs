using System;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace PBLgame.GamePlay
{
    public class ButtonsActions
    {
        public void UpgradeStrengthButton(PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0)
            {
                Console.WriteLine("Strength upgraded");
                stats.TalentPoints.Decrease(1);
                stats.BasePhysicalDamage.Increase(3);
            }
        }

        public void UpgradeManaButton(PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0)
            {
                Console.WriteLine("Mana upgraded");
                stats.TalentPoints.Decrease(1);
                stats.Energy.Increase(10);
            }
        }
        public void UpgradeFastAttackButton(PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0)
            {
                Console.WriteLine("Fast attack upgraded");
                stats.TalentPoints.Decrease(1);
                stats.FastAttackDamageBonus.Increase(2);
            }
        }

        public void UpgradeStrongAttackButton(PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0)
            {
                Console.WriteLine("Strong attack upgraded");
                stats.TalentPoints.Decrease(1);
                stats.StrongAttackDamageBonus.Increase(3);
            }
        }

        public void UpgradeTelekineticPushButton(PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0)
            {
                Console.WriteLine("Telekinetic push upgraded");
                stats.TalentPoints.Decrease(1);
                stats.PushManaCost.Decrease(1);
            }
        }

        public void UpgradeShieldButton(PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0)
            {
                Console.WriteLine("Shield upgraded");
                stats.TalentPoints.Decrease(1);
                stats.ShieldAbsorption.Increase(5);
                stats.ShieldManaCost.Decrease(1);
            }
        }

        public void UpgradeShootingButton(PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0)
            {
                Console.WriteLine("Shooting upgraded");
                stats.TalentPoints.Decrease(1);
                stats.ShootDamage.Increase(4);
                stats.ShootManaCost.Decrease(1);
            }
        }
    }
}