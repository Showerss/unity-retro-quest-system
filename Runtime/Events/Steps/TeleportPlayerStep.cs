using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Moves the player to a marker instantly, facing the way it faces. Pair with a fade or cut to black to hide it, or don't.</summary>
    [Serializable]
    [SequenceStepMenu("Player/Teleport Player")]
    public sealed class TeleportPlayerStep : SequenceStep
    {
        [Tooltip("Where the player stands afterwards: an empty object on the ground, its blue arrow facing where they should look.")]
        [SerializeField] private Transform _marker;

        public override string Summary => $"Teleport player to {NameOf(_marker)}";

        public override IEnumerator Run(SequenceContext context)
        {
            context.Director.TeleportPlayer(_marker);
            yield break;
        }
    }
}
