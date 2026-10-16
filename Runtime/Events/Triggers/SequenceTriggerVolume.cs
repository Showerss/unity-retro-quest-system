using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Plays a sequence when the player walks into this trigger collider. Who counts as the player is up to the director's <see cref="ISequencePlayer"/>.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public sealed class SequenceTriggerVolume : MonoBehaviour
    {
        private static readonly Color GizmoFill = new Color(1f, 0.55f, 0f, 0.15f);
        private static readonly Color GizmoEdge = new Color(1f, 0.55f, 0f, 0.8f);

        [Tooltip("The sequence to play.")]
        [SerializeField] private GameSequence _sequence;

        private void Awake()
        {
            if (_sequence == null) Debug.LogError($"[SequenceTriggerVolume] '{name}' has no sequence.", this);
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_sequence == null || _sequence.Director == null) return;
            ISequencePlayer player = _sequence.Director.Player;
            if (player != null && player.IsPlayer(other)) _sequence.Play();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!TryGetComponent(out BoxCollider box)) return;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = GizmoFill;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = GizmoEdge;
            Gizmos.DrawWireCube(box.center, box.size);
        }
#endif
    }
}
