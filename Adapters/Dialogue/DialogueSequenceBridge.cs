using Retro.Dialogue;
using Retro.QuestSystem.Events;
using UnityEngine;

namespace Retro.QuestSystem.Adapters
{
    /// <summary>Gives Show Dialogue steps the scene's dialogue box and the player's dialogue host. Put it on the <see cref="SequenceDirector"/>.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SequenceDirector))]
    public sealed class DialogueSequenceBridge : MonoBehaviour
    {
        [Tooltip("The scene's dialogue box (under --- UI ---).")]
        [SerializeField] private DialogueBoxUI _dialogueBox;

        [Tooltip("The player's dialogue host (anything implementing IDialogueHost, e.g. FpcDialogueHost).")]
        [SerializeField] private MonoBehaviour _host;

        public DialogueBoxUI DialogueBox => _dialogueBox;
        public IDialogueHost Host => _host as IDialogueHost;

        private void Awake()
        {
            if (_dialogueBox == null || Host == null) Debug.LogError($"[DialogueSequenceBridge] '{name}' needs a Dialogue Box and a Host that implements IDialogueHost.", this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_host != null && !(_host is IDialogueHost))
            {
                Debug.LogWarning($"[DialogueSequenceBridge] '{_host.name}' doesn't implement IDialogueHost.", this);
                _host = null;
            }
        }
#endif
    }
}
