using System;
using System.Collections.Generic;
using System.Linq;
using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.BehaviourCreation
{
    public class BehaviourToggleCollection : ScriptableObject
    {
        private const string BEHAVIOURS_PATH = "Composed Behaviours";
        [SerializeField] private TypeToggle[] _typeToggles = default;
        public TypeToggle[] TypeToggles => _typeToggles;

        // Used by the custom editor (ModulariuPartProfileEditor.cs)
        public void UpdateToggles()
        {
            GetRaw();
            GetComposed();
        }

        public void Remove(string name)
        {
            List<TypeToggle> tempList = _typeToggles.ToList();
            tempList.RemoveAll(t => t.Name == name);
            _typeToggles = tempList.ToArray();
        }

        private void GetRaw()
        {
            Type[] types = ReflectionHelper.GetAllSubclasses<ModularBehaviour>();
            if (_typeToggles == null)
                _typeToggles = new TypeToggle[0];
            for (int i = 0; i < types.Length; i++)
            {
                TypeToggle newToggle = new TypeToggle(types[i].Name, false);

                bool flag = false;
                foreach (TypeToggle t in _typeToggles)
                {
                    flag = t.Equals(newToggle);
                    if (flag) break;
                }
                if (!flag)
                    AddNew(types[i]);
            }

            //_typeToggles.Sort();
        }

        private void GetComposed()
        {
            ComposedBehavior[] composedBehaviors =
                Resources.LoadAll<ComposedBehavior>(BEHAVIOURS_PATH);

            foreach (ComposedBehavior b in composedBehaviors)
            {
                bool flag = false;
                foreach (TypeToggle t in _typeToggles)
                {
                    flag = t.Name == b.Name;
                    if (flag) break;
                }
                if (!flag)
                    AddNew(b);
            }
        }

        private void AddNew(Type type)
        {
            CreateEmptyLastSlot();
            _typeToggles[_typeToggles.Length - 1] = new TypeToggle(type.Name, false);
        }
        private void AddNew(ComposedBehavior behavior)
        {
            CreateEmptyLastSlot();
            _typeToggles[_typeToggles.Length - 1] = new TypeToggle(behavior, false);
        }

        private void CreateEmptyLastSlot()
        {
            TypeToggle[] temp = (TypeToggle[]) _typeToggles.Clone();
            _typeToggles = new TypeToggle[_typeToggles.Length + 1];
            for (int i = 0; i < _typeToggles.Length - 1; i++)
                _typeToggles[i] = temp[i];
        }
    }
}