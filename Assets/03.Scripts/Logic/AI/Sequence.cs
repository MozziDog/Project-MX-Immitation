using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AI
{
    public class Sequence : BehaviorNode
    {
        private List<BehaviorNode> _behaviors = new List<BehaviorNode>();

        public Sequence(params BehaviorNode[] behaviors)
        {
            for (int i = 0; i < behaviors.Length; i++)
            {
                _behaviors.Add(behaviors[i]);
            }
        }

        public void Add(BehaviorNode behavior)
        {
            _behaviors.Add(behavior);
        }

        public override BehaviorResult Behave()
        {
            if (_behaviors == null || _behaviors.Count == 0) { return BehaviorResult.Failure; }

            for (int i = 0; i < _behaviors.Count; i++)
            {
                BehaviorResult childResult = _behaviors[i].Behave();
                if (childResult == BehaviorResult.Success)
                    continue;
                else
                    return childResult;
            }
            return BehaviorResult.Success;  // 기본값 Success
        }
    }
}