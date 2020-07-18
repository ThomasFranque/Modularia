using System;
using Entities.Modularius.BehaviourCreation;
using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius
{
    [CreateAssetMenu(menuName = "Modularius/Part Profile")]
    public class ModulariuPartProfile : BehaviourToggleCollection
    {
        [SerializeField] private string _name = default;
        [SerializeField] private ModulariuType _type = default;
        [SerializeField, Range(.0f, 1f)] private float _influence = 0.5f;

        public string Name => _name;
        public ModulariuType Type => _type;
        public float Influence => _influence;
    }
}