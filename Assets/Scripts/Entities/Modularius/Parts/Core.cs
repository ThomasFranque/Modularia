using System.Collections.Generic;
using Projectiles;
using UnityEngine;

namespace Entities.Modularius.Parts
{
    public class Core : ModulariuPart
    {
        [SerializeField] private Limb[] _limbs = default;
        private float _limbOffset;
        public bool HasShooterLimb { get; private set; }
        public bool HasBrawlerLimb { get; private set; }
        public bool HasTankLimb { get; private set; }
        public int ShooterLimbs { get; private set; }
        public int BrawlerLimbs { get; private set; }
        public int TankLimbs { get; private set; }
        public Limb[] Limbs => _limbs;

        public override void FactorySetup(ModulariuType type,
            ModifierStats stats, Core core, Limb[] limbs)
        {
            base.FactorySetup(type, stats, core, limbs);
            _limbs = limbs;
            for (int i = 0; i < _limbs.Length; i++)
                AppendLimb(_limbs[i]);
        }

        private void AppendLimb(Limb l)
        {
            for (int i = 0; i < _limbs.Length; i++)
            {
                ModulariuType limbType = _limbs[i].Type;

                switch (limbType)
                {
                    case ModulariuType.Shooter:
                        HasShooterLimb = true;
                        ShooterLimbs++;
                        break;
                    case ModulariuType.Brawler:
                        HasBrawlerLimb = true;
                        BrawlerLimbs++;
                        break;
                    case ModulariuType.Tank:
                        HasTankLimb = true;
                        TankLimbs++;
                        break;
                }
            }
            l.transform.SetParent(transform);
            Vector3 limbPos = Vector3.zero;
            _limbOffset += 1;
            limbPos.y += _limbOffset;
            l.transform.localPosition = limbPos;
        }

        protected override void OnHit(RaycastHit hit,
            Projectile projectile, float damage)
        {
            DealDamage(damage);
            DamageLimbs(damage);
        }
        protected override void OnHit(float damage)
        {
            DealDamage(damage);
            DamageLimbs(damage);
        }

        private void DamageLimbs(float receivedDmg)
        {
            float dividedLimbDamage = (receivedDmg / 4) / _limbs.Length;

            for (int i = 0; i < _limbs.Length; i++)
                _limbs[i].DealIndividualDamage(dividedLimbDamage);
        }

        protected override void OnTreeFinished()
        {
            for (int i = 1; i < _limbs.Length; i++)
                UpgradeLimbs();
        }

        private void UpgradeLimbs()
        {
            foreach (Limb l in _limbs)
                l.ModifierStats.Upgrade(Type);
        }

        public IEnumerable<ModulariuPartProfile> GetLimbProfiles()
        {
            foreach (Limb l in _limbs)
                yield return l.Profile;
        }
    }
}