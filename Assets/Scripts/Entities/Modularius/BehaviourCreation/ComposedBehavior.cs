using System;
using System.Collections.Generic;
using System.Linq;
using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.BehaviourCreation
{
    [CreateAssetMenu(menuName = "Modularius/Composed Behaviour", fileName = "Composed Behaviour")]
    public class ComposedBehavior : BehaviourToggleCollection
    {
        [SerializeField, HideInInspector] private TypeToggle[] _selectedBehavs = default;
        [SerializeField] private string _name = default;
        [SerializeField] private TreeComponentType _composedType = default;
        [SerializeField, HideInInspector] private bool _isRandomSelector
            = default;

        public TreeComponentType ComposedType => _composedType;
        public TypeToggle[] SelectedBehaviours => _selectedBehavs;
        public string Name => _name;
        public bool IsRandomSelector => _isRandomSelector;

        private void OnEnable()
        {
            if (_selectedBehavs == null) _selectedBehavs = new TypeToggle[0];
            _isRandomSelector = false;
        }

        public void ClearSelected()
        {
            _selectedBehavs = new TypeToggle[0];
        }

        public bool SelectionContains(TypeToggle toggle)
        {
            for (int i = 0; i < _selectedBehavs.Length; i++)
                if (toggle == _selectedBehavs[i]) return true;
            return false;
        }

        public void AddToSelectedBehaviours(TypeToggle typeToggle)
        {
            List<TypeToggle> tempCollection = _selectedBehavs.ToList();
            tempCollection.Add(typeToggle);
            _selectedBehavs = tempCollection.ToArray();
            typeToggle.GetAllChildTypes();
        }
        public void RemoveFromSelectedBehaviours(TypeToggle typeToggle)
        {
            List<TypeToggle> tempCollection = _selectedBehavs.ToList();
            tempCollection.Remove(typeToggle);
            _selectedBehavs = tempCollection.ToArray();
        }

        public Type[] GetAllChildTypes()
        {
            List<Type> tempTypes = new List<Type>();

            for (int i = 0; i < _selectedBehavs.Length; i++)
            {
                Type[] childTypes = _selectedBehavs[i].GetAllChildTypes();
                foreach (Type t in childTypes)
                    tempTypes.Add(t);
            }
            return tempTypes.ToArray();
        }
    }
}