﻿using System;
using System.Collections.Generic;
using Entities.Modularius.BehaviourCreation;
using UnityEngine;

namespace EntityFactory
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] private ComposedBehavior _composedBehaviour = default;

        private void Awake()
        {
            Type[] types = _composedBehaviour.GetAllChildTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Debug.Log(types[i].Name);
            }
        }
    }
}