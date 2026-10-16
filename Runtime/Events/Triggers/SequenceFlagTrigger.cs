using System.Collections.Generic;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>
    /// Plays a sequence as soon as the story flags say so, e.g. once all three letters are found, or when the bell has rung twice.
    /// Also checks when the scene starts, and whenever <see cref="CheckAll"/> is called (e.g. after a save loads).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SequenceFlagTrigger : MonoBehaviour
    {
        [Tooltip("The sequence to play.")]
        [SerializeField] private GameSequence _sequence;

        [Tooltip("Every condition must be true. Checked whenever a flag changes.")]
        [SerializeField] private FlagCondition[] _conditions = new FlagCondition[0];

        private static readonly List<SequenceFlagTrigger> _enabled = new List<SequenceFlagTrigger>();

        /// <summary>Re-checks every enabled trigger, e.g. once a loaded save has been fully applied.</summary>
        public static void CheckAll()
        {
            // Copied: a sequence that plays could enable or disable other triggers.
            foreach (SequenceFlagTrigger trigger in _enabled.ToArray()) trigger.Check();
        }

        private void Awake()
        {
            if (_sequence == null) Debug.LogError($"[SequenceFlagTrigger] '{name}' has no sequence.", this);
        }

        private void OnEnable()
        {
            GameFlags.Changed += HandleFlagChanged;
            _enabled.Add(this);
        }

        private void OnDisable()
        {
            GameFlags.Changed -= HandleFlagChanged;
            _enabled.Remove(this);
        }

        private void Start()
        {
            Check();
        }

        private void HandleFlagChanged(string id, int value)
        {
            Check();
        }

        private void Check()
        {
            if (_sequence != null && FlagCondition.AllMet(_conditions)) _sequence.Play();
        }
    }
}
