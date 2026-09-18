using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Retro.QuestSystem
{
    /// <summary>
    /// Modular 90s Retro Quest HUD.
    /// Supports chapter badges, quest titles, and sequential objective reveal.
    /// </summary>
    public class QuestHUD : MonoBehaviour
    {
        [Header("UI Header Elements")]
        [SerializeField] private TextMeshProUGUI _chapterBadgeText;
        [SerializeField] private TextMeshProUGUI _questTitleText;
        [SerializeField] private RectTransform _objectivesContainer;

        [Header("Row Template")]
        [Tooltip("Template row containing checkbox and description TMP texts.")]
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

        [Header("Sequential Display Settings")]
        [Tooltip("Seconds to display the completed strike-through before sliding to next objective in Sequential mode.")]
        [SerializeField] private float _delayBeforeNextObjective = 1.0f;

        private readonly List<GameObject> _spawnedRows = new List<GameObject>();
        private Coroutine _refreshRoutine;

        private void Awake()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (_rowTemplate != null && _rowTemplate.activeSelf)
            {
                _rowTemplate.SetActive(false);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(SubscribeToManager());
        }

        private void OnDisable()
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnChapterStarted -= HandleChapterStarted;
                QuestManager.Instance.OnQuestStarted -= HandleQuestStarted;
                QuestManager.Instance.OnObjectiveUpdated -= HandleObjectiveUpdated;
                QuestManager.Instance.OnObjectiveCompleted -= HandleObjectiveCompleted;
                QuestManager.Instance.OnQuestCompleted -= HandleQuestCompleted;
            }
        }

        private IEnumerator SubscribeToManager()
        {
            while (QuestManager.Instance == null)
            {
                yield return null;
            }

            QuestManager.Instance.OnChapterStarted += HandleChapterStarted;
            QuestManager.Instance.OnQuestStarted += HandleQuestStarted;
            QuestManager.Instance.OnObjectiveUpdated += HandleObjectiveUpdated;
            QuestManager.Instance.OnObjectiveCompleted += HandleObjectiveCompleted;
            QuestManager.Instance.OnQuestCompleted += HandleQuestCompleted;

            RefreshDisplay();
        }

        private void HandleChapterStarted(ChapterDefinition chapter) => RefreshDisplay();
        private void HandleQuestStarted(QuestDefinition quest) => RefreshDisplay();

        private void HandleObjectiveUpdated(QuestObjective obj)
        {
            RefreshDisplay();
        }

        private void HandleObjectiveCompleted(QuestObjective obj)
        {
            PlaySound(obj.completionSound != null ? obj.completionSound : _objectiveCompleteSound);

            if (_refreshRoutine != null) StopCoroutine(_refreshRoutine);
            _refreshRoutine = StartCoroutine(DelayedRefreshRoutine());
        }

        private void HandleQuestCompleted(QuestDefinition quest)
        {
            PlaySound(quest.questCompleteSound != null ? quest.questCompleteSound : _questCompleteSound);
        }

        private IEnumerator DelayedRefreshRoutine()
        {
            RefreshDisplay(showActiveObjectiveCompleted: true);
            yield return new WaitForSeconds(_delayBeforeNextObjective);
            RefreshDisplay();
        }

        public void RefreshDisplay(bool showActiveObjectiveCompleted = false)
        {
            if (QuestManager.Instance == null) return;

            var chapter = QuestManager.Instance.ActiveChapter;
            var quest = QuestManager.Instance.ActiveQuest;

            // Chapter Badge
            if (_chapterBadgeText != null)
            {
                if (chapter != null)
                {
                    _chapterBadgeText.text = $"[ {chapter.chapterBadge.ToUpper()} ]";
                    _chapterBadgeText.color = _chapterBadgeColor;
                    _chapterBadgeText.gameObject.SetActive(true);
                }
                else
                {
                    _chapterBadgeText.gameObject.SetActive(false);
                }
            }

            // Quest Title
            if (_questTitleText != null)
            {
                if (quest != null)
                {
                    _questTitleText.text = quest.questTitle.ToUpper();
                    _questTitleText.gameObject.SetActive(true);
                }
                else
                {
                    _questTitleText.text = "";
                }
            }

            ClearSpawnedRows();

            if (quest == null || _rowTemplate == null || _objectivesContainer == null) return;

            int activeIdx = QuestManager.Instance.ActiveObjectiveIndex;

            switch (quest.displayMode)
            {
                case ObjectiveDisplayMode.SequentialOnly:
                    // Render ONLY current active objective
                    if (activeIdx >= 0 && activeIdx < quest.objectives.Count)
                    {
                        var obj = quest.objectives[activeIdx];
                        SpawnRow(obj, isForceCompleted: showActiveObjectiveCompleted && obj.isCompleted);
                    }
                    break;

                case ObjectiveDisplayMode.ShowCompletedAndCurrent:
                    // Render all previously completed plus the active one
                    for (int i = 0; i < quest.objectives.Count; i++)
                    {
                        var obj = quest.objectives[i];
                        if (obj.isCompleted || i == activeIdx)
                        {
                            SpawnRow(obj);
                        }
                    }
                    break;

                case ObjectiveDisplayMode.ShowAllAtOnce:
                default:
                    foreach (var obj in quest.objectives)
                    {
                        SpawnRow(obj);
                    }
                    break;
            }
        }

        private void SpawnRow(QuestObjective obj, bool isForceCompleted = false)
        {
            GameObject row = Instantiate(_rowTemplate, _objectivesContainer);
            row.SetActive(true);
            _spawnedRows.Add(row);

            var texts = row.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshProUGUI iconText = null;
            TextMeshProUGUI descText = null;

            foreach (var t in texts)
            {
                if (t.name.ToLower().Contains("icon") || t.name.ToLower().Contains("box"))
                {
                    iconText = t;
                }
                else
                {
                    descText = t;
                }
            }

            if (descText == null && texts.Length > 0) descText = texts[0];

            bool completed = obj.isCompleted || isForceCompleted;

            if (iconText != null)
            {
                iconText.text = completed ? "[X]" : "[ ]";
                iconText.color = completed ? _completedObjectiveColor : _checkboxColor;
            }

            if (descText != null)
            {
                string countSuffix = obj.targetAmount > 1 ? $" ({obj.currentAmount}/{obj.targetAmount})" : "";
                if (completed)
                {
                    descText.text = $"<s>{obj.description}{countSuffix}</s>";
                    descText.color = _completedObjectiveColor;
                }
                else
                {
                    descText.text = $"{obj.description}{countSuffix}";
                    descText.color = _activeObjectiveColor;
                }
            }
        }

        private void ClearSpawnedRows()
        {
            foreach (var r in _spawnedRows)
            {
                if (r != null) Destroy(r);
            }
            _spawnedRows.Clear();
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
