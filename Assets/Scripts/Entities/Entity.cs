using System;
using Projectiles;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Collider))]
    public abstract class Entity : MonoBehaviour, IHittable
    {
        [SerializeField] private float _maxHp = 20;
        public float MaxHp => _maxHp;
        private float _currentHP;
        public float CurrentHP
        {
            get => _currentHP;
            private set => _currentHP = Mathf.Clamp(value, 0, MaxHp);
        }
        public bool Invulnerable { get; private set; }
        public Entity AttachedEntity => this;
        private Collider _col;
        public Collider AttachedCollider => _col;
        private bool _dead;

        protected void Awake()
        {
            _currentHP = _maxHp;
            _col = GetComponent<Collider>();
            OnAwake();
        }

        public void Hit(RaycastHit hit, Projectile projectile, float damage)
        {
            OnHit(hit, projectile, damage);
        }

        public void Hit(float damage)
        {
            OnHit(damage);
        }

        protected virtual void OnHit(RaycastHit hit, Projectile projectile, float damage) { DealDamage(damage); }
        protected virtual void OnHit(float damage) { DealDamage(damage); }
        protected virtual void OnAwake() { }
        protected virtual void DeathTriggered() { }
        private void Death()
        {
            DeathTriggered();
            OnDeath?.Invoke();
        }

        protected void DealDamage(float damage, bool overrideInvulnerability = false)
        {
            if (_dead || (Invulnerable && !overrideInvulnerability)) return;

            CurrentHP -= damage;

            if (CurrentHP <= 0)
                Death();
        }

        public void Heal(float heal)
        {
            CurrentHP += heal;
        }

        public void Revive()
        {
            CurrentHP = _maxHp;
            _dead = false;
        }

        public void InstaKill()
        {
            DealDamage(float.MaxValue, true);
        }

        public void SetInvulnerable(bool state)
        {
            Invulnerable = state;
        }

        protected void ResetOnDeathAction()
        {
            OnDeath = default;
        }

        public event Action OnDeath;
    }
}