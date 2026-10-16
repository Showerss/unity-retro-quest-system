using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Plays one sequence if the flags say so, otherwise another. How the story reacts to what the player has done.</summary>
    [Serializable]
    [SequenceStepMenu("Flow/If Flags")]
    public sealed class BranchStep : SequenceStep
    {
        [Tooltip("Every condition must be true to play Then; otherwise Else plays.")]
        [SerializeField] private FlagCondition[] _conditions = new FlagCondition[0];

        [Tooltip("Played when the conditions are true. Leave empty to do nothing.")]
        [SerializeField] private GameSequence _then;

        [Tooltip("Played when they aren't. Leave empty to do nothing.")]
        [SerializeField] private GameSequence _else;

        public override string Summary
        {
            get
            {
                string test = _conditions == null || _conditions.Length == 0 ? "always" : string.Join(" and ", _conditions);
                return $"If {test} → {NameOf(_then)}, else → {NameOf(_else)}";
            }
        }

        public override IEnumerator Run(SequenceContext context)
        {
            GameSequence chosen = FlagCondition.AllMet(_conditions) ? _then : _else;
            if (chosen == null || !chosen.TryPlay()) yield break;
            while (chosen.IsPlaying) yield return null;
        }
    }
}
