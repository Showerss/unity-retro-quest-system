using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Pauses the sequence. Game time, so it also stops while the game is paused.</summary>
    [Serializable]
    [SequenceStepMenu("Flow/Wait")]
    public sealed class WaitStep : SequenceStep
    {
        [Tooltip("How long to wait, in seconds.")]
        [Min(0f)]
        [SerializeField] private float _seconds = 1f;

        public override string Summary => $"Wait {_seconds:0.##}s";

        public override IEnumerator Run(SequenceContext context)
        {
            yield return new WaitForSeconds(_seconds);
        }
    }
}
