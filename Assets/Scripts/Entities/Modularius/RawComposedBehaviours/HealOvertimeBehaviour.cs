using System.Collections;
using FX;
using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.ComposedBehaviours
{
    public class HealOvertimeBehaviour : ModularBehaviour
    {
        private const string PREFAB_PATH = PATH_TO_PREFABS_FOLDER + "Defences/Heal";
        private static GameObject _healPrefab;
        public override ModulariuType Type => ModulariuType.Brawler;
        private HealFX _healFX;
        public override float Weight => 0.2f;
        private Coroutine _coroutine;

        protected override void OnAwake()
        {
            if (_healPrefab == null)
                _healPrefab = Resources.Load<GameObject>(PREFAB_PATH);
            _healFX =
                Instantiate(_healPrefab, transform.position,
                    _healPrefab.transform.rotation,
                    BehaviourSpawnsTransform)
                .GetComponent<HealFX>();
            enabled = false;
        }

        private void Update()
        {

        }

        protected override void OnExecute()
        {
            enabled = true;
            _coroutine = StartCoroutine(CHeal());
        }

        private IEnumerator CHeal()
        {
            LookAtPlayer.StartLooking(Player.transform, FollowType.Lerp, 3f);
            _healFX.StartHeal(AttachedEntity, OnOverheal);
            yield return new WaitForSeconds(Random.Range(4.0f, 8.0f));
            _healFX.EndHeal();
            Complete();
            enabled = false;
        }

        private void OnOverheal()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _healFX.EndHeal();
            Complete();
            enabled = false;
        }

        public override bool Condition() => AttachedEntity.Damaged && !_healFX.Healing;
    }
}