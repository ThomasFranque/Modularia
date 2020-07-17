using System;
using UnityEngine;

namespace Enhancements
{
    public class EnhancementModule
    {
        public string Name { get; }
        public int MaxLevel { get; }
        public float EnhanceValue { get; }
        public int BaseCost { get; }
        public float ExponentialCostGrowth { get; }
        public float BaseValue { get; }
        public int Level { get; private set; }
        public float Value => BaseValue + (EnhanceValue * Level);
        public int Cost => (BaseCost * Level) +
            Mathf.RoundToInt(BaseCost * Level * (1 + ExponentialCostGrowth * Level));

        public bool WithinMaxLevel => Level < MaxLevel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the module</param>
        /// <param name="baseCost">The initial base cost</param>
        /// <param name="upgradeAmount">On upgrade, how much should the values increment</param>
        /// <param name="maxLevel">Optional max level</param>
        /// <param name="level">In case of a starting level</param>
        /// <param name="baseValue">Optional starting value</param>
        /// <param name="exponentialCostPercentage">How much should the cost increment per upgrade (.0f to 1.0f)</param>
        public EnhancementModule(string name, int baseCost, float upgradeAmount,
            int maxLevel = 9999, int level = 0, float baseValue = 0, float exponentialCostPercentage = 0.0f)
        {
            Name = name;
            BaseCost = baseCost;
            EnhanceValue = upgradeAmount;
            BaseValue = baseValue;
            MaxLevel = maxLevel;
            ExponentialCostGrowth = exponentialCostPercentage;

            Level = level;
        }

        public void ForceUpgrade()
        {
            if (WithinMaxLevel)
            {
                Level++;
                OnUpgrade?.Invoke();
            }
        }

        public bool TryUpgrade(int energy, out int cost)
        {
            cost = Cost;
            if (WithinMaxLevel && energy >= cost)
            {
                Level++;
                OnUpgrade?.Invoke();
                return true;
            }
            return false;
        }

        public Action OnUpgrade;
    }
}