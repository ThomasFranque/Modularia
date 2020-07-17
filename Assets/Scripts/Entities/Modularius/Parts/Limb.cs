using UnityEngine;

namespace Entities.Modularius.Parts
{
    public class Limb : Entity
    {
        private Core _core;

        public void INIT(Core core)
        {
            _core = core;
        }
    }
}