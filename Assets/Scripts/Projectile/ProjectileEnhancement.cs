using System.Collections;
using System.Collections.Generic;
using Enhancements;
using UnityEngine;
using Entities;

namespace Projectiles.Enhancements
{
    public abstract class ProjectileEnhancement : MonoBehaviour
    {
        protected PlayerEnhancementsHandler Handler => PlayerEnhancementsHandler.Instance;
        [SerializeField] protected Projectiles.Projectile AttachedProjectile = default;
        public virtual void OnHittableCollision(ref IHittable i) { }
        public virtual void OnObstacleCollision(ref RaycastHit hit) { }
        public virtual void OnAnyCollision(ref RaycastHit hit) { }
        public virtual void OnShoot() { }
    }
}