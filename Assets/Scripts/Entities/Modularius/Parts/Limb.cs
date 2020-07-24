using Projectiles;
using UnityEngine;

namespace Entities.Modularius.Parts
{
    public class Limb : ModulariuPart
    {
        private Core _core;
        private ModulariuPartProfile _profile;
        public ModulariuPartProfile Profile => _profile;

        public override void FactorySetup(ModulariuType type,
            ModifierStats stats,
            Core core, Limb[] limbs)
        {
            base.FactorySetup(type, stats, core, limbs);
            _core = core;
        }

        public void SetProfile(ModulariuPartProfile profile)
        {
            _profile = profile;
        }

        protected override void OnHit(RaycastHit hit,
            Projectile projectile, float damage)
        {
            base.OnHit(hit, projectile, damage);
            DamageCore(damage);
        }
        protected override void OnHit(float damage)
        {
            base.OnHit(damage);
            DamageCore(damage);
        }

        private void DamageCore(float receivedDmg)
        {
            // it has a ? because of the multishot upgrade, shots can hit at the
            // same time and the core die before the other ones have a chance
            // to deal damage, same goes for explosions
            _core?.DealIndividualDamage(receivedDmg / 4);
        }
    }
}