﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModulariaBehaviourTree
{
    public class TreeSequence : ITreeComponent
    {
        public float Weight { get; }
        private int _currentIndex;
        private ITreeComponent[] _sequence;
        public BehaviourTree Tree { get; set; }
        ITreeComponent _runningComponent;
        private List<ITreeComponent> Parents;

        public TreeSequence(float weight, params ITreeComponent[] sequence)
        {
            Weight = weight;
            _sequence = sequence;
            _currentIndex = 0;
        }

        public bool Execute()
        {
            return Execute(default);
        }

        public bool Execute(ITreeComponent caller, Action onComplete = default)
        {
            return Execute(onComplete);
        }


        // Debug purposes
        string name;
        public void SetName(string name)
        {
            this.name = name;
        }

        private bool Execute(Action onComplete)
        {
            //Debug.Log("Executing Sequence!");
            this.OnComplete = onComplete;
            _currentIndex = 0;
            _runningComponent = null;
            ExecuteNext();
            return Condition();
        }

        /// <summary>
        /// Executes the next child in line
        /// </summary>
        private void ExecuteNext()
        {
            bool success = false;

            if (_currentIndex < _sequence.Length)
            {
                _runningComponent = _sequence[_currentIndex];
                success = _runningComponent.Execute(this, ExecuteNext);
                _currentIndex++;
                if (!success) Complete();
            }
            else
                Complete();
        }

        /// <summary>
        /// Adds a new Tree component
        /// </summary>
        /// <param name="newComponent">Component to add</param>
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
        /// Kill all children
        /// </summary>
        // You were the chosen one!
        public void Kill()
        {
            foreach (ITreeComponent c in _sequence)
                c.Kill();
        }

        public bool Condition()
        {
            bool condition = true;
            for (int i = 0; i < _sequence.Length; i++)
            {
                condition = _sequence[0].Condition() && condition;
                if (!condition) break;
            }
            return condition;
        }

        public event Action OnComplete;
    }
}