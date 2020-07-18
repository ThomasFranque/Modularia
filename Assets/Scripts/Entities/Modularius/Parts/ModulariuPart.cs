using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.Parts
{
    public abstract class ModulariuPart : Entity
    {
        [SerializeField] private ModulariuPartProfile _profile = default;
        public ModulariuPartProfile Profile => _profile;
    }
}