using Entities.Modularius.BehaviourCreation;
using ModulariaBehaviourTree;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(ComposedBehavior))]
    public class ComposedBehaviorEditor : BehaviourSelectionEditor
    {
        private ComposedBehavior _profile;
        private SerializedProperty _selectorCreator;
        protected override TypeToggle[] TypeToggles => _profile.TypeToggles;
        private void OnEnable()
        {
            _profile = target as ComposedBehavior;
            _profile.UpdateToggles();
            _selectorCreator = serializedObject.FindProperty("_isRandomSelector");
        }

        public override void OnInspectorGUI()
        {
            ShowWarningText("The Name property must be the same as the filename!\n" +
            "Beware not to loop behaviours.");
            DrawDefaultInspector();
            if (_profile.ComposedType == TreeComponentType.Selector)
                ShowSelectorModule();
            Space(3);
            EditorGUILayout.LabelField("Added behaviours ---", GUILayout.Height(15));
            ShowAdded();
            Space(3);
            ShowPossibleBehaviours(_profile);
            DeleteButton(_profile);
            serializedObject.ApplyModifiedProperties();
        }

        private void ShowSelectorModule()
        {
            EditorGUILayout.PropertyField(_selectorCreator);
        }

        protected override void DisplaySingleToggle(TypeToggle typeToggle)
        {
            bool added = _profile.SelectionContains(typeToggle);
            string displayName =
                (added ? "Remove :" : "Add :") +
                typeToggle.Name;

            if (GUILayout.Button(displayName))
            {
                if (added)
                    _profile.RemoveFromSelectedBehaviours(typeToggle);
                else
                    _profile.AddToSelectedBehaviours(typeToggle);
            }
        }

        private void ShowAdded()
        {
            for (int i = 0; i < _profile.SelectedBehaviours.Length; i++)
                EditorGUILayout.LabelField(_profile.SelectedBehaviours[i].Name, GUILayout.Height(13));
        }
    }
}