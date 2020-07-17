using System.Collections;
using System.Collections.Generic;
using Entities;
using Projectiles.Enhancements;
using UnityEngine;

namespace Projectiles
{
    public class Projectile : Damager
    {
        private const float SPEED = 90;
        private const float LIFETIME = 10;

        private float _lifetimeCountdown;

        [SerializeField] private TrailRenderer _trailRenderer = default;
        [SerializeField] private ProjectileEnhancement[] _enhancements = default;
        [SerializeField] private GameObject _hitParticlesPrefab = default;
        [SerializeField] private ParticleSystem _mainParticles = default;
        [SerializeField] private LayerMask _hitMasks = default;
        private ParticleSystem _hitParticles;
        private Vector3 _lastFixedPos;
        public bool InUse { get; private set; }
        public override float Damage => 5;

        private void Awake()
        {
            _hitParticles = Instantiate(_hitParticlesPrefab).GetComponent<ParticleSystem>();
            _hitParticles.gameObject.hideFlags = HideFlags.HideInHierarchy;
            _lifetimeCountdown = LIFETIME;
            _lastFixedPos = transform.position;
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * Time.deltaTime * SPEED);
            _lifetimeCountdown -= Time.deltaTime;
            if (_lifetimeCountdown <= 0)
                Disable(false);
        }

        private void FixedUpdate()
        {
            RaycastHit hit;
            if (Physics.Linecast(_lastFixedPos, transform.position, out hit, _hitMasks))
                OnCollisionDetected(ref hit);
            _lastFixedPos = transform.position;
            //_rb.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * SPEED);
        }

        private void OnCollisionDetected(ref RaycastHit hit)
        {
            IHittable h;

            if (hit.collider.TryGetComponent<IHittable>(out h))
            {
                DealDamage(hit, h, this);
                for (int i = 0; i < _enhancements.Length; i++)
                    _enhancements[i].OnHittableCollision(ref h);
            }

            for (int i = 0; i < _enhancements.Length; i++)
            {
                _enhancements[i].OnAnyCollision(ref hit);
                _enhancements[i].OnObstacleCollision(ref hit);
            }
            PlayHitAt(hit.point + (hit.normal / 8));
            Debug.Log(hit.collider.name);

            //Disable(true);
        }

        public void Reflect(Vector3 dir, Vector3 from)
        {
            transform.forward = dir;
            transform.position = from + (dir / 8);
            _lastFixedPos = transform.position;
        }

        public void Disable(bool hitSomething)
        {
            _mainParticles.Stop();
            gameObject.SetActive(false);
            InUse = false;
        }
        public void SetInUse()
        {
            InUse = true;
        }

        public void Shoot(Vector3 from, Vector3 lookAt)
        {
            //_rb.velocity = Vector3.zero;
            _lifetimeCountdown = LIFETIME;
            _trailRenderer.enabled = false;
            transform.position = from;
            _trailRenderer.enabled = false;
            transform.LookAt(lookAt);
            _lastFixedPos = transform.position;
            gameObject.SetActive(true);
            for (int i = 0; i < _enhancements.Length; i++)
                _enhancements[i].OnShoot();
            _mainParticles.Play();
        }
        public void ShootSecondary(ref Projectile main, Vector3 offset)
        {
            //_rb.velocity = Vector3.zero;
            _lifetimeCountdown = LIFETIME;
            _trailRenderer.enabled = false;
            transform.position = main.transform.position + offset;
            _trailRenderer.enabled = false;
            transform.rotation = main.transform.rotation;
            _lastFixedPos = transform.position;
            gameObject.SetActive(true);
            for (int i = 0; i < _enhancements.Length; i++)
                _enhancements[i].OnShoot();
            _mainParticles.Play();
        }

        private void PlayHitAt(Vector3 pos)
        {
            _hitParticles.transform.position = pos;
            _hitParticles.Play();
        }
    }
}