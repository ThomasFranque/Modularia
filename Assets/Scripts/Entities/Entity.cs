﻿using System;
using LevelGeneration.Individuals;
using Projectiles;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Collider))]
    public abstract class Entity : MonoBehaviour, IHittable
    {
        [SerializeField] private float _maxHp = 20;
        [SerializeField] private ModifierStats _modStats = default;
        private float _currentHP;
        public bool Invulnerable { get; private set; }
        public Entity AttachedEntity => this;
        private Collider _col;
        public Collider AttachedCollider => _col;
        private bool _dead;

        public Room CurrentRoom { get; set; }

        public float CurrentHP
        {
            get => _currentHP;
            private set => _currentHP = Mathf.Clamp(value, 0, MaxHp);
        }

        public ModifierStats ModifierStats
        {
            get => _modStats;
            protected set => _modStats = value;
        }

        public float MaxHp => _maxHp;
        public bool Damaged => CurrentHP < MaxHp;

        protected void Awake()
        {
            _currentHP = _maxHp;
            _col = GetComponent<Collider>();
            OnAwake();
        }

        public void Hit(RaycastHit hit, Projectile projectile, float damage)
        {
            OnHit(hit, projectile, damage - (damage * ModifierStats.Def));
        }

        public void Hit(float damage)
        {
            OnHit(damage - (damage * ModifierStats.Def));
        }

        protected virtual void OnHit(RaycastHit hit, Projectile projectile, float damage)
        {
            DealDamage(damage);
        }
        protected virtual void OnHit(float damage)
        {
            DealDamage(damage);
        }
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

        public bool Heal(float heal)
        {
            bool overHeal = CurrentHP + heal >= MaxHp;
            CurrentHP += heal;
            Debug.Log("Bandages * " + heal);
            return overHeal;
        }

        public void Revive()
        {
            gameObject.SetActive(true);
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