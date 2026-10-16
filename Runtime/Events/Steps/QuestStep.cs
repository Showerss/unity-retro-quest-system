using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Moves the quest log along: completes or advances an objective, or starts a quest or chapter.</summary>
    [Serializable]
    [SequenceStepMenu("Story/Quest")]
    public sealed class QuestStep : SequenceStep
    {
        public enum Action { CompleteObjective, AddProgress, StartQuest, StartChapter }

        [Tooltip("What to do to the quest log.")]
        [SerializeField] private Action _action = Action.CompleteObjective;

        [Tooltip("Used by Complete Objective and Add Progress: the objective's id.")]
        [SerializeField] private string _objectiveId = "obj_1";

        [Tooltip("Used by Add Progress.")]
        [SerializeField] private int _amount = 1;

        [Tooltip("Used by Start Quest.")]
        [SerializeField] private QuestDefinition _quest;

        [Tooltip("Used by Start Chapter.")]
        [SerializeField] private ChapterDefinition _chapter;

        public override string Summary
        {
            get
            {
                switch (_action)
                {
                    case Action.CompleteObjective: return $"Complete objective '{_objectiveId}'";
                    case Action.AddProgress: return $"Add {_amount} to objective '{_objectiveId}'";
                    case Action.StartQuest: return $"Start quest {NameOf(_quest)}";
                    default: return $"Start chapter {NameOf(_chapter)}";
                }
            }
        }

        public override IEnumerator Run(SequenceContext context)
        {
            QuestManager quests = QuestManager.Instance;
            if (quests == null)
            {
                Debug.LogWarning($"[QuestStep] No QuestManager in the scene; '{Summary}' skipped.", context.Sequence);
                yield break;
            }

            switch (_action)
            {
                case Action.CompleteObjective: quests.CompleteObjective(_objectiveId); break;
                case Action.AddProgress: quests.AddProgress(_objectiveId, _amount); break;
                case Action.StartQuest: if (_quest != null) quests.StartQuest(_quest); break;
                case Action.StartChapter: if (_chapter != null) quests.StartChapter(_chapter); break;
            }
        }
    }
}
