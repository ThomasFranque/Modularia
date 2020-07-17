using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enhancements
{
    public class GenericEnhancements : Enhancement
    {
        public GenericEnhancements() : base()
        {
            AllModules = new EnhancementModule[5]
            {
                new EnhancementModule("Pew Capacity", 5, 1, exponentialCostPercentage : 0.05f),
                new EnhancementModule("Pew Replenish Rate", 10, 0.05f, 18, exponentialCostPercentage : 0.05f),
                new EnhancementModule("Pew Pew Speed", 30, 0.05f, exponentialCostPercentage : 0.1f),
                new EnhancementModule("Pew Damage", 35, 1, exponentialCostPercentage : 0.2f),
                new EnhancementModule("Speeeeeed", 50, 0.15f, 15, exponentialCostPercentage : 0.6f),
            };
        }
    }
}