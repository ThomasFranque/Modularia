using System;

namespace ModulariaBehaviourTree
{
    public interface ITreeComponent
    {
        /// <summary>
        /// Executes behaviour
        /// </summary>
        /// <param name="caller">Who called it</param>
        /// <returns>True if Executed successfully</returns>
        bool Execute(ITreeComponent caller, Action OnComplete = default);
        /// <summary>
        /// Called by the behaviour tree in case it is given as the main Component
        /// </summary>
        bool Execute();
        /// <summary>
        /// Used by the tree leafs to tell the tree that they are done
        /// </summary>
        void Complete();
        /// <summary>
        /// Used by random selectors to determine the chances
        /// </summary>
        /// <value>Chance, relative to others, of getting picked</value>
        float Weight { get; }
        /// <summary>
        /// Value to be set by the behaviour tree
        /// </summary>
        /// <value></value>
        BehaviourTree Tree { get; set; }
        /// <summary>
        /// Used to set the tree parameter and set parents, also allows to check
        /// if the tree is valid
        /// </summary>
        void Setup(BehaviourTree tree, ITreeComponent parent);
        /// <summary>
        /// This is called after setup and finalizes tree creation, returning 
        /// controling if the tree is possible
        /// </summary>
        /// <returns>true if the tree can run</returns>
        bool Finalize();
        /// <summary>
        /// Kills the behaviour if it is running
        /// </summary>
        void Kill();
        /// <summary>
        /// Condition required to perform
        /// </summary>
        /// <returns>Condition met</returns>
        bool Condition();
        /// <summary>
        /// On completion, this is called, allowing for sequences to know when
        /// they should move on
        /// </summary>
        event Action OnComplete;
    }
}