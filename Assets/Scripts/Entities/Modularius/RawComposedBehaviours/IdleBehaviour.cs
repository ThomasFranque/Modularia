using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModulariaBehaviourTree;

namespace Entities.Modularius.ComposedBehaviours
{
    public class IdleBehaviour : ModularBehaviour
    {
        public override ModulariuType Type => default;
        public override float Weight => 1f;

        private bool inIdle;

        protected override void OnAwake()
        {
            inIdle = false;
        }

        protected override void OnExecute()
        {
            inIdle = true;
            Complete();
        }

        protected override void OnBehaviourKill(bool isBehaviourActive)
        {
            EndIdle();
        }

        public override void NewBehaviourStarted()
        {
            EndIdle();
        }

        private void EndIdle()
        {
            inIdle = false;
        }

        public override bool Condition() => !inIdle;
    }
}