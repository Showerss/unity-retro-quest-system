using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>
    /// One thing a <see cref="GameSequence"/> does, such as wait, play a sound or show dialogue.
    /// To add a new kind of step, derive from this, mark it [Serializable] and give it a [SequenceStepMenu] path.
    /// It then shows up in the sequence's Add Step menu.
    /// </summary>
    [Serializable]
    public abstract class SequenceStep
    {
        [Tooltip("Start this step and go straight on to the next one, so the two happen together.")]
        [SerializeField] private bool _dontWait;

        public bool DontWait => _dontWait;

        /// <summary>One line describing what the step does, shown as its title in the inspector.</summary>
        public abstract string Summary { get; }

        /// <summary>Does the step. The sequence waits for this to finish unless Don't Wait is ticked.</summary>
        public abstract IEnumerator Run(SequenceContext context);

        protected static string NameOf(UnityEngine.Object obj) => obj != null ? obj.name : "(none)";
    }
}
