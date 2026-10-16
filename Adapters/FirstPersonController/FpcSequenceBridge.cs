using System.Collections;
using FirstPerson.Interaction;
using FirstPerson.UI;
using Retro.QuestSystem.Events;
using UnityEngine;

namespace Retro.QuestSystem.Adapters
{
    /// <summary>
    /// Gives sequences the first-person player and the screen fader. Put it on the <see cref="SequenceDirector"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SequenceDirector))]
    public sealed class FpcSequenceBridge : MonoBehaviour, ISequencePlayer, ISequenceFader
    {
        [Tooltip("The player's controller; frozen and teleported by sequence steps.")]
        [SerializeField] private FirstPersonController _player;

        [Tooltip("Full-screen black overlay (under --- UI ---).")]
        [SerializeField] private ScreenFader _fader;

        private void Awake()
        {
            if (_player == null || _fader == null) Debug.LogError($"[FpcSequenceBridge] '{name}' is missing a reference.", this);
        }

        public void SetLocked(bool locked) => PlayerLock.Set(_player, locked);

        public void Teleport(Transform marker)
        {
            if (_player == null || marker == null) return;
            _player.Teleport(marker.position + Vector3.up * PlayerLock.FeetToPivot(_player), marker.eulerAngles.y);
        }

        public bool IsPlayer(Collider other) => _player != null && other.GetComponentInParent<FirstPersonController>() == _player;

        public IEnumerator FadeTo(float darkness, float seconds)
        {
            if (_fader != null) yield return _fader.FadeTo(darkness, seconds);
        }
    }
}
