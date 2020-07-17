using Entities.Modularius;
using UnityEditor;

namespace Editors
{
    [CustomEditor(typeof(ModulariuPartProfile))]
    public class ModulariuPartProfileEditor : BehaviourSelectionEditor
    {
        private ModulariuPartProfile _profile;
        private SerializedProperty _typeToggles;
        protected override TypeToggle[] TypeToggles => _profile.TypeToggles;
        private void OnEnable()
        {
            _profile = target as ModulariuPartProfile;
            _profile.UpdateToggles();
        }

        public override void OnInspectorGUI()
        {
            ShowWarningText("Beware not to loop behaviours.");
            DrawDefaultInspector();
            Space(3);
            ShowPossibleBehaviours(_profile);
            DeleteButton(_profile);
        }
    }
}