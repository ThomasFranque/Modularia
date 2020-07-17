using System.Collections;
using System.Collections.Generic;
using Enhancements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EnhancementModuleUI : MonoBehaviour
    {
        [SerializeField] private Button _upgradeButton = default;
        [SerializeField] private TextMeshProUGUI _namePro = default;
        [SerializeField] private TextMeshProUGUI _levelPro = default;
        [SerializeField] private TextMeshProUGUI _costPro = default;

        private EnhancementModule _module;

        public void SetModule(EnhancementModule module)
        {
            _module = module;
            _namePro.text = module.Name;
            _costPro.text = module.Cost.ToString();
            _levelPro.text = module.Level.ToString();

            _upgradeButton.onClick.AddListener(UpgradeIntention);
        }

        private void UpgradeIntention()
        {
            _module.ForceUpgrade();
            UpdateInfo();
        }

        private void UpdateInfo()
        {            
            _costPro.text = _module.Cost.ToString();
            _levelPro.text = _module.Level.ToString();
        }
    }
}