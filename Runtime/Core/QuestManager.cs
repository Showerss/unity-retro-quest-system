using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Retro.QuestSystem
{
    /// <summary>
    /// Persistent central Quest and Chapter Manager.
    /// Controls chapter sequencing, quest advancement, and objective completion.
    /// Can persist across scene loads (DontDestroyOnLoad).
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Campaign Configuration")]
        [Tooltip("The overarching campaign containing all chapters.")]
        [SerializeField] private CampaignDefinition _campaign;

        [Tooltip("Initial chapter to start with if none active.")]
        [SerializeField] private ChapterDefinition _startingChapter;

        [Tooltip("Initial quest to start if chapter not used.")]
        [SerializeField] private QuestDefinition _startingQuest;

        [Header("Persistence")]
        [Tooltip("Keep QuestManager alive across scene transitions.")]
        [SerializeField] private bool _persistAcrossScenes = true;

        [Tooltip("Reset objective completion states on Play mode start (useful for development).")]
        [SerializeField] private bool _resetProgressOnPlay = true;

        /// <summary>The campaign this manager runs, if one is assigned.</summary>
        public CampaignDefinition Campaign => _campaign;

        /// <summary>The chapter started when no campaign is assigned.</summary>
        public ChapterDefinition StartingChapter => _startingChapter;

        // Runtime State
        public ChapterDefinition ActiveChapter { get; private set; }
        public QuestDefinition ActiveQuest { get; private set; }
        public int ActiveObjectiveIndex { get; private set; } = 0;

        // Events
        public event Action<ChapterDefinition> OnChapterStarted;
        public event Action<ChapterDefinition> OnChapterCompleted;
        public event Action<QuestDefinition> OnQuestStarted;
        public event Action<QuestDefinition> OnQuestCompleted;
        public event Action<QuestObjective> OnObjectiveUpdated;
        public event Action<QuestObjective> OnObjectiveCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (_persistAcrossScenes && transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (_resetProgressOnPlay)
            {
                ResetCampaignProgress();
            }
        }

        private void Start()
        {
            if (ActiveQuest == null)
            {
                if (_startingChapter != null)
                {
                    StartChapter(_startingChapter);
                }
                else if (_campaign != null && _campaign.chapters.Count > 0)
                {
                    StartChapter(_campaign.chapters[0]);
                }
                else if (_startingQuest != null)
                {
                    StartQuest(_startingQuest);
                }
            }
        }

        public void StartChapter(ChapterDefinition chapter)
        {
            if (chapter == null) return;

            ActiveChapter = chapter;
            OnChapterStarted?.Invoke(ActiveChapter);

            if (chapter.quests.Count > 0 && chapter.quests[0] != null)
            {
                StartQuest(chapter.quests[0]);
            }
            else
            {
                ActiveQuest = null;
            }
        }

        public void StartQuest(QuestDefinition quest)
        {
            if (quest == null) return;

            ActiveQuest = quest;
            ActiveObjectiveIndex = Mathf.Max(0, quest.GetFirstIncompleteIndex());
            if (ActiveObjectiveIndex < 0) ActiveObjectiveIndex = 0;

            OnQuestStarted?.Invoke(ActiveQuest);
        }

        public void CompleteObjective(string objectiveId)
        {
            if (ActiveQuest == null) return;

            var obj = ActiveQuest.GetObjective(objectiveId);
            if (obj == null)
            {
                // Search across active chapter if not in active quest
                if (ActiveChapter != null)
                {
                    foreach (var q in ActiveChapter.quests)
                    {
                        var candidate = q.GetObjective(objectiveId);
                        if (candidate != null)
                        {
                            if (ActiveQuest != q) StartQuest(q);
                            obj = candidate;
                            break;
                        }
                    }
                }
            }

            if (obj == null || obj.isCompleted) return;

            obj.isCompleted = true;
            obj.currentAmount = obj.targetAmount;

            OnObjectiveCompleted?.Invoke(obj);
            OnObjectiveUpdated?.Invoke(obj);

            CheckQuestProgression();
        }

        public void AddProgress(string objectiveId, int amount = 1)
        {
            if (ActiveQuest == null) return;

            var obj = ActiveQuest.GetObjective(objectiveId);
            if (obj == null || obj.isCompleted) return;

            obj.currentAmount += amount;
            if (obj.currentAmount >= obj.targetAmount)
            {
                obj.isCompleted = true;
                obj.currentAmount = obj.targetAmount;
                OnObjectiveCompleted?.Invoke(obj);
            }

            OnObjectiveUpdated?.Invoke(obj);
            CheckQuestProgression();
        }

        private void CheckQuestProgression()
        {
            if (ActiveQuest == null) return;

            // Update active objective index for sequential reveal
            int nextIncomplete = ActiveQuest.GetFirstIncompleteIndex();
            if (nextIncomplete >= 0)
            {
                ActiveObjectiveIndex = nextIncomplete;
            }

            // Check if full quest is completed
            if (ActiveQuest.IsQuestComplete())
            {
                StartCoroutine(HandleQuestCompletionRoutine(ActiveQuest));
            }
        }

        private IEnumerator HandleQuestCompletionRoutine(QuestDefinition completedQuest)
        {
            OnQuestCompleted?.Invoke(completedQuest);

            yield return new WaitForSeconds(1.2f);

            if (completedQuest.autoAdvanceToNextQuest && ActiveChapter != null)
            {
                AdvanceToNextQuest();
            }
        }

        public void AdvanceToNextQuest()
        {
            if (ActiveChapter == null) return;

            int currentIdx = ActiveChapter.quests.IndexOf(ActiveQuest);
            if (currentIdx >= 0 && currentIdx + 1 < ActiveChapter.quests.Count)
            {
                StartQuest(ActiveChapter.quests[currentIdx + 1]);
            }
            else
            {
                // Chapter finished!
                StartCoroutine(HandleChapterCompletionRoutine(ActiveChapter));
            }
        }

        private IEnumerator HandleChapterCompletionRoutine(ChapterDefinition completedChapter)
        {
            OnChapterCompleted?.Invoke(completedChapter);

            yield return new WaitForSeconds(1.5f);

            if (completedChapter.autoAdvanceToNextChapter && _campaign != null)
            {
                AdvanceToNextChapter();
            }
        }

        public void AdvanceToNextChapter()
        {
            if (_campaign == null || ActiveChapter == null) return;

            int nextChapterIdx = _campaign.GetNextChapterIndex(ActiveChapter);
            if (nextChapterIdx >= 0 && nextChapterIdx < _campaign.chapters.Count)
            {
                StartChapter(_campaign.chapters[nextChapterIdx]);
            }
            else
            {
                Debug.Log("[QuestManager] Campaign completed!");
            }
        }

        public void ResetCampaignProgress()
        {
            if (_campaign != null)
            {
                foreach (var ch in _campaign.chapters)
                {
                    if (ch == null) continue;
                    foreach (var q in ch.quests)
                    {
                        if (q != null) q.ResetAllObjectives();
                    }
                }
            }
            else if (_startingChapter != null)
            {
                foreach (var q in _startingChapter.quests)
                {
                    if (q != null) q.ResetAllObjectives();
                }
            }
            else if (_startingQuest != null)
            {
                _startingQuest.ResetAllObjectives();
            }
        }
    }
}
