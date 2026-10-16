using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Glides an object to a marker's position and rotation: a door creeping open, a figure stepping out, a painting turning.</summary>
    [Serializable]
    [SequenceStepMenu("World/Move Object")]
    public sealed class MoveObjectStep : SequenceStep
    {
        [Tooltip("The object to move.")]
        [SerializeField] private Transform _target;

        [Tooltip("Where it ends up. An empty object placed and rotated where you want it.")]
        [SerializeField] private Transform _destination;

        [Tooltip("How long the move takes, in seconds. 0 snaps it there.")]
        [Min(0f)]
        [SerializeField] private float _seconds = 1f;

        [Tooltip("Ease in and out instead of moving at a constant speed.")]
        [SerializeField] private bool _smooth = true;

        public override string Summary => $"Move {NameOf(_target)} to {NameOf(_destination)} over {_seconds:0.##}s";

        public override IEnumerator Run(SequenceContext context)
        {
            if (_target == null || _destination == null) yield break;

            Vector3 startPosition = _target.position;
            Quaternion startRotation = _target.rotation;
            for (float t = 0f; t < _seconds; t += Time.deltaTime)
            {
                float k = t / _seconds;
                if (_smooth) k = Mathf.SmoothStep(0f, 1f, k);
                _target.SetPositionAndRotation(
                    Vector3.Lerp(startPosition, _destination.position, k),
                    Quaternion.Slerp(startRotation, _destination.rotation, k));
                yield return null;
            }
            _target.SetPositionAndRotation(_destination.position, _destination.rotation);
        }
    }
}
