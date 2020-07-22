using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public struct HeuristicValue
    {
        float Alpha { get; }
        float Beta { get; }
        float Total { get; }

        public HeuristicValue(float alpha, float beta)
        {
            Alpha = alpha;
            Beta = beta;
            Total = alpha + beta;
        }
    }
}