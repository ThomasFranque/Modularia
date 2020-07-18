using UnityEngine;

namespace ModulariaBehaviourTree.Debugging
{
    public class PreBuiltTree : MonoBehaviour
    {
        [SerializeField] private ModularBehaviour[] _behaviours = default;
        // Start is called before the first frame update
        void Start()
        {
            TreeSequence sequence = new TreeSequence();
            for (int i = 0; i < _behaviours.Length; i++)
                sequence.AddNew(_behaviours[i]);

            GetComponent<BehaviourTree>().Initialize(sequence);
        }
    }
}