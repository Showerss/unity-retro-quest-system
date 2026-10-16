using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Retro.QuestSystem.Events
{
    /// <summary>Calls any method on any object, for things no other step covers (an Animator trigger, a particle system...).</summary>
    [Serializable]
    [SequenceStepMenu("Flow/Call Event")]
    public sealed class UnityEventStep : SequenceStep
    {
        [Tooltip("A short note on what this does, shown as the step's title.")]
        [SerializeField] private string _label = "Call event";

        [Tooltip("Methods called when the step runs.")]
        [SerializeField] private UnityEvent _event = new UnityEvent();

        public override string Summary => _label;

        public override IEnumerator Run(SequenceContext context)
        {
            _event.Invoke();
            yield break;
        }
    }
}
