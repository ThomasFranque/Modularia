using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using Entities.Modularius.ComposedBehaviours;

namespace ModulariaBehaviourTree
{
    public class TreeSelector : ITreeComponent
    {
        public float Weight
        {
            get
            {
                // Create total chance
                float totalChance = 0;
                // Get total
                for (int i = 0; i < _selections.Length; i++)
                    totalChance += _selections[i].next.Weight;
                return totalChance;
            }
        }
        public BehaviourTree Tree { get; set; }
        private(Func<bool> condition, ITreeComponent next) [] _selections;
        private(ITreeComponent next, float chance) [] _selectionChances;
        private bool _randomizedSelector;
        protected List<ITreeComponent> Parents;

        /// <summary>
        /// Constructor for a default selector
        /// </summary>
        /// <param name="weight">The weight of the selector</param>
        /// <param name="condition">next's condition</param>
        /// <param name="next">component to be called if the 
        /// condition checks out</param>
        /// <returns></returns>
        public TreeSelector(params(Func<bool> condition, ITreeComponent next) [] selections)
        {
            _selections = selections;
            SelectionAction = SequentialSelect;
        }

        /// <summary>
        /// Constructor for a random selector that uses weights as a deciding factor
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="options"></param>
        public TreeSelector(params ITreeComponent[] options)
        {
            _selections = new(Func<bool> condition, ITreeComponent next) [options.Length];
            for (int i = 0; i < options.Length; i++)
                _selections[i].next = options[i];
            _randomizedSelector = true;
            SelectionAction = RandomSelect;

            UpdateChances();
        }

        public bool Finalize()
        {
            bool flag = true;
            foreach ((Func<bool> condition, ITreeComponent next) s in _selections)
            {
                flag = s.next.Finalize() && flag;
                if (!flag) return flag;
            }
            return flag;
        }

        public void Setup(BehaviourTree tree, ITreeComponent parent)
        {
            if (Parents == null) Parents = new List<ITreeComponent>(2);
            Parents.Add(parent);

            Tree = tree;
            foreach ((Func<bool> condition, ITreeComponent next) opt in _selections)
                opt.next.Setup(tree, this);
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

            float totalChance = Weight;

            float finalChance = 0;
            // Assign to collection
            for (int i = 0; i < collectionSize; i++)
            {
                // Get final chance
                finalChance += _selections[i].next.Weight / totalChance;
                // Assign
                tempSelectionChances[i].next = _selections[i].next;
                tempSelectionChances[i].chance = finalChance;
            }

            _selectionChances = tempSelectionChances.OrderBy(s => s.chance).ToArray();
            // string final = "";
            // foreach ((ITreeComponent next, float chance) c in _selectionChances)
            //     final += c.chance + " <--> ";
            // Debug.Log(final);
        }

        public bool Execute(ITreeComponent caller, Action OnComplete = default)
        {
            return Execute(OnComplete);
        }
        public void Execute()
        {
            Execute(default);
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
        /// Adds a new option to a randomized selector
        /// </summary>
        /// <param name="newOption">new option</param>
        public void AddNewOption(ITreeComponent newOption)
        {
            if (!_randomizedSelector)
            {
                Debug.LogError("ADDING NO CONDITION OPTIONS TO A RANDOMIZED SELECTOR IS NOT PERMITTED.");
                return;
            }
            (Func<bool> condition, ITreeComponent next) [] temp =
                ((Func<bool> condition, ITreeComponent next) []) _selections.Clone();
            _selections =
                new(Func<bool> condition, ITreeComponent next) [_selections.Length + 1];
            for (int i = 0; i < _selections.Length - 1; i++)
                _selections[i].next = temp[i].next;
            _selections[_selections.Length - 1].next = newOption;
            UpdateChances();
        }

        /// <summary>
        /// Adds a new option to a default selector
        /// </summary>
        /// <param name="condition">condition</param>
        /// <param name="next">component to be called</param>
        public void AddNewOption((Func<bool> condition, ITreeComponent next) newOption)
        {
            if (_randomizedSelector)
            {
                Debug.LogError("ADDING CONDITION OPTIONS TO A NON RANDOMIZED SELECTOR IS NOT PERMITTED.");
                return;
            }
            (Func<bool> condition, ITreeComponent next) [] temp =
                ((Func<bool> condition, ITreeComponent next) []) _selections.Clone();
            _selections =
                new(Func<bool> condition, ITreeComponent next) [_selections.Length + 1];
            for (int i = 0; i < _selections.Length - 1; i++)
                _selections[i] = temp[i];
            _selections[_selections.Length - 1] = newOption;
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
                if (_selections[i].condition.Invoke())
                {
                    executed = _selectionChances[i].next.Execute(this, OnComplete);
                    // Selector fail safe
                    if (executed)
                        break;
                }
            }
            OnSelectionMade(executed);
            return executed;
        }

        /// <summary>
        /// Linearly select one, trying every option
        /// </summary>
        /// <returns>True if the selection was successful</returns>
        private bool SequentialSelect()
        {
            bool executed = false;
            for (int i = 0; i < _selections.Length; i++)
            {
                if (_selections[i].condition.Invoke())
                {
                    executed = _selections[i].next.Execute(this, OnComplete);
                    if (executed)
                        break;
                }
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
        /// 
        /// </summary>
        // Kill all the children and join the dark side
        public void Kill()
        {
            for (int i = 0; i < _selections.Length; i++)
                _selections[i].next.Kill();
        }

        private Func<bool> SelectionAction;
        public event Action OnComplete;
    }
}