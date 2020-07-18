using System;
using System.Collections.Generic;
using Entities;
using Entities.Modularius;
using Entities.Modularius.BaseBehaviours;
using Entities.Modularius.Parts;
using UnityEngine;

namespace ModulariaBehaviourTree
{
    [RequireComponent(typeof(BehaviourTree), typeof(Entity))]
    [RequireComponent(typeof(ProximityChecker), typeof(SmoothLookAt), typeof(Follow))]
    public abstract class ModularBehaviour : MonoBehaviour, ITreeComponent
    {
        private const string BEHAVIOUR_SPAWNS_OBJECT_NAME = "Behaviour Spawns";
        [SerializeField, ReadOnly] private bool _executing = default;
        protected const string PATH_TO_PREFABS_FOLDER = "Prefabs/";
        public PlayerEntity Player { get; private set; }
        public abstract ModulariuType Type { get; }
        public abstract float Weight { get; }
        public BehaviourTree Tree { get; set; }
        protected ITreeComponent _caller;
        protected List<ITreeComponent> Parents;

        protected Entity AttachedEntity { get; private set; }
        protected Transform BehaviourSpawnsTransform { get; private set; }
        protected ProximityChecker Proximity { get; private set; }
        protected SmoothLookAt LookAtPlayer { get; private set; }
        protected Follow Follow { get; private set; }

        public bool Executing
        {
            get => _executing;
            private set { _executing = value; }
        }

        protected void Awake()
        {
            BehaviourSpawnsTransform = transform.Find(BEHAVIOUR_SPAWNS_OBJECT_NAME);
            Player = GameObject.FindObjectOfType<PlayerEntity>();
            AttachedEntity = GetComponent<Entity>();
            Proximity = GetComponent<ProximityChecker>();
            LookAtPlayer = GetComponent<SmoothLookAt>();
            Follow = GetComponent<Follow>();
            OnAwake();
        }

        protected virtual void OnAwake() { }

        public void Execute()
        {
            Execute(default);
        }

        public bool Execute(ITreeComponent caller, Action onComplete = default)
        {
            if (!Condition()) return false;
            _caller = caller;
            Execute(onComplete);
            return true;
        }

        private void Execute(Action onComplete)
        {
            OnComplete = onComplete;
            Executing = true;
            OnExecute();
        }

        protected virtual void OnExecute() { }

        public void Complete()
        {
            ResetBasicBehaviours();
            if (OnComplete == default)
                Tree.SetDone();
            else
                OnComplete?.Invoke();
            Executing = false;
        }

        private void ResetBasicBehaviours()
        {
            LookAtPlayer.StopLooking();
            Follow.StopFollowing();
        }

        public void Setup(BehaviourTree tree, ITreeComponent parent)
        {
            if (Parents == null) Parents = new List<ITreeComponent>(2);
            Parents.Add(parent);

            Tree = tree;
        }

        /// <summary>
        /// 
        /// </summary>
        // Why master Anakin? *zwoom*
        public void Kill()
        {
            OnComplete = default;
            if (Executing)
                OnBehaviourKill(Executing);
        }

        // Will only get called if it is killed and the behaviour was running
        protected virtual void OnBehaviourKill(bool isBehaviourActive) { }

        protected void RestartTree()
        {
            Tree.Kill();
            Tree.SetDone();
        }

        public virtual bool Condition() { return true; }

        public bool Finalize() => true;

        public event Action OnComplete;
    }
}