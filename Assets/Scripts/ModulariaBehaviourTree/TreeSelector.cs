﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using Entities.Modularius.ComposedBehaviours;

namespace ModulariaBehaviourTree
{
    public class TreeSelector : ITreeComponent
    {
        public float Weight { get; }
        public BehaviourTree Tree { get; set; }
        private ITreeComponent[] _selections;
        private(ITreeComponent next, float chance) [] _selectionChances;
        private bool _randomizedSelector;
        protected List<ITreeComponent> Parents;

        /// <summary>
        /// Constructor for selector
        /// </summary>
        /// <param name="options"></param>
        public TreeSelector(bool randomized, float weight, params ITreeComponent[] options)
        {
            _selections = new ITreeComponent[options.Length];
            Weight = weight;
            for (int i = 0; i < options.Length; i++)
                _selections[i] = options[i];

            if (randomized)
            {
                _randomizedSelector = true;
                SelectionAction = RandomSelect;

                UpdateChances();
            }
            else
            {
                SelectionAction = SequentialSelect;
            }
        }
        public bool Finalize()
        {
            bool flag = true;
            foreach (ITreeComponent s in _selections)
            {
                flag = s.Finalize() && flag;
                if (!flag) return flag;
            }
            return flag;
        }

        public void Setup(BehaviourTree tree, ITreeComponent parent)
        {
            if (Parents == null) Parents = new List<ITreeComponent>(2);
            Parents.Add(parent);

            Tree = tree;
            foreach (ITreeComponent opt in _selections)
                opt.Setup(tree, this);
        }

        /// <summary>
        /// Updates the chances to a collection, from lowest to highest in index order
        // using the given weight, to be used linearly when selecting
        /// </summary>
        private void UpdateChances()
        {
            // Get size
            int collectionSize = _selections.Length;

            // Set/Reset chances collection
            _selectionChances = new(ITreeComponent next, float chance) [collectionSize];

            // Create a temporary collection
            (ITreeComponent next, float chance) [] tempSelectionChances =
                new(ITreeComponent next, float chance) [collectionSize];

            float totalChance = 0;
            // Get total chance
            for (int i = 0; i < collectionSize; i++)
                totalChance += _selections[i].Weight;

            float finalChance = 0;
            // Assign to collection
            // and Get final chance
            for (int i = 0; i < collectionSize; i++)
            {
                finalChance += _selections[i].Weight / totalChance;
                // Assign
                tempSelectionChances[i].next = _selections[i];
                tempSelectionChances[i].chance = finalChance;
            }

            _selectionChances = tempSelectionChances.OrderBy(s => s.chance).ToArray();
            // string final = "";
            // foreach ((ITreeComponent next, float chance) c in _selectionChances)
            //     final += c.chance + " <--> ";
            // Debug.Log(final);
        }

        public bool Execute()
        {
            return Execute(default);
        }

        public bool Execute(ITreeComponent caller, Action OnComplete = default)
        {
            return Execute(OnComplete);
        }

        private bool Execute(Action onComplete)
        {
            //Debug.Log("Executing Selector!");
            OnComplete = onComplete;
            return SelectionAction.Invoke();
        }

        /// <summary>
        /// Complete is empty due to the selector being just a passage branching
        /// And a tree should not end here. In case that happens, OnSelectionMade()
        /// handles that situation.
        /// </summary>
        public void Complete()
        {
            //OnComplete?.Invoke();
        }

        /// <summary>
        /// Adds a new option to the selector
        /// </summary>
        /// <param name="newOption">new option</param>
        public void AddNewOption(ITreeComponent newOption)
        {
            ITreeComponent[] temp =
                (ITreeComponent[]) _selections.Clone();
            _selections =
                new ITreeComponent[_selections.Length + 1];
            for (int i = 0; i < _selections.Length - 1; i++)
                _selections[i] = temp[i];
            _selections[_selections.Length - 1] = newOption;

            if (_randomizedSelector)
                UpdateChances();
        }

        /// <summary>
        /// Randomly select one using weights, only tries once
        /// </summary>
        /// <returns>True if the selection was successful</returns>
        private bool RandomSelect()
        {
            bool executed = false;
            // 0.999999f due to float imperfections
            // if 1 was used, there was a 0.00000001% chance of it failing.
            // I don't want to take that risk :)
            float roll = Random.Range(0.0f, 0.999999f);
            for (int i = 0; i < _selectionChances.Length; i++)
            {
                if (_selectionChances[i].chance >= roll)
                    executed = _selectionChances[i].next.Execute(this, OnComplete);
                // Selector fail safe
                if (executed)
                    break;

            }
            OnSelectionMade(executed);
            return executed;
        }

        /// <summary>
        /// Linearly select one, trying every option on the way
        /// </summary>
        /// <returns>True if the selection was successful</returns>
        private bool SequentialSelect()
        {
            bool executed = false;
            for (int i = 0; i < _selections.Length; i++)
            {
                executed = _selections[i].Execute(this, OnComplete);
                if (executed)
                    break;

            }
            OnSelectionMade(executed);
            return executed;
        }

        /// <summary>
        /// On any selection made
        /// </summary>
        /// <param name="success">Was the selection successful</param>
        private void OnSelectionMade(bool success)
        {
            // In case every choice fails and there is nothing else to do
            if (!success)
            {
                // In case it was called by a sequence 
                OnComplete?.Invoke();
            }
            // Else do nothing
            Complete();
        }

        /// <summary>
        /// Kills all of the children
        /// </summary>
        // Kill all the children and join the dark side
        public void Kill()
        {
            for (int i = 0; i < _selections.Length; i++)
                _selections[i].Kill();
        }

        public bool Condition() => true;

        private Func<bool> SelectionAction;
        public event Action OnComplete;
    }
}