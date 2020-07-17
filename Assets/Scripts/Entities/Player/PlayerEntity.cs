using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModulariaBehaviourTree;

namespace Entities
{
    public class PlayerEntity : Entity
    {
        protected override void OnAwake()
        {
            ReflectionHelper.GetAllSubclasses<ModularBehaviour>();
        }

        protected override void OnHit(RaycastHit hit, Projectiles.Projectile projectile, float damage)
        {
            Debug.Log("I took an arrow to the knee * " + damage);
        }

        protected override void OnHit(float damage)
        {
            Debug.Log("AAAAAAAAAAAH * " + damage);
        }
    }
}