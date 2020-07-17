using System;
using UnityEngine;

namespace Enhancements
{
    public abstract class Enhancement
    {
        public bool Unlocked { get; private set; }
        public EnhancementModule[] AllModules { get; protected set; }

        protected static EnhancementIcons Icons;

        public Sprite Icon { get; protected set; }

        public Enhancement()
        {
            if (Icons == null)
                Icons = Resources.Load<EnhancementIcons>("Visuals/UI/Skill Icons/Icons");

            Icon = Icons.Generic;
        }

        public void Unlock()
        {
            Unlocked = true;
            UpgradeAll();
            OnUnlock.Invoke();
        }

        public void UpgradeAll()
        {
            for(int i = 0 ; i < AllModules.Length; i++)
                AllModules[i].ForceUpgrade();
        }

        public Action OnUnlock;
        public Action OnEnchance;
    }
}