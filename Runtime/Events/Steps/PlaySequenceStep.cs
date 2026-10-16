using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Plays another sequence, so a common chunk (say, the lights dying) can be reused by several events.</summary>
    [Serializable]
    [SequenceStepMenu("Flow/Play Sequence")]
    public sealed class PlaySequenceStep : SequenceStep
    {
        [Tooltip("The sequence to play. Its own conditions and Play Once still apply.")]
        [SerializeField] private GameSequence _sequence;

        public override string Summary => $"Play {NameOf(_sequence)}";

        public override IEnumerator Run(SequenceContext context)
        {
            if (_sequence == null || !_sequence.TryPlay()) yield break;
            while (_sequence.IsPlaying) yield return null;
        }
    }
}
