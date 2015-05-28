namespace PBLgame.GamePlay
{

    public class Stat
    {
        public int Value { get; private set; }
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
        public bool Decrease(int amount)
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

        public bool Increase(int amount)
        {
            if (Value == MaxValue) return false;
            Value += amount;
            if (Value > MaxValue) Value = MaxValue;
            return true;
        }
    }

    public class PlayerStatistics
    {
        public Stat Health { get; private set; }
        public Stat Energy { get; private set; }

        public PlayerStatistics(int health, int energy)
        {
            Health = new Stat(health, health);
            Energy = new Stat(energy, energy);
        }
        
        public override string ToString()
        {
            return string.Format("HP: {0}, Mana: {1}", Health.Value, Energy.Value);
        }
    }
}
