﻿using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace FX
{
    public class WhirlwindFX : Damager
    {
        public const float BASE_RADIUS = 1.8f;
        private const float BASE_DAMAGE = 5.0f;
        //
        [SerializeField] private float _dps = BASE_DAMAGE;
        [SerializeField, Range(0f, 1f)] private float _damageSplitMiliSecs = 0.1f;
        private float _dpsCountdown;
        private bool CanDealDamage => _dpsCountdown <= 0;
        public override float Damage => _damageSplitMiliSecs * _dps;

        private bool _emitting;
        //

        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            _dpsCountdown -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (!_emitting || !CanDealDamage) return;
            Collider[] within =
                Physics.OverlapSphere(transform.position, BASE_RADIUS, _hurtableMask);
            foreach (Collider c in within)
            {
                IHittable hittable;
                if (c.TryGetComponent<IHittable>(out hittable))
                    DealDamage(hittable);
            }
        }

        protected override void OnDealDamage(IHittable hitEntity)
        {
            _dpsCountdown = _damageSplitMiliSecs;
        }

        public void StartSpin()
        {
            gameObject.SetActive(true);
            _emitting = true;
            _particleSystem.Play();
        }
        public void EndSpin()
        {
            _emitting = false;
            _particleSystem.Stop();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, BASE_RADIUS);
        }
    }
}