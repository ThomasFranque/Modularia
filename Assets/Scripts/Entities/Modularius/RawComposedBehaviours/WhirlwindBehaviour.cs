using System.Collections;
using FX;
using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.ComposedBehaviours
{
    public class WhirlwindBehaviour : ModularBehaviour
    {
        private const string PREFAB_PATH = PATH_TO_PREFABS_FOLDER + "Attacks/Whirlwind";
        private static GameObject _whirlwindPrefab;
        public override ModulariuType Type => ModulariuType.Brawler;
        private WhirlwindFX _whirlwindFX;
        public override float Weight => 0.2f;
        private WaitForSeconds _delayAfterSpin;

        protected override void OnAwake()
        {
            if (_whirlwindPrefab == null)
                _whirlwindPrefab = Resources.Load<GameObject>(PREFAB_PATH);
            _whirlwindFX =
                Instantiate(_whirlwindPrefab, transform.position,
                    _whirlwindPrefab.transform.rotation,
                    transform)
                .GetComponent<WhirlwindFX>();

            _delayAfterSpin = new WaitForSeconds(1.0f);
        }

        protected override void OnExecute()
        {
            StartCoroutine(CWhirlind());
        }

        private IEnumerator CWhirlind()
        {
            Follow.StartFollowing(Player.transform, FollowType.Lerp, 1.3f);
            LookAtPlayer.StartLooking(Player.transform, FollowType.Lerp, 3f);
            _whirlwindFX.StartSpin();
            yield return new WaitForSeconds (Random.Range(4.0f, 8.0f));
            _whirlwindFX.EndSpin();
            Follow.StopFollowing();
            yield return _delayAfterSpin;
            Complete();
        }

        public override bool Condition() => Proximity.Check(Player.transform, WhirlwindFX.BASE_RADIUS * 4.5f, WhirlwindFX.BASE_RADIUS * 0.3f);
    }
}