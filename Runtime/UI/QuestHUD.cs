using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

namespace Retro.QuestSystem
{
    public class QuestHUD : MonoBehaviour
    {
        [Header("UI Header Elements")]
        [SerializeField] private TextMeshProUGUI _chapterBadgeText;
        [SerializeField] private TextMeshProUGUI _questTitleText;
        [SerializeField] private RectTransform _objectivesContainer;

        [Header("Row Template")]
        [Tooltip("Inactive template with a QuestHUDRow and explicit checkbox, description and ink references.")]
        [SerializeField] private GameObject _rowTemplate;

        [Header("Visual Styling (90s Retro)")]
        [SerializeField] private Color _activeObjectiveColor = new Color(0.95f, 0.95f, 0.95f, 1f);
        [SerializeField] private Color _completedObjectiveColor = new Color(0.45f, 0.85f, 0.45f, 0.85f);
        [SerializeField] private Color _checkboxColor = new Color(0.95f, 0.75f, 0.2f, 1f);
        [SerializeField] private Color _chapterBadgeColor = new Color(0.95f, 0.75f, 0.2f, 1f);

        [Header("Audio Feedback")]
        [SerializeField] private AudioClip _objectiveCompleteSound;
        [SerializeField] private AudioClip _questCompleteSound;
        [SerializeField] private AudioSource _audioSource;

        [Header("Completion Presentation")]
        [Tooltip("Disable to show the current quest state immediately, without completion motion or holds.")]
        [SerializeField] private bool _animateCompletion = true;
        [Tooltip("Unscaled seconds to draw the completion ink stroke.")]
        [Min(0f)] [SerializeField] private float _completionDrawDuration = 0.18f;
        [Tooltip("Unscaled seconds to hold the completed appearance before revealing the next objective.")]
        [FormerlySerializedAs("_delayBeforeNextObjective")]
        [Min(0f)] [SerializeField] private float _completionHoldDuration = 0.45f;

        private readonly List<QuestHUDRow> _rows = new List<QuestHUDRow>();
        private readonly Queue<QuestHUDRow> _completions = new Queue<QuestHUDRow>();
        private QuestManager _manager;
        private QuestDefinition _presentedQuest;
        private bool _displayDirty;

        public bool AnimateCompletion
        {
            get => _animateCompletion;
            set
            {
                _animateCompletion = value;
                if (!value) CancelCelebrations();
                RefreshDisplay();
            }
        }

        private void Awake()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (_rowTemplate != null) _rowTemplate.SetActive(false);
        }

        private void OnEnable()
        {
            if (QuestManager.Instance != null) BindManager(QuestManager.Instance);
        }

        private void OnDisable()
        {
            if (_manager != null)
            {
                _manager.OnChapterStarted -= HandleChapterStarted;
                _manager.OnQuestStarted -= HandleQuestStarted;
                _manager.OnObjectiveUpdated -= HandleObjectiveUpdated;
                _manager.OnObjectiveCompleted -= HandleObjectiveCompleted;
                _manager.OnQuestCompleted -= HandleQuestCompleted;
            }

            _manager = null;
            CancelCelebrations();
            foreach (var row in _rows) row.gameObject.SetActive(false);
        }

        private void BindManager(QuestManager manager)
        {
            if (_manager == manager) return;
            _manager = manager;
            _manager.OnChapterStarted += HandleChapterStarted;
            _manager.OnQuestStarted += HandleQuestStarted;
            _manager.OnObjectiveUpdated += HandleObjectiveUpdated;
            _manager.OnObjectiveCompleted += HandleObjectiveCompleted;
            _manager.OnQuestCompleted += HandleQuestCompleted;
            CancelCelebrations();
            RefreshDisplay();
        }

        private void LateUpdate()
        {
            if (_manager == null)
            {
                if (QuestManager.Instance != null) BindManager(QuestManager.Instance);
                return;
            }

            AdvancePresentation(Time.unscaledDeltaTime);
        }

        private void AdvancePresentation(float deltaTime)
        {
            if (!_animateCompletion) CancelCelebrations();

            if (_presentedQuest != null && _presentedQuest.displayMode == ObjectiveDisplayMode.SequentialOnly)
            {
                if (_completions.Count > 0 && _completions.Peek().AdvanceCompletion(deltaTime))
                {
                    _completions.Dequeue();
                    _displayDirty = true;
                }
            }
            else
            {
                foreach (var row in _rows)
                {
                    if (row.IsCelebrating && row.AdvanceCompletion(deltaTime)) _displayDirty = true;
                }
                _completions.Clear();
            }

            // ObjectiveUpdated is emitted before the manager advances its index.
            if (_displayDirty) RefreshDisplay();
        }

