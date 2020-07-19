using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ModulariaBehaviourTree.Debugging
{
    public class DebugText : MonoBehaviour
    {
        public static List<DebugText> Texts;
        public static bool _show;
        [SerializeField] private TextMeshPro _textPro = default;

        private BehaviourTree _tree;

        public static void HideAll()
        {
            SetStates(false);
        }
        public static void ShowAll()
        {
            SetStates(true);
        }

        private static void SetStates(bool state)
        {
            foreach(DebugText t in Texts)
                t?.SetState(state);
        }

        private void Awake()
        {
            if (Texts == default) Texts = new List<DebugText>(100);
        }

        // Start is called before the first frame update
        void Start()
        {
            Texts.Add(this);
            _tree = GetComponentInParent<BehaviourTree>();
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            _textPro.text = _tree.CurrentRunning?.GetType().Name;
        }

        private void SetState(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}