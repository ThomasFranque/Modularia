using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.Parts
{
    public abstract class ModulariuPart : Entity
    {
        [SerializeField] private ModulariuPartProfile _profile = default;

        public abstract float Influence { get; protected set; }
        public ModulariuPartProfile Profile => _profile;
    }
}