        private void HandleChapterStarted(ChapterDefinition chapter)
        {
            CancelCelebrations();
            _displayDirty = true;
        }

        private void HandleQuestStarted(QuestDefinition quest)
        {
            CancelCelebrations();
            _displayDirty = true;
        }

        private void HandleObjectiveUpdated(QuestObjective objective) => _displayDirty = true;

        private void HandleObjectiveCompleted(QuestObjective objective)
        {
            PlaySound(objective.completionSound != null ? objective.completionSound : _objectiveCompleteSound);
            RefreshDisplay();
            if (!_animateCompletion || _presentedQuest == null) return;

            int index = _presentedQuest.objectives.IndexOf(objective);
            if (index < 0 || index >= _rows.Count) return;

            var row = _rows[index];
            if (row.IsCelebrating) return;
            row.CaptureCompletion(_completionDrawDuration, _completionHoldDuration);
            if (row.IsCelebrating && _presentedQuest.displayMode == ObjectiveDisplayMode.SequentialOnly)
            {
                _completions.Enqueue(row);
            }
            _displayDirty = true;
        }

        private void HandleQuestCompleted(QuestDefinition quest)
        {
            PlaySound(quest.questCompleteSound != null ? quest.questCompleteSound : _questCompleteSound);
        }

        // The legacy parameter remains source-compatible; refresh never invents completion feedback.
        public void RefreshDisplay(bool showActiveObjectiveCompleted = false)
        {
            _displayDirty = false;
            if (_manager == null) return;

            var chapter = _manager.ActiveChapter;
            var quest = _manager.ActiveQuest;
            if (_presentedQuest != quest)
            {
                CancelCelebrations();
                _presentedQuest = quest;
            }

            if (_chapterBadgeText != null)
            {
                _chapterBadgeText.gameObject.SetActive(chapter != null);
                if (chapter != null) _chapterBadgeText.text = $"[ {chapter.chapterBadge.ToUpper()} ]";
                _chapterBadgeText.color = _chapterBadgeColor;
            }

            if (_questTitleText != null)
            {
                _questTitleText.text = quest != null ? quest.questTitle.ToUpper() : "";
            }

            if (quest == null)
            {
                foreach (var row in _rows) row.gameObject.SetActive(false);
                return;
            }
            if (_rowTemplate == null || _objectivesContainer == null) return;

            while (_rows.Count < quest.objectives.Count)
            {
                var instance = Instantiate(_rowTemplate, _objectivesContainer);
                if (!instance.TryGetComponent<QuestHUDRow>(out var row) || !row.IsConfigured)
                {
                    Debug.LogError("[QuestHUD] Row template needs a QuestHUDRow with checkbox, description and ink references.", this);
                    Destroy(instance);
                    return;
                }
                _rows.Add(row);
            }

            QuestHUDRow sequentialCompletion = _completions.Count > 0 ? _completions.Peek() : null;
            bool celebrating = false;
            foreach (var row in _rows) celebrating |= row.IsCelebrating;
            for (int i = 0; i < _rows.Count; i++)
            {
                var row = _rows[i];
                if (i >= quest.objectives.Count)
                {
                    row.gameObject.SetActive(false);
                    continue;
                }

                var objective = quest.objectives[i];
                row.Bind(objective, _activeObjectiveColor, _completedObjectiveColor, _checkboxColor);
                bool visible;
                switch (quest.displayMode)
                {
                    case ObjectiveDisplayMode.SequentialOnly:
                        visible = sequentialCompletion != null ? row == sequentialCompletion : i == _manager.ActiveObjectiveIndex;
                        break;
                    case ObjectiveDisplayMode.ShowCompletedAndCurrent:
                        visible = objective.isCompleted || (!celebrating && i == _manager.ActiveObjectiveIndex);
                        break;
                    default:
                        visible = true;
                        break;
                }
                row.gameObject.SetActive(visible);
                if (visible && !row.IsCelebrating) row.ShowCurrentState();
            }
        }

        private void CancelCelebrations()
        {
            bool hadCelebrations = _completions.Count > 0;
            _completions.Clear();
            foreach (var row in _rows)
            {
                hadCelebrations |= row.IsCelebrating;
                row.CancelCompletion();
            }
            _displayDirty |= hadCelebrations;
        }

        private void PlaySound(AudioClip clip)
        {
            if (_audioSource != null && clip != null)
            {
                _audioSource.pitch = Random.Range(0.98f, 1.02f);
                _audioSource.PlayOneShot(clip);
            }
        }
    }
}
