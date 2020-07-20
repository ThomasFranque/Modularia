using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration.Individuals
{
    public class Branch
    {
        private Branch _originatedBranch;
        private Branch _mainBranch;
        private List<Room> _rooms;
        private List<Branch> _branches;
        public bool IsMain { get; }

        public Branch()
        {
            IsMain = true;
            _branches = new List<Branch>();
        }

        public Branch(Branch originatedBranch, Branch mainBranch)
        {
            IsMain = false;
            _originatedBranch = originatedBranch;
            _branches = new List<Branch>();
        }

        public void AddNewBranch(Branch newBranch)
        {
            _branches.Add(newBranch);
        }
    }
}