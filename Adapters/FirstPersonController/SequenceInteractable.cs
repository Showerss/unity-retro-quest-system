using FirstPerson.Interaction;
using Retro.QuestSystem.Events;
using UnityEngine;

namespace Retro.QuestSystem.Adapters
{
    /// <summary>
    /// Plays a sequence when the player interacts with this object (press E): examine a photo, ring a bell, open a cupboard.
    /// The prompt disappears while the sequence can't play.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SequenceInteractable : MonoBehaviour, IInteractable
    {
        [Tooltip("The sequence to play.")]
        [SerializeField] private GameSequence _sequence;

        [Tooltip("Prompt shown while looking at it. The HUD adds the key.")]
        [SerializeField] private string _promptText = "Examine";

        private InteractionPrompt _prompt;

        private void Awake()
        {
            if (_sequence == null) Debug.LogError($"[SequenceInteractable] '{name}' has no sequence.", this);
            _prompt = new InteractionPrompt(_promptText);
        }

        public InteractionPrompt GetPrompt(PlayerInteraction interactor) =>
            _sequence != null && _sequence.CanPlay ? _prompt : InteractionPrompt.None;

        public void Interact(PlayerInteraction interactor)
        {
            if (_sequence != null) _sequence.Play();
        }
    }
}
