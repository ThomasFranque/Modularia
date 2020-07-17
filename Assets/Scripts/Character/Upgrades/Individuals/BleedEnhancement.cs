using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enhancements
{
    public class BleedEnhancement : Enhancement
    { 
        public BleedEnhancement() : base()
        {
            Icon = Icons.Bleed;
            AllModules = new EnhancementModule[2]
            {
                new EnhancementModule("Blood Intensity", 30, 0.1f, exponentialCostPercentage : 0.2f),
                new EnhancementModule("Blood Amount", 30, 0.5f, 50, exponentialCostPercentage : 0.8f)
            };
        }
    }
}