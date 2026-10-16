using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Shows or hides objects: a figure that appears, a room that's suddenly different, a blocked doorway.</summary>
    [Serializable]
    [SequenceStepMenu("World/Show or Hide Objects")]
    public sealed class SetActiveStep : SequenceStep
    {
        [Tooltip("Objects to show or hide.")]
        [SerializeField] private GameObject[] _objects = new GameObject[0];

        [Tooltip("Ticked shows them, unticked hides them.")]
        [SerializeField] private bool _show = true;

        public override string Summary => $"{(_show ? "Show" : "Hide")} {Describe()}";

        public override IEnumerator Run(SequenceContext context)
        {
            foreach (GameObject obj in _objects)
            {
                if (obj != null) obj.SetActive(_show);
            }
            yield break;
        }

        private string Describe()
        {
            if (_objects == null || _objects.Length == 0) return "(nothing)";
            return _objects.Length == 1 ? NameOf(_objects[0]) : $"{_objects.Length} objects";
        }
    }
}
