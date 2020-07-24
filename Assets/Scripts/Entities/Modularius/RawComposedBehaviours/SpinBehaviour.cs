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
        public override float Weight => 0.9f;
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
            _spinFX.transform.localPosition = Vector3.zero;

            _delayAfterSpin = new WaitForSeconds(0.5f);
        }

        protected override void OnExecute()
        {
            StartCoroutine(CSpin());
        }

        private IEnumerator CSpin()
        {
            Follow.StartFollowing(Player.transform, FollowType.Linear, AttachedEntity.CurrentRoom.RoomGrid);
            yield return new WaitUntil(() => HitCondition());
            _spinFX.DewIt();
            yield return _delayAfterSpin;
            Complete();
        }

        private bool HitCondition()
        {
            return Proximity.Check(Player.transform, SpinFX.BASE_RADIUS * 1.3f);
        }

        public override bool Condition() =>
            Proximity.Check(Player.transform, SpinFX.BASE_RADIUS * 10);

    }
}