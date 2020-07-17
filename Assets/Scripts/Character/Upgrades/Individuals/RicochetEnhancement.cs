using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enhancements
{
    public class RicochetEnhancement : Enhancement
    {
        public RicochetEnhancement() : base()
        {
            Icon = Icons.Ricochet;
            AllModules = new EnhancementModule[1]
            {
                new EnhancementModule("Max Zwooshes", 100, 1, exponentialCostPercentage : 1f)
            };
        }
    }
}