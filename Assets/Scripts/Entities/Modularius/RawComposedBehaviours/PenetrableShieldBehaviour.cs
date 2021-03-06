﻿using System.Collections;
using FX;
using ModulariaBehaviourTree;
using UnityEngine;

namespace Entities.Modularius.ComposedBehaviours
{
    public class PenetrableShieldBehaviour : ImpenetrableShieldBehaviour
    {
        protected override float ShieldActiveTime => Random.Range(5.0f, 15.0f);
        protected override void SetShieldInvulnerable() { }

        protected override IEnumerator CProtect()
        {
            _shieldFX.Protect(OnPrematureKill);
            Complete();
            yield return new WaitForSeconds(ShieldActiveTime);
            _shieldFX.InstaKill();
        }

        private void OnPrematureKill()
        {
            ResetCorroutine();
        }

    }
}