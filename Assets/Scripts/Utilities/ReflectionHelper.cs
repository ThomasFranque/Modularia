using System;
using System.Linq;
using System.Reflection;

public static class ReflectionHelper
{
    private const string BEHAVIOUR_NAMESPACE_PATH = "Entities.Modularius.ComposedBehaviours.";
    // from http://www.blackwasp.co.uk/FindAllSubclasses.aspx
    public static Type[] GetAllSubclasses<T>()
    {
        Type parentType = typeof(T);
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();
        Type[] subclasses =
            types.Where(t => t.IsSubclassOf(parentType) && !t.IsAbstract).ToArray();

        return subclasses;
    }
    public static Type GetBehaviourType(string behaviourClassName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type t = assembly.GetType(BEHAVIOUR_NAMESPACE_PATH + behaviourClassName);
        return t;
    }
}