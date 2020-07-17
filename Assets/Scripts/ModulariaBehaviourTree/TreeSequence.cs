﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModulariaBehaviourTree
{
    public class TreeSequence : ITreeComponent
    {
        public float Weight
        {
            get
            {
                // Create total chance
                float totalChance = 0;
                // Get total
                for (int i = 0; i < _sequence.Length; i++)
                    totalChance += _sequence[i].Weight;
                return totalChance;
            }
        }
        private int _currentIndex;
        private ITreeComponent[] _sequence;
        public BehaviourTree Tree { get; set; }
        ITreeComponent _runningComponent;
        private List<ITreeComponent> Parents;

        public TreeSequence(params ITreeComponent[] sequence)
        {
            _sequence = sequence;
            _currentIndex = 0;
        }

        public void Execute()
        {
            Execute(default);
        }

        public bool Execute(ITreeComponent caller, Action onComplete = default)
        {
            Execute(onComplete);
            return true;
        }

        private void Execute(Action onComplete)
        {
            //Debug.Log("Executing Sequence!");
            this.OnComplete = onComplete;
            _currentIndex = 0;
            _runningComponent = null;
            ExecuteNext();
        }

        private void ExecuteNext()
        {
            bool success = false;

            if (_currentIndex < _sequence.Length)
            {
                _runningComponent = _sequence[_currentIndex];
                success = _runningComponent.Execute(this, ExecuteNext);
                _currentIndex++;
            }
            else
                Complete();
        }

        public void AddNew(ITreeComponent newComponent)
        {
            ITreeComponent[] temp = (ITreeComponent[]) _sequence.Clone();
            _sequence = new ITreeComponent[_sequence.Length + 1];
            for (int i = 0; i < _sequence.Length - 1; i++)
                _sequence[i] = temp[i];
            _sequence[_sequence.Length - 1] = newComponent;
        }

        // Called when the sequence is over
        public void Complete()
        {
            // Means that there is nothing else
            if (OnComplete == default)
                Tree.SetDone();
            // Means that the sequence is within another sequence
            else
                OnComplete.Invoke();
        }

        public void Setup(BehaviourTree tree, ITreeComponent parent)
        {
            if (Parents == null) Parents = new List<ITreeComponent>(2);
            Parents.Add(parent);

            Tree = tree;
            foreach (ITreeComponent opt in _sequence)
                opt.Setup(tree, this);
        }

        public bool Finalize()
        {
            bool flag = true;
            foreach (ITreeComponent c in _sequence)
            {
                flag = c.Finalize() && flag;
                if (!flag) return flag;
            }
            return flag;
        }

        /// <summary>
        ///
        /// </summary>
        // You were the chosen one!
        public void Kill()
        {
            foreach (ITreeComponent c in _sequence)
                c.Kill();
        }

        public event Action OnComplete;
    }
}