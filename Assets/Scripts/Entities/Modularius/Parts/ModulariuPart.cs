using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.Parts
{
    public abstract class ModulariuPart : Entity
    {
        [SerializeField, ReadOnly] private ModulariuType _type;

        public ModulariuType Type
        {
            get => _type;
            protected set => _type = value;
        }

        public virtual void FactorySetup(ModulariuType type, ModifierStats stats,
            Core core, Limb[] limbs)
        {
            Type = type;
            ModifierStats = stats;
        }

        public void DealIndividualDamage(float damage)
        {
            DealDamage(damage);
        }

        public void TreeCreationFinished()
        {
            IHurt[] iHurts =
                GetComponentsInChildren<IHurt>();

            foreach(IHurt h in iHurts)
                h.DamageModifier = ModifierStats.Dmg;
        }

        protected virtual void OnTreeFinished()
        {

        }
    }
}