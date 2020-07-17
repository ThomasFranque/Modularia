using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace FX
{
    public class DeathRay : Damager
    {
        [Header("Death Parameters")]
        [SerializeField] private float _dps = 5f;
        [SerializeField, Range(0f, 1f)] private float _damageSplitMiliSecs = 0.1f;
        [Space]
        [Space]
        #region FX
        [Header("FX")]
        public float maxLength = 32f;
        public float globalProgressSpeed = 1f;
        public float globalImpactProgressSpeed = 1.5f;
        public Color finalColor;
        [Range(0.2f, 1.0f)]
        public float gammaLinear = 1f;
        public Renderer meshRend;
        public float meshRendPower = 3f;
        public Light pointLight;
        public StartPointEffectControllerV3D startPointEffect;
        public EndPointEffectControllerV3D endPointEffect;
        public SmartWaveParticlesControllerV3D smartWaveParticles;
        public SFXControllerV3D sfxcontroller;

        private float globalProgress;
        private float globalImpactProgress;
        private LaserLineV3D[] lls;
        private LightLineV3D[] lils;
        private Renderer[] renderers;
        #endregion
        public bool Emitting { get; private set; }

        private float _dpsCountdown;
        private bool CanDealDamage => _dpsCountdown <= 0;
        public override float Damage => _damageSplitMiliSecs * _dps;

        private void Start()
        {
            _dpsCountdown = 0;
            EffectStartup();
        }

        void Update()
        {
            _dpsCountdown -= Time.deltaTime;
            EffectUpdate();
        }

        private void FixedUpdate()
        {
            if (!Emitting || !CanDealDamage) return;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxLength, _hurtableMask))
            {
                IHittable hittable;
                if (hit.collider.TryGetComponent<IHittable>(out hittable))
                {
                    DealDamage(hittable);
                }
            }
        }

        protected override void OnDealDamage(IHittable hittable)
        {
            _dpsCountdown = _damageSplitMiliSecs;
        }

        private void EffectStartup()
        {
            globalProgress = 1f;
            globalImpactProgress = 1f;
            lls = GetComponentsInChildren<LaserLineV3D>(true);
            lils = GetComponentsInChildren<LightLineV3D>(true);
            renderers = GetComponentsInChildren<Renderer>(true);

            foreach (LightLineV3D lil in lils)
            {
                lil.SetFinalColor(finalColor);
                lil.maxLength = maxLength;
                lil._hittableMask = _hurtableMask;
            }
            foreach (Renderer rend in renderers)
                rend.material.SetColor("_FinalColor", finalColor);

            foreach (LaserLineV3D ll in lls)
            {
                ll.maxLength = maxLength;
                ll._hittableMask = _hurtableMask;
            }
            startPointEffect.SetFinalColor(finalColor);
            endPointEffect.SetFinalColor(finalColor);
        }
        private void EffectUpdate()
        {
            for (int i = 0; i < renderers.Length; i++)
                // Control Gamma and Linear modes
                renderers[i].material.SetFloat("_GammaLinear", gammaLinear);

            // Sending global progress value to other scripts
            startPointEffect.SetGlobalProgress(globalProgress);
            startPointEffect.SetGlobalImpactProgress(globalImpactProgress);
            endPointEffect.SetGlobalProgress(globalProgress);
            endPointEffect.SetGlobalImpactProgress(globalImpactProgress);
            smartWaveParticles.SetGlobalProgress(globalProgress);

            // Overall progress control
            if (meshRend != null)
            {
                meshRend.material.SetColor("_EmissionColor", finalColor * meshRendPower);
            }

            if (globalProgress < 1f)
            {
                globalProgress += Time.deltaTime * globalProgressSpeed;
            }

            if (globalImpactProgress < 1f)
            {
                globalImpactProgress += Time.deltaTime * globalImpactProgressSpeed;
            }

            if (Emitting)
            {
                globalProgress = 0f;
                endPointEffect.emit = true;
            }

            for (int i = 0; i < lls.Length; i++)
            {
                lls[i].SetGlobalProgress(globalProgress);
                lls[i].SetGlobalImpactProgress(globalImpactProgress);
            }
            for (int i = 0; i < lils.Length; i++)
            {
                lils[i].SetGlobalProgress(globalProgress);
                lils[i].SetGlobalImpactProgress(globalImpactProgress);
            }

            sfxcontroller.SetGlobalProgress(1f - globalProgress);
            if (!Emitting && globalProgress >= 1)
                DisableObject();
        }

        public void StartEmitting()
        {
            Emitting = true;
            EnableObject();
            globalImpactProgress = 0f;
            smartWaveParticles.SpawnWave();
            sfxcontroller.PlayShootSound();
        }

        public void StopEmitting()
        {
            Emitting = false;
            endPointEffect.emit = false;
        }

        private void DisableObject()
        {
            gameObject.SetActive(false);
        }
        private void EnableObject()
        {
            gameObject.SetActive(true);
        }
    }
}