using System.Linq;
using Entities;
using Entities.Modularius;
using Entities.Modularius.Parts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EntityFactory
{
    public static class Factory
    {
        private const string PREFAB_PARTS_PATH = "Prefabs/Modularius Parts/";
        private const string LIMBS_PATH = PREFAB_PARTS_PATH + "Limbs/";
        private const string CORES_PATH = PREFAB_PARTS_PATH + "Cores/";
        private const string PARTS_PROFILE_PATH = "Modulariu Parts/";
        private const int MAX_LIMBS = 3;
        private const float BASE_RARE_PART_CHANCE = 0.25f;
        private const float BASE_NEW_PART_CHANCE = .3f;
        private static GameObject _ShooterLimb;
        private static GameObject _BrawlerLimb;
        private static GameObject _TankLimb;
        private static GameObject _ShooterCore;
        private static GameObject _BrawlerCore;
        private static GameObject _TankCore;
        private static ModulariuPartProfile[] _CommonPartProfiles;
        private static ModulariuPartProfile[] _RarePartProfiles;

        // To be changed to be dynamic using the time spent on the level
        private static float DifficultyMod => 1f;
        private static float RarePartChance => BASE_RARE_PART_CHANCE * (DifficultyMod / 1.5f);
        private static float NewLimbChance => BASE_NEW_PART_CHANCE * (DifficultyMod);

        private static void Load()
        {
            _ShooterLimb = LoadPrefab(LIMBS_PATH + "Shooter");
            _BrawlerLimb = LoadPrefab(LIMBS_PATH + "Brawler");
            _TankLimb = LoadPrefab(LIMBS_PATH + "Tank");

            _ShooterCore = LoadPrefab(CORES_PATH + "Shooter");
            _BrawlerCore = LoadPrefab(CORES_PATH + "Brawler");
            _TankCore = LoadPrefab(CORES_PATH + "Tank");

            _CommonPartProfiles =
                Resources.LoadAll<ModulariuPartProfile>(PARTS_PROFILE_PATH +
                    "Common/");
            _RarePartProfiles =
                Resources.LoadAll<ModulariuPartProfile>(PARTS_PROFILE_PATH +
                    "Rare/");
        }

        private static GameObject LoadPrefab(string path) =>
            Resources.Load<GameObject>(path);

        static Factory()
        {
            Load();
        }

        /// <summary>
        /// Generates a new Modulariu
        /// </summary>
        /// <returns>The main core</returns>
        public static Core GenerateNew()
        {
            (Core core, ModulariuType type) coreAndType = GenerateNewCore();
            (Limb limb, ModulariuType type) [] limbsAndType =
                new(Limb limb, ModulariuType type) [MAX_LIMBS];

            Limb[] limbs = new Limb[MAX_LIMBS];

            for (int i = 0; i < MAX_LIMBS; i++)
            {
                float roll = Random.Range(.0f, 1.0f);
                if (roll <= NewLimbChance)
                {
                    limbsAndType[i] = GenerateNewLimb();
                    limbs[i] = limbsAndType[i].limb;
                }
            }

            limbs = limbs.Where(l => l != null).ToArray();

            for (int i = 0; i < limbsAndType.Length; i++)
            {
                (Limb limb, ModulariuType type) lt = limbsAndType[i];
                if (lt.limb == null) break;

                ModifierStats lMods = ModifierStats
                    .GetRandomized(DifficultyMod, lt.type);
                lt.limb
                    .FactorySetup(lt.type, lMods, coreAndType.core, limbs);
            }

            ModifierStats mods = ModifierStats
                .GetRandomized(DifficultyMod, coreAndType.type);
            coreAndType.core
                .FactorySetup(coreAndType.type, mods, coreAndType.core, limbs);

            TreeFactory.GenerateTree(coreAndType.core);
            return coreAndType.core;
        }

        /// <summary>
        /// Generates a new individual core
        /// </summary>
        /// <returns>Generated core</returns>
        private static(Core, ModulariuType) GenerateNewCore()
        {
            ModulariuType coreType = GetRandomType();
            GameObject coreObject =
                GetPrefabFromType(coreType, _ShooterCore, _BrawlerCore, _TankCore);
            Core core = GameObject.Instantiate(coreObject).AddComponent<Core>();
            return (core, coreType);
        }

        /// <summary>
        /// Generates an individual limb
        /// </summary>
        /// <returns>Generated limb</returns>
        private static(Limb, ModulariuType) GenerateNewLimb()
        {
            ModulariuPartProfile profile = GetRandomProfile();
            ModifierStats stats = ModifierStats.GetRandomized(DifficultyMod, profile.Type);
            ModulariuType limbType = profile.Type;
            GameObject limbObject =
                GetPrefabFromType(limbType, _ShooterLimb, _BrawlerLimb, _TankLimb);
            Limb limb = GameObject.Instantiate(limbObject).AddComponent<Limb>();
            limb.SetProfile(profile);
            return (limb, limbType);
        }

        /// <summary>
        /// Gets a random profile, taking int account common and rare chances
        /// </summary>
        /// <returns>Random profile</returns>
        private static ModulariuPartProfile GetRandomProfile()
        {
            float roll = Random.Range(.0f, 1.0f);
            if (roll <= RarePartChance)
                return _RarePartProfiles[Random.Range(
                    0,
                    _RarePartProfiles.Length)];
            else
                return _CommonPartProfiles[Random.Range(
                    0,
                    _CommonPartProfiles.Length)];
        }

        /// <summary>
        /// Gets a random modulariu type
        /// </summary>
        /// <returns>Random type</returns>
        private static ModulariuType GetRandomType() =>
            (ModulariuType) Random.Range(0, 3);

        /// <summary>
        /// Chooses the respective prefab based on the given type
        /// </summary>
        /// <param name="type">Selected type</param>
        /// <param name="shooter">Target shooter prefab</param>
        /// <param name="brawler">Target shooter prefab</param>
        /// <param name="tank">Target shooter prefab</param>
        /// <returns>One of the given prefabs corresponding 
        /// to the given type</returns>
        private static GameObject GetPrefabFromType(ModulariuType type,
            GameObject shooter, GameObject brawler, GameObject tank)
        {
            switch (type)
            {
                case ModulariuType.Shooter:
                    return shooter;
                case ModulariuType.Brawler:
                    return brawler;
                case ModulariuType.Tank:
                    return tank;
            }
            return default;
        }
    }
}