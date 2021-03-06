﻿using System.Collections;
using FX;
using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.ComposedBehaviours
{
    public class ImpenetrableShieldBehaviour : ModularBehaviour
    {
        protected string PREFAB_PATH = PATH_TO_PREFABS_FOLDER + "Defences/Shield";
        private static GameObject _spinPrefab;
        public override ModulariuType Type => ModulariuType.Brawler;
        public override float Weight => 0.1f;
        protected ShieldFX _shieldFX;
        protected virtual float ShieldActiveTime => Random.Range(10.0f, 15.0f);

        protected Coroutine _coroutine;

        protected override void OnAwake()
        {
            if (_spinPrefab == null)
                _spinPrefab = Resources.Load<GameObject>(PREFAB_PATH);
            _shieldFX =
                Instantiate(_spinPrefab, transform.position,
                    _spinPrefab.transform.rotation,
                    BehaviourSpawnsTransform)
                .GetComponentInChildren<ShieldFX>();
            _shieldFX.transform.localPosition = Vector3.zero;

            SetShieldInvulnerable();
        }

        protected override void OnExecute()
        {
            if (_coroutine == default)
                _coroutine = StartCoroutine(CProtect());
        }

        protected virtual IEnumerator CProtect()
        {
            _shieldFX.Protect();
            Complete();
            yield return new WaitForSeconds(ShieldActiveTime);
            _shieldFX.InstaKill();
        }

        protected virtual void SetShieldInvulnerable()
        {
            _shieldFX.SetInvulnerable(true);
        }

        protected void ResetCorroutine()
        {
            if (_coroutine != default)
            {
                StopCoroutine(_coroutine);
                _coroutine = default;
            }
        }

        public override bool Condition() => !_shieldFX.Active;
    }
}