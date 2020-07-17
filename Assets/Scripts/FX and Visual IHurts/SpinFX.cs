using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace FX
{
    public class SpinFX : Damager
    {
        private const float BASE_DAMAGE = 2.0f;
        private const float BASE_RADIUS = 1.3f;
        private ParticleSystem _spin;
        public override float Damage => BASE_DAMAGE;
        private void Awake()
        {
            _spin = GetComponent<ParticleSystem>();
        }

        public void DewIt()
        {
            _spin.Play();
            Collider[] within =
                Physics.OverlapSphere(transform.position, BASE_RADIUS, _hurtableMask);
            foreach (Collider c in within)
            {
                IHittable hittable;
                if (c.TryGetComponent<IHittable>(out hittable))
                    DealDamage(hittable);
            }
        }

        protected override void OnDealDamage(IHittable hitEntity)
        {
            // cowabunga
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, BASE_RADIUS);
        }
    }
}