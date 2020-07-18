using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Projectiles;

namespace Entities.Modularius.Parts
{
    public class Core : ModulariuPart
    {
        [SerializeField] private Limb[] _limbs = default;
        public void INIT()
        {
            _limbs = GetComponentsInChildren<Limb>();
            for (int i = 0; i < _limbs.Length; i++)
                _limbs[i].INIT(this);
        }

        protected override void OnHit(RaycastHit hit, Projectile projectile, float damage)
        {

        }
    }
}