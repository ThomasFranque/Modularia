using System;
using Entities.Modularius.BehaviourCreation;
using UnityEngine;

[System.Serializable]
public class TypeToggle
{
    [SerializeField, ReadOnly] private string _name;
    [SerializeField, ReadOnly] private bool _toggle;
    [SerializeField, ReadOnly] private bool _isRaw;
    [SerializeField, ReadOnly] private ComposedBehavior _composedBehavior;

    public string Name => _name;
    public bool Toggle => _toggle;
    public bool IsRaw => _isRaw;
    public ComposedBehavior ComposedBehavior => _composedBehavior;

    public TypeToggle(string name, bool toggle)
    {
        _name = name;
        _toggle = toggle;
        _composedBehavior = default;
        _isRaw = true;
    }

    public TypeToggle(ComposedBehavior behavior, bool toggle)
    {
        _name = behavior.Name;
        _toggle = toggle;
        _composedBehavior = behavior;
        _isRaw = false;
    }

    public void SetToggle(bool value)
    {
        _toggle = value;
    }

    public override bool Equals(object obj)
    {
        TypeToggle objAsToggle = (TypeToggle) obj;
        if (obj == default) return false;
        return Name == objAsToggle.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public Type[] GetAllChildTypes()
    {
        Type[] _types;
        if (IsRaw) // Use reflection to get em
        {
            _types = new Type[1]
            {
                ReflectionHelper.GetBehaviourType(Name)
            };
        }
        else
        {
            if (ComposedBehavior == default)
                throw new Exception("Something is wrong with " + Name);
            _types = ComposedBehavior.GetAllChildTypes();
        }

        return _types;

        // Implement when the selection is fully functional
    }
}