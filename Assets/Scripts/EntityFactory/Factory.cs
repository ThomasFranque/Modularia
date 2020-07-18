using System;
using Entities.Modularius.BehaviourCreation;
using Entities.Modularius;
using UnityEngine;

namespace EntityFactory
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] private ComposedBehavior _composedBehaviour = default;
        [SerializeField] private ModulariuPartProfile _partProfile = default;

        private void Awake()
        {
            Type[] types = _composedBehaviour.GetAllChildTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Debug.Log(types[i].Name);
            }
        }
    }
}