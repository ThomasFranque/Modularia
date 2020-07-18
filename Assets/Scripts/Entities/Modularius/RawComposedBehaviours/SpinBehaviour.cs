using System.Collections;
using FX;
using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.ComposedBehaviours
{
    public class SpinBehaviour : ModularBehaviour
    {
        private const string PREFAB_PATH = PATH_TO_PREFABS_FOLDER + "Attacks/Spin";
        private static GameObject _spinPrefab;
        public override ModulariuType Type => ModulariuType.Brawler;
        private SpinFX _spinFX;
        public override float Weight => 0.8f;
        private WaitForSeconds _delayAfterSpin;

        protected override void OnAwake()
        {
            if (_spinPrefab == null)
                _spinPrefab = Resources.Load<GameObject>(PREFAB_PATH);
            _spinFX =
                Instantiate(_spinPrefab, transform.position,
                    _spinPrefab.transform.rotation,
                    BehaviourSpawnsTransform)
                .GetComponentInChildren<SpinFX>();

            _delayAfterSpin = new WaitForSeconds(0.5f);
        }

        protected override void OnExecute()
        {
            StartCoroutine(CSpin());
        }

        private IEnumerator CSpin()
        {
            _spinFX.DewIt();
            yield return _delayAfterSpin;
            Complete();
        }
    }
}