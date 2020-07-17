using System;
using System.Collections.Generic;
using Enhancements;
using Projectiles;
using UnityEngine;

namespace Weapons
{
    public class Gun : MonoBehaviour
    {
        private const int INITIAL_PROJECTILES = 100;
        [SerializeField] private Transform _gunEnd = default;
        [SerializeField] private float _shootDelay = 0.3f;
        [SerializeField] private float _focusSpeed = 4.0f;
        [SerializeField] private ParticleSystem _muzzleFlash = default;
        [SerializeField] private GameObject _projectile = default;

        private Queue<Projectile> _projectiles;
        private Magazine _mag;
        private RaycastHit _gunTargetHit;
        private Animator _anim;
        private Quaternion _initialLocalRotation;
        private Vector3 _initialLocalPosition;
        private Vector3 _runtimeFocusPosition;
        private readonly Vector3 _focusPosition = new Vector3(0, -0.383f, 0.36f);
        private float _shootCountdown;
        private int _shootHash;

        private bool _inFocus;

        private bool CountdownIsDone => _shootCountdown <= 0;
        public Magazine Mag => _mag;
        private Transform _projectileParent;
        private List<Projectile> _multiProjectiles;
        private void Awake()
        {
            _multiProjectiles = new List<Projectile>(5);
            _initialLocalPosition = transform.localPosition;
            _initialLocalRotation = transform.localRotation;
            _runtimeFocusPosition = _initialLocalPosition;
            _anim = GetComponentInChildren<Animator>();
            _shootHash = Animator.StringToHash("Shoot");

            CreateMagazine();
            CreateProjectiles();

            void CreateProjectiles()
            {
                _projectiles = new Queue<Projectile>(INITIAL_PROJECTILES);
                _projectileParent = new GameObject("Projectiles").transform;
                for (int i = 0; i < INITIAL_PROJECTILES; i++)
                {
                    CreateNewProjectile();
                }
            }

            void CreateMagazine()
            {
                GameObject magObj = new GameObject("Magazine");
                magObj.transform.SetParent(transform);
                _mag = magObj.AddComponent<Magazine>();
                _mag.Setup(this, 15, _shootDelay * 2f);
            }
        }

        private Projectile CreateNewProjectile()
        {
            Projectile newProjectile = Instantiate(_projectile)
                .GetComponent<Projectile>();
            newProjectile.transform.SetParent(_projectileParent);
            _projectiles.Enqueue(newProjectile);
            newProjectile.Disable(false);
            return newProjectile;
        }

        private void Update()
        {
            _shootCountdown -= Time.deltaTime;
            UpdateFocus();
        }

        private void UpdateFocus()
        {
            if (Vector3.Distance(transform.position, _runtimeFocusPosition) <= 0.001f) return;

            float step = _focusSpeed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _runtimeFocusPosition, step);
        }

        public void Shoot()
        {
            if (!CountdownIsDone || !_mag.TrySpend()) return;

            Vector3 screenCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 50));
            _anim.SetTrigger(_shootHash);
            _shootCountdown = _shootDelay -
                (PlayerEnhancementsHandler.Instance.GenericEnhancements.AllModules[2].Value * _shootDelay);

            Projectile p;
            GetFreeShot(out p);
            p.Shoot(_gunEnd.position, screenCenter);
            TryMultiShot(ref p);
            _muzzleFlash.Play();
        }

        private void GetFreeShot(out Projectile p)
        {
            int i = 0;
            int max = _projectiles.Count;
            do
            {
                p = _projectiles.Dequeue();
                _projectiles.Enqueue(p);
                i++;
                if (i >= max)
                {
                    p = CreateNewProjectile();
                    break;
                }
            } while (p.InUse);
            p.SetInUse();
        }

        private void GetFreeMultiShots()
        {
            _multiProjectiles.Clear();
            for (int i = 0; i < (int) PlayerEnhancementsHandler.Instance.MultishotEnhancement.AllModules[0].Value; i++)
            {
                Projectile p;
                int j = 0;
                int max = _projectiles.Count;
                do
                {
                    p = _projectiles.Dequeue();
                    _projectiles.Enqueue(p);
                    j++;
                    if (j >= max)
                    {
                        p = CreateNewProjectile();
                        break;
                    }
                } while (p.InUse && _multiProjectiles.Contains(p));

                _multiProjectiles.Add(p);
                p.SetInUse();
            }
        }

        private void TryMultiShot(ref Projectile main)
        {
            Vector3 centerPos = main.transform.position;
            int amount = (int) PlayerEnhancementsHandler.Instance.MultishotEnhancement.AllModules[0].Value;
            if (amount == 1)
            {
                Projectile newP;
                GetFreeShot(out newP);
                newP.ShootSecondary(ref main, main.transform.right * (MultishotEnhancement.OFFSET / 2));
                main.transform.position -= main.transform.right * (MultishotEnhancement.OFFSET / 2);
                return;
            }
            GetFreeMultiShots();
            Vector3 offset = default;
            for (int i = 0; i < amount; i++)
            {
                switch (i)
                {
                    case 0:
                        offset = main.transform.right * MultishotEnhancement.OFFSET;
                        break;
                    case 1:
                        offset = -main.transform.right * MultishotEnhancement.OFFSET;
                        break;
                    case 2:
                        offset = main.transform.up * MultishotEnhancement.OFFSET;
                        break;
                    case 3:
                        offset = -main.transform.up * MultishotEnhancement.OFFSET;
                        break;
                }
                _multiProjectiles[i].ShootSecondary(ref main, offset);
            }

            // for (int i = _multiProjectiles.Count - 1; i >= 0; i--)
            // {
            //     _multiProjectiles[i]?.ShootSecondary(main);
            // }
        }

        public void Focus()
        {
            if (_inFocus) return;
            _inFocus = true;
            _runtimeFocusPosition = _focusPosition;
        }

        public void UnFocus()
        {
            if (!_inFocus) return;
            _inFocus = false;
            _runtimeFocusPosition = _initialLocalPosition;
        }

        private Action OnShoot;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawLine(_gunEnd.position, _gunEnd.forward * 500);
        }
    }
}