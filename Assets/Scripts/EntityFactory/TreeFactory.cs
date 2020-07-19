using System;
using System.Collections.Generic;
using Entities;
using Entities.Modularius;
using Entities.Modularius.BehaviourCreation;
using Entities.Modularius.Parts;
using ModulariaBehaviourTree;
using UnityEngine;

namespace EntityFactory
{
    public static class TreeFactory
    {
        private const string COMPOSED_PATH = "Composed Behaviours/";
        private const string BASICS_PATH = COMPOSED_PATH + "Type Basics/";
        private const string DEFAULTS_PATH = COMPOSED_PATH + "Type Selectors/";
        private const string SPECIALS_PATH = COMPOSED_PATH + "Type Specials/";

        private static ComposedBehavior _shooterBasic;
        private static ComposedBehavior _brawlerBasic;
        private static ComposedBehavior _tankBasic;

        private static ComposedBehavior _shooterDefault;
        private static ComposedBehavior _brawlerDefault;
        private static ComposedBehavior _tankDefault;

        private static ComposedBehavior _shooterSpecial;
        private static ComposedBehavior _brawlerSpecial;
        private static ComposedBehavior _tankSpecial;

        static TreeFactory()
        {
            _shooterBasic =
                GetComposedBehaviour(BASICS_PATH + "ShooterBasics");
            _brawlerBasic =
                GetComposedBehaviour(BASICS_PATH + "BrawlerBasics");
            _tankBasic =
                GetComposedBehaviour(BASICS_PATH + "TankBasics");

            _shooterDefault =
                GetComposedBehaviour(DEFAULTS_PATH + "ShooterSelector");
            _brawlerDefault =
                GetComposedBehaviour(DEFAULTS_PATH + "BrawlerSelector");
            _tankDefault =
                GetComposedBehaviour(DEFAULTS_PATH + "TankSelector");

            _shooterSpecial =
                GetComposedBehaviour(SPECIALS_PATH + "ShieldedDeathRay");
            _brawlerSpecial =
                GetComposedBehaviour(SPECIALS_PATH + "ShieldedWhirlwind");
            _tankSpecial =
                GetComposedBehaviour(SPECIALS_PATH + "PenetrableHealingZone");
        }

        private static ComposedBehavior GetComposedBehaviour(string path)
        {
            return Resources.Load<ComposedBehavior>(path);
        }

        public static void GenerateTree(Core core)
        {
            BehaviourTree tree;
            List<Type> addedTypes;
            List<ComposedBehavior> composedBehaviors;

            tree = core.gameObject.AddComponent<BehaviourTree>();
            addedTypes = new List<Type>();
            composedBehaviors = new List<ComposedBehavior>();

            // Go through all the parts
            foreach (ModulariuPartProfile p in core.GetLimbProfiles())
            {
                // Gather the required types
                for (int i = 0; i < p.TypeToggles.Length; i++)
                {
                    TypeToggle toggle = p.TypeToggles[i];
                    Type[] profileTypes = toggle.GetAllChildTypes();

                    // Go through all the types
                    for (int j = 0; j < profileTypes.Length; j++)
                    {
                        Type t = profileTypes[j];
                        // Add it if it was not already added
                        if (!addedTypes.Contains(t))
                        {
                            addedTypes.Add(t);
                            AddTypeToCore(t, core);
                        }

                    }

                    if (!toggle.IsRaw)
                    {
                        if (!composedBehaviors.Contains(toggle.ComposedBehavior))
                            composedBehaviors.Add(toggle.ComposedBehavior);
                    }
                }
            }

            TreeSelector _shooterSelector = new TreeSelector(true);
            TreeSelector _brawlerSelector = new TreeSelector(true);
            TreeSelector _tankSelector = new TreeSelector(true);
            TreeSelector _mainSelector = new TreeSelector(true,
                _shooterSelector, _brawlerSelector, _tankSelector);

            foreach (ComposedBehavior c in composedBehaviors)
            {
                ITreeComponent component;
                if (c.ComposedType == TreeComponentType.Selector)
                    component = CreateSelectorFromComposed(core, c);
                else
                    component = CreateSequenceFromComposed(core, c);

                switch (c.BehaviourType)
                {
                    case ModulariuType.Shooter:
                        _shooterSelector.AddNewOption(component);
                        break;
                    case ModulariuType.Brawler:
                        _brawlerSelector.AddNewOption(component);
                        break;
                    case ModulariuType.Tank:
                        _tankSelector.AddNewOption(component);
                        break;
                }
            }

            tree.Initialize(_mainSelector);
        }

        private static void AddTypeToCore(Type t, Core core)
        {
            core.gameObject.AddComponent(t);
        }

        private static TreeSequence CreateSequenceFromComposed(Core core,
            ComposedBehavior target)
        {
            TreeSequence sequence;
            sequence = new TreeSequence();

            foreach (Type t in GetAllTypesIn(target))
            {
                sequence.AddNew(GetModularBehaviourInCore(core, t));
            }

            return sequence;
        }

        private static TreeSelector CreateSelectorFromComposed(Core core,
            ComposedBehavior target)
        {
            TreeSelector selector;
            List<ModularBehaviour> modularBehaviours;

            modularBehaviours = new List<ModularBehaviour>();
            selector = new TreeSelector(true);

            if (target.IsRandomSelector)
            {
                foreach (Type t in GetAllTypesIn(target))
                {
                    selector.AddNewOption(GetModularBehaviourInCore(core, t));
                }
            }
            else
            {
                foreach (Type t in GetAllTypesIn(target))
                {
                    ModularBehaviour behaviour = GetModularBehaviourInCore(core, t);
                    selector.AddNewOption((behaviour.Condition, behaviour));
                }
            }

            return selector;
        }

        private static IEnumerable<Type> GetAllTypesIn(ComposedBehavior composed)
        {
            List<Type> collection = new List<Type>();

            for (int i = 0; i < composed.TypeToggles.Length; i++)
            {
                TypeToggle toggle = composed.TypeToggles[i];
                Type[] profileTypes = toggle.GetAllChildTypes();

                // Go through all the types
                for (int j = 0; j < profileTypes.Length; j++)
                {
                    Type t = profileTypes[j];
                    // Add it if it was not already added
                    if (!collection.Contains(t))
                    {
                        collection.Add(t);
                        yield return t;
                    }
                }
            }
        }

        private static ModularBehaviour GetModularBehaviourInCore(Core core, Type type)
        {
            ModularBehaviour b =
                core.gameObject.GetComponent(type) as ModularBehaviour;
            return b;
        }
    }
}