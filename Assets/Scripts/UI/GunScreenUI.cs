using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UI
{
    public class GunScreenUI : MonoBehaviour
    {
        [SerializeField] private Transform _ammoSlotsContents = default;
        [SerializeField] private GameObject _ammoSlotPrefab = default;
        private List<Image> _ammoSlots;
        private Gun _gun;
        private int _lastSlotIndex;
        private int MagMaxAmmo => _gun.Mag.MaxAmmo;
        private int MagAmmo => _gun.Mag.Ammo;

        private void Start()
        {
            _gun = FindObjectOfType<Gun>();
            if (_gun == default)
            {
                Debug.LogError("No gun found, Gun screen wont update.");
                return;
            }

            _lastSlotIndex = MagMaxAmmo - 1;
            _ammoSlots = new List<Image>(MagMaxAmmo * 2);

            UpdateTotalSlots();
            UpdateEnabled();

            _gun.Mag.OnSpend += UpdateEnabled;
            _gun.Mag.OnReplenish += UpdateEnabled;
        }

        private void UpdateEnabled()
        {
            UpdateTotalSlots();
            for (int i = 0; i < MagMaxAmmo; i++)
                _ammoSlots[i].enabled = i + 1 <= MagAmmo;
        }

        private void UpdateTotalSlots()
        {
            int difference = MagMaxAmmo - _ammoSlots.Count;
            for (int i = 0; i < difference; i++)
            {
                Image newImg = Instantiate(_ammoSlotPrefab, _ammoSlotsContents).GetComponent<Image>();
                _ammoSlots.Add(newImg);
                newImg.enabled = false;
            }
        }
    }
}