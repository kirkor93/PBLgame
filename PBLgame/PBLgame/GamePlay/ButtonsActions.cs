using System;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.GUI;

namespace PBLgame.GamePlay
{
    public class ButtonsActions
    {
        public void UpgradeStrengthButton(Button button, PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0 && stats.BasePhysicalDamage.IsUpgradable)
            {
                Console.WriteLine("Strength upgraded");
                stats.TalentPoints.Decrease(1);
                stats.BasePhysicalDamage.Increase(2 + stats.BasePhysicalDamage.Progress);
                stats.BasePhysicalDamage.Progress++;
                button.SetProgressBarsValue(stats.BasePhysicalDamage.Progress);
                if (button.DownNeighbour != null && stats.FastAttackDamageBonus.IsUpgradable)
                {
                    button.DownNeighbour.MakeClickable(true);
                    if (button.DownNeighbour.RightNeighbour != null
                        && stats.ShootDamage.IsUpgradable
                        && stats.ShootManaCost.IsUpgradable)
                    {
                        button.DownNeighbour.RightNeighbour.MakeClickable(true);
                    }
                }
            }
        }

        public void UpgradeManaButton(Button button, PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0 && stats.Energy.IsUpgradable)
            {
                Console.WriteLine("Mana upgraded");
                stats.TalentPoints.Decrease(1);
                stats.Energy.Increase(50);
                stats.Energy.Progress++;
                button.SetProgressBarsValue(stats.Energy.Progress);
                if (button.DownNeighbour != null && stats.PushManaCost.IsUpgradable)
                {
                    button.DownNeighbour.MakeClickable(true);
                    if (button.DownNeighbour.LeftNeighbour != null
                        && stats.ShootDamage.IsUpgradable
                        && stats.ShootManaCost.IsUpgradable)
                    {
                        button.DownNeighbour.LeftNeighbour.MakeClickable(true);
                    }
                }
            }
        }
        public void UpgradeFastAttackButton(Button button, PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0 && stats.FastAttackDamageBonus.IsUpgradable)
            {
                Console.WriteLine("Fast attack upgraded");
                stats.TalentPoints.Decrease(1);
                stats.FastAttackDamageBonus.Increase(8);
                stats.FastAttackDamageBonus.Progress++;
                button.SetProgressBarsValue(stats.FastAttackDamageBonus.Progress);
                if (button.DownNeighbour != null && stats.StrongAttackDamageBonus.IsUpgradable)
                {
                    button.DownNeighbour.MakeClickable(true);
                }
            }
        }

        public void UpgradeStrongAttackButton(Button button, PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0 && stats.StrongAttackDamageBonus.IsUpgradable)
            {
                Console.WriteLine("Strong attack upgraded");
                stats.TalentPoints.Decrease(12);
                stats.StrongAttackDamageBonus.Increase(3);
                stats.StrongAttackDamageBonus.Progress++;
                button.SetProgressBarsValue(stats.StrongAttackDamageBonus.Progress);
            }
        }

        public void UpgradeTelekineticPushButton(Button button, PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0 && stats.PushManaCost.IsUpgradable)
            {
                Console.WriteLine("Telekinetic push upgraded");
                stats.TalentPoints.Decrease(1);
                stats.PushManaCost.Decrease(1);
                stats.PushManaCost.Progress++;
                button.SetProgressBarsValue(stats.PushManaCost.Progress);
                if (button.DownNeighbour != null 
                    && stats.ShieldAbsorption.IsUpgradable
                    && stats.ShieldManaCost.IsUpgradable)
                {
                    button.DownNeighbour.MakeClickable(true);
                }
            }
        }

        public void UpgradeShieldButton(Button button, PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0 
                && stats.ShieldAbsorption.IsUpgradable
                && stats.ShieldManaCost.IsUpgradable)
            {
                Console.WriteLine("Shield upgraded");
                stats.TalentPoints.Decrease(1);
                stats.ShieldAbsorption.Increase(10);
                stats.ShieldManaCost.Increase(2);
                stats.ShieldAbsorption.Progress++;
                stats.ShieldManaCost.Progress++;
                button.SetProgressBarsValue(stats.ShieldAbsorption.Progress);
            }
        }

        public void UpgradeShootingButton(Button button, PlayerStatistics stats)
        {
            if (stats.TalentPoints.Value > 0
                && stats.ShootDamage.IsUpgradable
                && stats.ShootManaCost.IsUpgradable)
            {
                Console.WriteLine("Shooting upgraded");
                stats.TalentPoints.Decrease(1);
                stats.ShootDamage.Increase(10);
                stats.ShootManaCost.Decrease(5);
                stats.ShootDamage.Progress++;
                stats.ShootManaCost.Progress++;
                button.SetProgressBarsValue(stats.ShootDamage.Progress);
            }
        }
    }
}