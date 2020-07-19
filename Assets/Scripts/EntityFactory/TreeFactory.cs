using System;
using System.Collections.Generic;
using Entities;
using Entities.Modularius;
using Entities.Modularius.BehaviourCreation;
using Entities.Modularius.ComposedBehaviours;
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

        /// <summary>
        /// Will generate a behaviour tree to the given core using the limbs
        /// and core's behaviours
        /// </summary>
        /// <param name="core">Target core</param>
        public static void GenerateTree(Core core)
        {
            /*
            The tree works as follows:

            [X] - Random Selector
            [>] - Sequential Selector

                          [>]Main Selector          --> Layer 0
                           /            \
                     [X]Attack          Idle        --> Layer 1
                       Selector         Leaf
                  /      |     \
        [X]Shooter  [X]Brawler  [X]Tank             --> Layer 2
           Selector    Selector    Selector
              |          |         |
             ...        ...       ...               --> Layer 3...

            - All the behaviours and composed behaviours will be added to the 
            respective selector.

            - If a core has all limbs of its type it will be considered an elite
            and add the type special composed behaviour.

            - The Layer 2 Selector chances will be determined by taking into
            account the core type and limbs, the core having an weight of 0.8
            while limbs have 0.2 (Do not mix weights with influences. Influences
            were supposed to be the stats influence on the core, having nothing
            to do with the tree).
            */

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

                    // Is it selected?
                    if (!toggle.Toggle) continue;

                    Debug.Log("Adding: " + toggle.Name);

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

            //TODO: Put bellow in another method

            float shooterChance;
            float brawlerChance;
            float tankChance;

            TreeSelector shooterSelector;
            TreeSelector brawlerSelector;
            TreeSelector tankSelector;

            TreeSelector attackSelector;
            ITreeComponent idleBehaviour;

            TreeSelector mainSelector;

            idleBehaviour = core.gameObject.AddComponent<IdleBehaviour>();

            // Get the chances of each selector
            shooterChance =
                core.IsOfType(ModulariuType.Shooter) ?
                0.8f :
                0.1f;
            brawlerChance =
                core.IsOfType(ModulariuType.Brawler) ?
                0.8f :
                0.1f;
            tankChance =
                core.IsOfType(ModulariuType.Tank) ?
                0.8f :
                0.1f;

            if (core.HasShooterLimb)
                shooterChance += 0.2f * core.ShooterLimbs;
            if (core.HasBrawlerLimb)
                brawlerChance += 0.2f * core.BrawlerLimbs;
            if (core.HasTankLimb)
                tankChance += 0.2f * core.TankLimbs;

            // Initialize selectors
            shooterSelector = new TreeSelector(true, shooterChance);
            brawlerSelector = new TreeSelector(true, brawlerChance);
            tankSelector = new TreeSelector(true, tankChance);
            attackSelector = new TreeSelector(true, 1,
                shooterSelector, brawlerSelector, tankSelector);
            mainSelector = new TreeSelector(false, 1, attackSelector, idleBehaviour);

            // Set defaults and determine special abilities
            //Is Shooter?
            if (core.IsOfType(ModulariuType.Shooter))
            {
                // Is elite shooter
                if (core.ShooterLimbs > 2)
                    shooterSelector
                    .AddNewOption(
                        CreateITreeComponentFromComposed(
                            core, _shooterSpecial));
                shooterSelector
                    .AddNewOption(
                        CreateITreeComponentFromComposed(
                            core, _shooterDefault));
            }
            // Is Brawler?
            else if (core.IsOfType(ModulariuType.Brawler))
            {
                // Is elite brawler
                if (core.BrawlerLimbs > 2)
                    brawlerSelector
                    .AddNewOption(
                        CreateITreeComponentFromComposed(
                            core, _brawlerSpecial));
                brawlerSelector
                    .AddNewOption(
                        CreateITreeComponentFromComposed(
                            core, _brawlerDefault));
            }
            // Is Tank?
            else if (core.IsOfType(ModulariuType.Tank))
            {
                // Is elite tank
                if (core.TankLimbs > 2)
                    tankSelector
                    .AddNewOption(
                        CreateITreeComponentFromComposed(
                            core, _tankSpecial));
                tankSelector
                    .AddNewOption(
                        CreateITreeComponentFromComposed(
                            core, _tankDefault));
            }

            // Run through limb's behaviours and add them
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
                        shooterSelector.AddNewOption(component);
                        break;
                    case ModulariuType.Brawler:
                        brawlerSelector.AddNewOption(component);
                        break;
                    case ModulariuType.Tank:
                        tankSelector.AddNewOption(component);
                        break;
                }
            }

            // Initialize the tree with the default selector
            tree.Initialize(mainSelector);
        }

        private static ITreeComponent CreateITreeComponentFromComposed(Core core,
            ComposedBehavior composed)
        {
            if (composed.ComposedType == TreeComponentType.Selector)
                return CreateSelectorFromComposed(core, composed);
            else
                return CreateSequenceFromComposed(core, composed);
        }

        private static TreeSequence CreateSequenceFromComposed(Core core,
            ComposedBehavior target)
        {
            TreeSequence sequence;
            sequence = new TreeSequence(target.Weight);

            sequence.SetName(target.Name);

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
            selector = new TreeSelector(true, target.Weight);

            if (target.IsRandomSelector)
            {
                foreach (Type t in GetAllTypesIn(target))
                {
                    Debug.Log("Adding \"" + t.Name + " from \"" + target.Name + "\"");
                    selector.AddNewOption(GetModularBehaviourInCore(core, t));
                }
            }
            else
            {
                foreach (Type t in GetAllTypesIn(target))
                {
                    ModularBehaviour behaviour = GetModularBehaviourInCore(core, t);
                    selector.AddNewOption(behaviour);
                }
            }

            return selector;
        }

        private static IEnumerable<Type> GetAllTypesIn(ComposedBehavior composed)
        {
            for (int i = 0; i < composed.SelectedBehaviours.Length; i++)
            {
                TypeToggle toggle = composed.SelectedBehaviours[i];
                Type[] profileTypes = toggle.GetAllChildTypes();

                // Go through all the types
                for (int j = 0; j < profileTypes.Length; j++)
                    yield return profileTypes[j];
            }
        }

        private static ModularBehaviour GetModularBehaviourInCore(Core core, Type type)
        {
            ModularBehaviour b =
                core.gameObject.GetComponent(type) as ModularBehaviour;
            if (b == default)
                b = AddTypeToCore(type, core) as ModularBehaviour;
            return b;
        }

        private static Component AddTypeToCore(Type t, Core core)
        {
            return core.gameObject.AddComponent(t);
        }
    }
}