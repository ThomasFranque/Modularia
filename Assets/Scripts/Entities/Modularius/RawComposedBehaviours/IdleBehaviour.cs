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

        protected override void OnAwake()
        {
            this.enabled = false;
        }

        protected override void OnExecute()
        {
            this.enabled = true;
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
            this.enabled = false;
        }

        public override bool Condition() => !this.enabled;
    }
}