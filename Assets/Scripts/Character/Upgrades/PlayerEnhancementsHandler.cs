using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enhancements
{
    public class PlayerEnhancementsHandler : MonoBehaviour
    {
        public static PlayerEnhancementsHandler Instance { get; private set; }
        
        public List<Enhancement>    Enhancements            { get; private set; }

        public GenericEnhancements  GenericEnhancements     { get; private set; }
        public BleedEnhancement     BleedEnhancement        { get; private set; }
        public PiercingEnhancement  PiercingEnhancement     { get; private set; }
        public MultishotEnhancement MultishotEnhancement    { get; private set; }
        public RicochetEnhancement  RicochetEnhancement     { get; private set; }
        public ExplosiveEnhancement ExplosiveEnhancement    { get; private set; }

        private void Awake()
        {
            Instance = this;
            Enhancements = new List<Enhancement>();

            GenericEnhancements = new GenericEnhancements();
            BleedEnhancement = new BleedEnhancement();
            PiercingEnhancement = new PiercingEnhancement();
            MultishotEnhancement = new MultishotEnhancement();
            RicochetEnhancement = new RicochetEnhancement();
            ExplosiveEnhancement = new ExplosiveEnhancement();

            Enhancements.Add(GenericEnhancements);
            Enhancements.Add(BleedEnhancement);
            Enhancements.Add(PiercingEnhancement);
            Enhancements.Add(MultishotEnhancement);
            Enhancements.Add(RicochetEnhancement);
            Enhancements.Add(ExplosiveEnhancement);
        }
    }
}