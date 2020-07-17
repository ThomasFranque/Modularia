using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enhancements
{
    public class ExplosiveEnhancement : Enhancement
    {
        public ExplosiveEnhancement() : base()
        {
            Icon = Icons.Explosive;
            AllModules = new EnhancementModule[2]
            {
                new EnhancementModule("Boom Chance", 100, 0.1f, 10, exponentialCostPercentage : 0.8f),
                new EnhancementModule("Boom Size", 200, 1, 20, exponentialCostPercentage : 1.5f)
            };
        }
    }
}