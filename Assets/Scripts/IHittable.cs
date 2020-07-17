using System;
using UnityEngine;
using Projectiles;

namespace Entities
{
    public interface IHittable
    {
        Entity AttachedEntity { get; }
        Collider AttachedCollider { get; }
        void Hit(RaycastHit hit, Projectile projectile, float damage);
        void Hit(float damage);
        event Action OnDeath;        
    }
}