using System.Collections;
using System.Collections.Generic;
using Enhancements;
using UnityEngine;
using Character;

namespace UI
{
    public class EnhancementMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _enhancementDisplayPrefab = default;
        [SerializeField] private GameObject _enhancementTabPrefab = default;
        [SerializeField] private Transform _windowsTransform = default;
        [SerializeField] private Transform _tabsContents = default;
        [SerializeField] private TabGroup _tabGroup = default;
        [SerializeField] private Transform _holder = default;
        private PlayerEnhancementsHandler _handler;

        // Start to wait for EnhancementHandler's awake to load
        void Start()
        {
            _holder.gameObject.SetActive(true);
            _handler = GameObject.FindObjectOfType<PlayerEnhancementsHandler>();

            if (_handler == null)
            {
                Debug.LogError("EnhancementHandler not found. UI wont update.");
                return;
            }

            for (int i = 0; i < _handler.Enhancements.Count; i++)
            {
                EnhancementDisplay newDisplay =
                    Instantiate(_enhancementDisplayPrefab, _windowsTransform)
                    .GetComponent<EnhancementDisplay>();
                newDisplay.SetEnhancement(_handler.Enhancements[i]);
                
                newDisplay.name = nameof(newDisplay.Enhancement);
                EnhancementTab t =
                    Instantiate(_enhancementTabPrefab, _tabsContents).GetComponent<EnhancementTab>();
                t.SetLinkedWindow(newDisplay.gameObject);
                t.SetSprite(_handler.Enhancements[i].Icon);
                t.name = newDisplay.name + " Tab";      
            }
            _tabGroup.GetChildTabs();
            _holder.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))
            {
                Toggle();
            }
        }

        private void Toggle()
        {
            bool active = !_holder.gameObject.activeSelf;
            _holder.gameObject.SetActive(active);

            if (active)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                PlayerControl.LockMovement = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PlayerControl.LockMovement = false;
            }
        }
    }
}