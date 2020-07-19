using System.Collections;
using FX;
using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.ComposedBehaviours
{
    public class DeathRayBehaviour : ModularBehaviour
    {
        private const string PATH_TO_PREFAB = PATH_TO_PREFABS_FOLDER + "Attacks/Death Ray";

        public const float CHARGE_UP_TIME = 3f;
        private static GameObject _rayPrefab;
        public override ModulariuType Type => ModulariuType.Shooter;
        public override float Weight => 0.1f;
        private DeathRay _ray;
        private WaitForSeconds _delayAfterCompletion;
        private WaitForSeconds _chargeUp;

        protected override void OnAwake()
        {
            if (_rayPrefab == null)
                _rayPrefab = Resources.Load<GameObject>(PATH_TO_PREFAB);

            _ray =
                Instantiate(_rayPrefab, transform.position,
                    transform.rotation,
                    BehaviourSpawnsTransform)
                .GetComponentInChildren<DeathRay>();

            _delayAfterCompletion = new WaitForSeconds(1.0f);
            _chargeUp = new WaitForSeconds(CHARGE_UP_TIME);
        }

        protected override void OnExecute()
        {
            StartCoroutine(CShootBeam());
        }

        private IEnumerator CShootBeam()
        {
            LookAtPlayer.StartLooking(Player.transform, FollowType.Lerp, 2f);
            Follow.StartFollowing(Player.transform, FollowType.Linear, 2);
            yield return _chargeUp;
            LookAtPlayer.StartLooking(Player.transform, FollowType.Linear, 0.25f);
            Follow.Speed /= 2;
            _ray.StartEmitting();

            yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));
            _ray.StopEmitting();
            yield return _delayAfterCompletion;
            Complete();
        }

        public override bool Condition() => Proximity.CheckSight(Player.transform, 32);
    }
}