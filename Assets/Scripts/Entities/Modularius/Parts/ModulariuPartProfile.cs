using System;
using ModulariaBehaviourTree;
using UnityEngine;
using Entities.Modularius.BehaviourCreation;

namespace Entities.Modularius
{
    [CreateAssetMenu(menuName = "Modularius/Part Profile")]
    public class ModulariuPartProfile : BehaviourToggleCollection
    {
        [SerializeField] private ModulariuType _type = default;
        [SerializeField, Range(.0f, 1f)] private float _influence = 0.5f;

        public ModulariuType Type => _type;
        public float Influence => _influence;
    }
}