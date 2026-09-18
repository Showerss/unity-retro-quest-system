using UnityEngine;

namespace Retro.QuestSystem
{
    public enum TriggerActionType
    {
        CompleteObjective,
        AddProgress,
        StartSpecificQuest,
        StartSpecificChapter
    }

    /// <summary>
    /// Attach to any world trigger volume, interactable, or event to progress quests and chapters.
    /// </summary>
    public class QuestTrigger : MonoBehaviour
    {
        [Header("Action")]
        [SerializeField] private TriggerActionType _actionType = TriggerActionType.CompleteObjective;

        [Tooltip("Objective ID to complete or advance.")]
        [SerializeField] private string _objectiveId = "obj_1";

        [Tooltip("Progress amount to add if using AddProgress.")]
        [SerializeField] private int _progressAmount = 1;

        [Header("Direct Assignment (Optional)")]
        [SerializeField] private QuestDefinition _targetQuest;
        [SerializeField] private ChapterDefinition _targetChapter;

        [Header("Activation Conditions")]
        [Tooltip("Automatically trigger when Player CharacterController / Collider enters.")]
        [SerializeField] private bool _triggerOnPlayerEnter = true;

        [Tooltip("Only trigger once in the scene.")]
        [SerializeField] private bool _triggerOnlyOnce = true;

        private bool _hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!_triggerOnPlayerEnter || (_triggerOnlyOnce && _hasTriggered)) return;

            if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
            {
                ExecuteAction();
            }
        }

        public void ExecuteAction()
        {
            if (_triggerOnlyOnce && _hasTriggered) return;
            _hasTriggered = true;

            if (QuestManager.Instance == null) return;

            switch (_actionType)
            {
                case TriggerActionType.CompleteObjective:
                    QuestManager.Instance.CompleteObjective(_objectiveId);
                    break;

                case TriggerActionType.AddProgress:
                    QuestManager.Instance.AddProgress(_objectiveId, _progressAmount);
                    break;

                case TriggerActionType.StartSpecificQuest:
                    if (_targetQuest != null) QuestManager.Instance.StartQuest(_targetQuest);
                    break;

                case TriggerActionType.StartSpecificChapter:
                    if (_targetChapter != null) QuestManager.Instance.StartChapter(_targetChapter);
                    break;
            }
        }
    }
}
