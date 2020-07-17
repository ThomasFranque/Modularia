using System;
using Enhancements;
using UnityEngine;

namespace Weapons
{
    public class Magazine : MonoBehaviour
    {
        private Gun _gun;
        private int _size;
        private float _replenishTime;
        private float _replenishCountdown;

        public int Ammo { get; private set; }
        public int MaxAmmo => _size + (int) PlayerEnhancementsHandler.Instance.GenericEnhancements.AllModules[0].Value;
        public bool HasAmmo => Ammo <= _size;
        private bool CanReplenish => _replenishCountdown <= 0 && Ammo < MaxAmmo;

        public void Setup(Gun gun, int size, float replenishTime)
        {
            _gun = gun;
            _size = size;
            _replenishTime = replenishTime;
            Ammo = size - (int) (size / 3);
        }

        private void Update()
        {
            _replenishCountdown -= Time.deltaTime;
            if (CanReplenish)
                Replenish();
        }

        private void Spend()
        {
            Ammo--;
            RestartCountdown();
            OnSpend?.Invoke();
        }

        private void Replenish()
        {
            Ammo++;
            RestartCountdown();
            OnReplenish?.Invoke();
        }

        public bool TrySpend()
        {
            if (Ammo > 0)
            {
                Spend();
                return true;
            }
            return false;
        }

        public void AddAmmo(int amount)
        {
            _size += amount;
        }

        private void RestartCountdown() => _replenishCountdown = _replenishTime -
            (PlayerEnhancementsHandler.Instance.GenericEnhancements.AllModules[1].Value * _replenishTime);

        public Action OnReplenish;
        public Action OnSpend;
    }
}