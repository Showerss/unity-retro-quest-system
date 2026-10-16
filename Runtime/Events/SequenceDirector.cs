using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>
    /// The scene's shared pieces that sequences drive. One per gameplay scene, under --- SYSTEMS ---;
    /// every <see cref="GameSequence"/> in the scene points at it.
    /// The player and the black screen come from an <see cref="ISequencePlayer"/> and <see cref="ISequenceFader"/> on this object
    /// (the adapters supply them, e.g. FpcSequenceBridge); steps find anything else with <see cref="Get{T}"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SequenceDirector : MonoBehaviour
    {
        [Tooltip("Plays sounds that aren't placed in the world (stingers, voices in your head). Should be 2D (Spatial Blend 0).")]
        [SerializeField] private AudioSource _audioSource;

        public AudioSource AudioSource => _audioSource;

        /// <summary>The player, or null if no component on this object provides one.</summary>
        public ISequencePlayer Player { get; private set; }

        /// <summary>The black screen, or null if no component on this object provides one.</summary>
        public ISequenceFader Fader { get; private set; }

        private void Awake()
        {
            Player = GetComponent<ISequencePlayer>();
            Fader = GetComponent<ISequenceFader>();
            if (_audioSource == null) Debug.LogError($"[SequenceDirector] '{name}' has no Audio Source.", this);
            if (Player == null) Debug.LogWarning($"[SequenceDirector] '{name}' has no player bridge (e.g. FpcSequenceBridge); player steps and trigger volumes won't work.", this);
            if (Fader == null) Debug.LogWarning($"[SequenceDirector] '{name}' has no fader bridge (e.g. FpcSequenceBridge); fades will be skipped.", this);
        }

        /// <summary>A service component on this object, e.g. the dialogue adapter's DialogueSequenceBridge. Null if missing.</summary>
        public T Get<T>() where T : class => GetComponent<T>();

        /// <summary>Stops (true) or restores (false) the player's movement, look and interaction.</summary>
        public void SetPlayerLocked(bool locked) => Player?.SetLocked(locked);

        /// <summary>Puts the player standing on <paramref name="marker"/>'s position, facing its forward direction.</summary>
        public void TeleportPlayer(Transform marker)
        {
            if (marker != null) Player?.Teleport(marker);
        }
    }
}
