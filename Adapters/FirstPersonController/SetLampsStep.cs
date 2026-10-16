using System;
using System.Collections;
using FirstPerson.Interaction;
using Retro.QuestSystem.Events;
using UnityEngine;

namespace Retro.QuestSystem.Adapters
{
    /// <summary>Lights or puts out lamps (anything with a <see cref="LightSwitch"/>), flame meshes included.</summary>
    [Serializable]
    [SequenceStepMenu("Lights/Set Lamps")]
    public sealed class SetLampsStep : SequenceStep
    {
        [Tooltip("Lamps to change.")]
        [SerializeField] private LightSwitch[] _lamps = new LightSwitch[0];

        [Tooltip("Ticked lights them, unticked puts them out.")]
        [SerializeField] private bool _lit;

        public override string Summary => $"{(_lit ? "Light" : "Put out")} {_lamps?.Length ?? 0} lamp(s)";

        public override IEnumerator Run(SequenceContext context)
        {
            foreach (LightSwitch lamp in _lamps)
            {
                if (lamp != null) lamp.SetLit(_lit);
            }
            yield break;
        }
    }
}
