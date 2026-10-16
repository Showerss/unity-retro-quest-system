using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Records something about the story, e.g. "found the letter", or counts something up.</summary>
    [Serializable]
    [SequenceStepMenu("Flow/Set Flag")]
    public sealed class SetFlagStep : SequenceStep
    {
        public enum Operation { Set, Add }

        [Tooltip("The flag to change.")]
        [SerializeField] private FlagDefinition _flag;

        [Tooltip("Set replaces the value; Add adds to it (use a negative number to subtract).")]
        [SerializeField] private Operation _operation = Operation.Set;

        [Tooltip("1 = set, 0 = cleared. Any number for counters.")]
        [SerializeField] private int _value = 1;

        public override string Summary => _operation == Operation.Set
            ? $"Set {NameOf(_flag)} = {_value}"
            : $"Add {_value} to {NameOf(_flag)}";

        public override IEnumerator Run(SequenceContext context)
        {
            if (_operation == Operation.Set) GameFlags.Set(_flag, _value);
            else GameFlags.Add(_flag, _value);
            yield break;
        }
    }
}
