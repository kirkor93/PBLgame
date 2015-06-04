using System;
using PBLgame.Engine.GUI;

namespace PBLgame.GamePlay
{

    public class Stat
    {
        public int Value { get; protected set; }
        public int MaxValue { get; set; }

        public Stat(int value, int maxValue)
        {
            if (value > maxValue) value = maxValue;
            Value = value;
            MaxValue = maxValue;
        }

        public bool CanDecrease(int amount)
        {
            if (amount > Value) return false;
            return true;
        }

        /// <summary>
        /// Decreases stat by given amount. If amount more then stat then becomes 0 and returns false;
        /// </summary>
        /// <param name="amount">amount to decrease</param>
        /// <returns>false if new value would be below 0</returns>
        public virtual bool Decrease(int amount)
        {
            if (amount > Value)
            {
                Value = 0;
                return false;
            }
            Value -= amount;
            return true;
        }

        /// <summary>
        /// Decreases stat by given amount only if new value isn't below 0.
        /// </summary>
        /// <param name="amount">amount to decrease</param>
        /// <returns>false if cannot decrease</returns>
        public bool TryDecrease(int amount)
        {
            if (!CanDecrease(amount)) return false;
            Value -= amount;
            return true;
        }

        public virtual bool Increase(int amount)
        {
            if (Value == MaxValue) return false;
            Value += amount;
            if (Value > MaxValue) Value = MaxValue;
            return true;
        }
    }

    public class ExperienceStat : Stat
    {
        public event EventHandler<EventArgs> OnLevelUp; 
        public ExperienceStat(int value, int maxValue) : base(value, maxValue)
        {
        }

        public override bool Increase(int amount)
        {
            Value += amount;
            if (Value >= MaxValue)
            {
                Value %= MaxValue;
                if (OnLevelUp != null)
                {
                    OnLevelUp(this, EventArgs.Empty);
                }
            }
            return true;
        }
    }

    public class PlayerStatistics
    {
        public Stat Health { get; private set; }
        public Stat Energy { get; private set; }
        public Stat Experience { get; private set ; }
        public Stat TalentPoints { get; private set; }
        public Stat BasePhysicalDamage { get; private set; }
        public Stat FastAttackDamageBonus { get; private set; }
        public Stat StrongAttackDamageBonus { get; private set; }
        public Stat ShootDamage { get; private set; }
        public Stat ShootManaCost { get; private set; }
        public Stat ShieldAbsorption { get; private set; }
        public Stat ShieldManaCost { get; private set; }
        public Stat PushManaCost { get; private set; }

        public PlayerStatistics(int health, int energy, int experience)
        {
            Health = new Stat(health, health);
            Energy = new Stat(energy, energy);
            Experience = new ExperienceStat(0, experience);
            TalentPoints = new Stat(0, int.MaxValue);
            BasePhysicalDamage = new Stat(10, int.MaxValue);
            FastAttackDamageBonus = new Stat(5, int.MaxValue);
            StrongAttackDamageBonus = new Stat(10, int.MaxValue);
            ShootDamage = new Stat(13, int.MaxValue);
            ShootManaCost = new Stat(6, int.MaxValue);
            ShieldAbsorption = new Stat(20, int.MaxValue);
            ShieldManaCost = new Stat(10, int.MaxValue);
            PushManaCost = new Stat(10, int.MaxValue);

            (Experience as ExperienceStat).OnLevelUp += OnLevelUp;
            HUD.Instance.TalentWindowManager.AssignPlayerStatisticsScript(this);
        }

        private void OnLevelUp(object sender, EventArgs eventArgs)
        {
            TalentPoints.Increase(1);
        }

        public override string ToString()
        {
            return string.Format("HP: {0}, Mana: {1}, Experience: {2}", Health.Value, Energy.Value, Experience.Value);
        }
    }
}
