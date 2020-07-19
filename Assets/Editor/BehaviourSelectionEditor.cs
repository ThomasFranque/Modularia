using Entities.Modularius.BehaviourCreation;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    public abstract class BehaviourSelectionEditor : Editor
    {
        protected const string RAW_PREFIX = "Raw\t: ";
        protected const string COMPOSED_PREFIX = "Comp\t: ";
        protected abstract TypeToggle[] TypeToggles { get; }

        string toDelete = "";
        bool confirmation;
        protected void ShowPossibleBehaviours(UnityEngine.Object dirtyObject)
        {
            EditorGUILayout.LabelField("Possible behaviours ---", GUILayout.Height(10));
            ShowToggles();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(dirtyObject);
            }
        }

        private void ShowToggles()
        {
            for (int i = 0; i < TypeToggles.Length; i++)
            {
                DisplaySingleToggle(TypeToggles[i]);
            }
        }

        protected virtual void DisplaySingleToggle(TypeToggle typeToggle)
        {
            string displayName =
                (typeToggle.IsRaw ? RAW_PREFIX : COMPOSED_PREFIX) +
                typeToggle.Name;

            typeToggle.SetToggle(EditorGUILayout.Toggle(displayName,
                typeToggle.Toggle));
        }

        protected void Space(int height)
        {
            EditorGUILayout.LabelField("", GUILayout.Height(height));
        }

        protected void DeleteButton(BehaviourToggleCollection behaviourToggleCollection)
        {
            Space(10);
            toDelete = EditorGUILayout.TextField("Target name: ", toDelete);
            if (!confirmation)
            {
                if (GUILayout.Button("Delete"))
                    confirmation = true;
            }
            else if (GUILayout.Button("Certain?"))
            {
                behaviourToggleCollection.Remove(toDelete);
                toDelete = "";
                confirmation = false;
            }
        }

        protected void ShowWarningText(string text)
        {
            EditorGUILayout.HelpBox(text, MessageType.Warning);
        }
    }
}