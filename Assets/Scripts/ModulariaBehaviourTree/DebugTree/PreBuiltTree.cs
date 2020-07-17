using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModulariaBehaviourTree;
using Entities.Modularius.ComposedBehaviours;

public class PreBuiltTree : MonoBehaviour
{
    [SerializeField] private ModularBehaviour[] _behaviours = default;
    // Start is called before the first frame update
    void Start()
    {
        TreeSequence sequence = new TreeSequence();
        for (int i = 0 ; i< _behaviours.Length; i++)
            sequence.AddNew(_behaviours[i]);
            
        GetComponent<BehaviourTree>().Initialize(sequence);
    }
}
