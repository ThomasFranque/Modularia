using UnityEngine;

namespace Entities.Modularius.BaseBehaviours
{
    public class ProximityChecker : MonoBehaviour
    {
        // Max being the closest border and min being the furthest
        public bool Check(Transform target, float maxDistance = float.PositiveInfinity, float minDistance = 0)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            return distance >= minDistance && distance <= maxDistance;
        }

        public bool CheckSight(Transform target, float maxDistance = float.PositiveInfinity,float minDistance = 0)
        {
            if (!Check(target, maxDistance, minDistance)) return false;
            return Physics.Linecast(transform.position, target.position);
        }
    }
}