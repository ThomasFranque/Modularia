using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModulariaBehaviourTree
{
    public class BehaviourTree : MonoBehaviour
    {
        private const string ERROR_MSG =
            "The created tree is not valid!\nCheck if every component ends in a leaf.";
        private ITreeComponent _main;
        private ITreeComponent _currentRunning;
        private bool _running;
        // This bool is used to flag the start method
        private bool _runIssuedBeforeStart;
        private bool _startIssued;

        public ITreeComponent CurrentRunning => _currentRunning;
        
        public void Initialize(TreeSelector main) =>
            Initialize(main as ITreeComponent);
        public void Initialize(TreeSequence main) =>
            Initialize(main as ITreeComponent);
        private void Initialize(ITreeComponent main)
        {
            _main = main;
            _main.Setup(this, default);
            if (!_main.Finalize()) throw new System.Exception(ERROR_MSG);

            if (_startIssued)
                Run();
            else
                _runIssuedBeforeStart = true;
        }

        private void Start()
        {
            _startIssued = true;
            if (_runIssuedBeforeStart) Run();
        }

        public void SetDone() => OnSetDone();
        public void Kill()
        {
            _main.Kill();
            Debug.Log("Running tree killed.");
        }

        public void SetCurrentRunning(ITreeComponent current)
        {
            _currentRunning = current;
        }

        private void OnSetDone()
        {
            // Debug.Log("Tree done.");
            _currentRunning = null;
            StartCoroutine(RunNextFrame());
        }
        private void Run() => _main.Execute();

        // From https://forum.unity.com/threads/how-to-wait-for-a-frame-in-c.24616/
        IEnumerator RunNextFrame()
        {
            yield return 0;
            Run();
        }
    }
}