using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Freezes the player (no moving, looking or interacting) or frees them. They're freed anyway when the sequence ends.</summary>
    [Serializable]
    [SequenceStepMenu("Player/Freeze or Free Player")]
    public sealed class LockPlayerStep : SequenceStep
    {
        [Tooltip("Ticked freezes the player, unticked frees them.")]
        [SerializeField] private bool _freeze = true;

        public override string Summary => _freeze ? "Freeze player" : "Free player";

        public override IEnumerator Run(SequenceContext context)
        {
            context.SetPlayerLocked(_freeze);
            yield break;
        }
    }
}
