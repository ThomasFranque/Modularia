using UnityEngine;

namespace ModulariaBehaviourTree.Debugging
{
    public class TextStateCotroller : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B)) DebugText.ShowAll();
            else if (Input.GetKeyUp(KeyCode.B)) DebugText.HideAll();
        }
    }
}