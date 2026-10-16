using System;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>A test against one story flag, e.g. "found_letter is set" or "bell_rings is at least 3".</summary>
    [Serializable]
    public struct FlagCondition
    {
        public enum Comparison { IsSet, IsNotSet, Equals, AtLeast, LessThan }

        [Tooltip("The flag to test.")]
        [SerializeField] private FlagDefinition _flag;

        [Tooltip("How to test it. Equals, At Least and Less Than compare against Value.")]
        [SerializeField] private Comparison _comparison;

        [Tooltip("Number compared against for Equals, At Least and Less Than.")]
        [SerializeField] private int _value;

        public bool IsMet()
        {
            int current = GameFlags.Get(_flag);
            switch (_comparison)
            {
                case Comparison.IsSet: return current != 0;
                case Comparison.IsNotSet: return current == 0;
                case Comparison.Equals: return current == _value;
                case Comparison.AtLeast: return current >= _value;
                case Comparison.LessThan: return current < _value;
                default: return false;
            }
        }

        public override string ToString()
        {
            string flag = _flag != null ? _flag.name : "(no flag)";
            switch (_comparison)
            {
                case Comparison.IsSet: return $"{flag} is set";
                case Comparison.IsNotSet: return $"{flag} is not set";
                case Comparison.Equals: return $"{flag} = {_value}";
                case Comparison.AtLeast: return $"{flag} >= {_value}";
                default: return $"{flag} < {_value}";
            }
        }

        /// <summary>True when every condition is met (or there are none).</summary>
        public static bool AllMet(FlagCondition[] conditions)
        {
            if (conditions == null) return true;
            foreach (FlagCondition condition in conditions)
            {
                if (!condition.IsMet()) return false;
            }
            return true;
        }
    }
}
