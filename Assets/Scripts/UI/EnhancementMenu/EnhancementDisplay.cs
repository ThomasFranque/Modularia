using System.Collections;
using System.Collections.Generic;
using Enhancements;
using UnityEngine;

namespace UI
{
    public class EnhancementDisplay : MonoBehaviour
    {
        private Enhancement _enhancement;
        public Enhancement Enhancement => _enhancement;
        [SerializeField] private Transform _modulesContent = default;
        [SerializeField] private GameObject _modulesPrefab = default;

        public void SetEnhancement(Enhancement enhancement)
        {
            _enhancement = enhancement;

            for (int i = 0; i < enhancement.AllModules.Length; i++)
                Instantiate(_modulesPrefab, _modulesContent)
                .GetComponent<EnhancementModuleUI>()
                .SetModule(enhancement.AllModules[i]);
        }
    }
}