using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enhancements
{
    public class MultishotEnhancement : Enhancement
    {
        public const float OFFSET = .35f;
        public MultishotEnhancement() : base()
        {
            Icon = Icons.Multishot;
            AllModules = new EnhancementModule[2]
            {
                new EnhancementModule("Multi Count", 500, 1f, 4, exponentialCostPercentage : 1f),
                new EnhancementModule("Damage", 150, 0.05f)
            };
        }
    }
}