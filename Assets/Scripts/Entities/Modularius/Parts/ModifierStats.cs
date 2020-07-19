using Entities.Modularius;
using Entities.Modularius.Parts;
using UnityEngine;

namespace Entities
{
    [System.Serializable]
    public struct ModifierStats
    {
        private const float TYPE_MODIFIER = 0.5f;
        [SerializeField, ReadOnly] private float _damage;
        [SerializeField, ReadOnly] private float _speed;
        [SerializeField, ReadOnly] private float _defense;

        public float Dmg => _damage;
        public float Spd => _speed;
        public float Def => _defense;

        private ModifierStats(float damage, float speed, float defense)
        {
            _damage = damage;
            _speed = speed;
            _defense = defense;
        }

        public void Upgrade(ModulariuType coreType) =>
            MultiplyStatsByType(coreType, ref _damage, ref _speed, ref _defense);

        public static ModifierStats GetRandomized(float difficultyScale,
            ModulariuType type)
        {
            float dmg = Random.Range(0.0f, .6f) * difficultyScale;
            float spd = Random.Range(0.0f, .4f) * difficultyScale;
            float def = Random.Range(0.0f, .2f) * difficultyScale;

            MultiplyStatsByType(type, ref dmg, ref spd, ref def);

            return new ModifierStats(dmg, spd, def);
        }

        private static void MultiplyStatsByType(ModulariuType type,
            ref float damage, ref float speed, ref float defense)
        {
            float positiveMod = Random.Range(1.0f, 2.0f);
            float negativeMod = Random.Range(1.0f, 2.0f);
            switch (type)
            {
                case ModulariuType.Shooter:
                    damage *= positiveMod;
                    defense /= negativeMod;
                    break;
                case ModulariuType.Brawler:
                    speed *= positiveMod;
                    damage *= positiveMod / 2;
                    break;
                case ModulariuType.Tank:
                    defense *= positiveMod;
                    defense = Mathf.Clamp(defense, 0, 0.95f);
                    speed /= negativeMod;
                    break;
            }
        }

        public static ModifierStats PlayerDefaults =>
            new ModifierStats(1, 1, 1);
    }
}