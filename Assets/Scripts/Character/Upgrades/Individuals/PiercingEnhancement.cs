using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enhancements
{
    public class PiercingEnhancement : Enhancement
    {
        public PiercingEnhancement() : base()
        {
            Icon = Icons.Piercing;
            AllModules = new EnhancementModule[2]
            {
                new EnhancementModule("Max Slooshes", 200, 1f, exponentialCostPercentage : 0.4f),
                new EnhancementModule("Sloosh damage multiplier", 100, 0.1f, exponentialCostPercentage : 0.5f)
            };
        }
    }
}