using System;
using System.Collections.Generic;
using UnityEngine;

namespace Retro.QuestSystem
{
    public enum ObjectiveDisplayMode
    {
        [Tooltip("Shows only the currently active objective. When completed, the next one appears.")]
        SequentialOnly,

        [Tooltip("Shows completed objectives crossed out, plus the current active objective.")]
        ShowCompletedAndCurrent,

        [Tooltip("Displays all objectives in the quest from the start.")]
        ShowAllAtOnce
    }

    [CreateAssetMenu(fileName = "Quest_", menuName = "Retro Quest System/Quest Definition", order = 2)]
    public class QuestDefinition : ScriptableObject
    {
        [Header("Quest Identity")]
        [Tooltip("Unique ID for referencing this quest in code or triggers.")]
        public string questId = "quest_escape";

        [Tooltip("Header title displayed on HUD.")]
        public string questTitle = "ESCAPE THE SECTOR";

        [TextArea(2, 4)]
        [Tooltip("Short lore or brief description.")]
        public string questDescription = "";

        [Header("Progression Flow")]
        [Tooltip("How objectives are revealed on the HUD.")]
        public ObjectiveDisplayMode displayMode = ObjectiveDisplayMode.SequentialOnly;

        [Tooltip("If true, automatically advances to the next quest in the chapter when this quest finishes.")]
        public bool autoAdvanceToNextQuest = true;

        [Header("Audio")]
        [Tooltip("Sound played when this whole quest finishes.")]
        public AudioClip questCompleteSound;

        [Header("Objectives")]
        public List<QuestObjective> objectives = new List<QuestObjective>();

        public QuestObjective GetObjective(string id)
        {
            return objectives.Find(o => string.Equals(o.id, id, StringComparison.OrdinalIgnoreCase));
        }

        public int GetFirstIncompleteIndex()
        {
            for (int i = 0; i < objectives.Count; i++)
            {
                if (!objectives[i].IsFulfilled && !objectives[i].isOptional)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsQuestComplete()
        {
            foreach (var obj in objectives)
            {
                if (!obj.isOptional && !obj.IsFulfilled) return false;
            }
            return true;
        }

        public void ResetAllObjectives()
        {
            foreach (var obj in objectives)
            {
                obj.ResetProgress();
            }
        }
    }
}
