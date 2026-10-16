using System;
using System.Collections;
using Retro.Dialogue;
using Retro.QuestSystem.Events;
using UnityEngine;

namespace Retro.QuestSystem.Adapters
{
    /// <summary>Shows a conversation or inner monologue in the dialogue box and waits until the player dismisses it.</summary>
    [Serializable]
    [SequenceStepMenu("Story/Show Dialogue")]
    public sealed class DialogueStep : SequenceStep
    {
        [Tooltip("The lines to show (an SO_Dialogue_ asset).")]
        [SerializeField] private DialogueDefinition _dialogue;

        public override string Summary => $"Dialogue {NameOf(_dialogue)}";

        public override IEnumerator Run(SequenceContext context)
        {
            DialogueSequenceBridge bridge = context.Director.Get<DialogueSequenceBridge>();
            if (bridge == null)
            {
                Debug.LogWarning($"[DialogueStep] The director has no DialogueSequenceBridge; '{Summary}' skipped.", context.Sequence);
                yield break;
            }

            DialogueBoxUI box = bridge.DialogueBox;
            if (_dialogue == null || box == null || bridge.Host == null) yield break;

            while (box.IsOpen) yield return null;

            bool open = true;
            void HandleClosed() => open = false;
            box.Closed += HandleClosed;
            box.Open(_dialogue, bridge.Host);
            while (open) yield return null;
            box.Closed -= HandleClosed;

            // The box frees the player when it closes; keep them frozen if an earlier step froze them.
            if (context.PlayerLocked) context.SetPlayerLocked(true);
        }
    }
}
