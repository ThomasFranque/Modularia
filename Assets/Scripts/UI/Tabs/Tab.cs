using UI.TabEvents;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class Tab : MonoBehaviour
    {
        [Space]
        [SerializeField] private GameObject _linkedWindow = default;
        [Space]
        [SerializeField] private TabOnCloseEvent OnClose = default;
        [SerializeField] private TabOnCloseEvent OnOpen = default;
        private Button _b;
        public TabGroup ContainedTabGroup { get; set; }
        public bool Opened { get; private set; }
        ColorBlock _openedColorBlock;
        ColorBlock _closedColorBlock;
        private void Awake()
        {
            _b = GetComponent<Button>();
            _b.onClick.AddListener(RequestOpen);
            _closedColorBlock = _b.colors;
            _openedColorBlock = _b.colors;
            _openedColorBlock.normalColor = _b.colors.selectedColor;
        }

        public void RequestOpen()
        {
            ContainedTabGroup.RequestOpen(this);
        }

        public void Open()
        {
            if (Opened) return;

            _b.colors = _openedColorBlock;
            ForceSilentOpen();
            OnClose?.Invoke();
        }
        public void Close()
        {
            if (!Opened) return;

            _b.colors = _closedColorBlock;
            ForceSilentClose();
            OnOpen?.Invoke();
        }

        public void ForceSilentOpen()
        {
            _b.Select();
            _linkedWindow.SetActive(true);
            Opened = true;
        }
        public void ForceSilentClose()
        {
            _linkedWindow.SetActive(false);
            Opened = false;
        }

        public void SetLinkedWindow(GameObject newLinked)
        {
            _linkedWindow = newLinked;
        }
    }
}