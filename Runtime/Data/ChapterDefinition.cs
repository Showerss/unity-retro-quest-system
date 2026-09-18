using System;
using System.Collections.Generic;
using UnityEngine;

namespace Retro.QuestSystem
{
    [CreateAssetMenu(fileName = "Chapter_", menuName = "Retro Quest System/Chapter Definition", order = 1)]
    public class ChapterDefinition : ScriptableObject
    {
        [Header("Chapter Info")]
        [Tooltip("Chapter index number, e.g. 1 for Chapter 1.")]
        public int chapterNumber = 1;

        [Tooltip("Unique chapter identifier.")]
        public string chapterId = "chapter_1";

        [Tooltip("Full display title (e.g. 'ESCAPE THE INDUSTRIAL ZONE').")]
        public string chapterTitle = "INDUSTRIAL COMPLEX";

        [Tooltip("Short badge header (e.g. 'CHAPTER 1').")]
        public string chapterBadge = "CHAPTER 1";

        [TextArea(2, 4)]
        [Tooltip("Brief narrative or scene setting.")]
        public string chapterSynopsis = "";

        [Header("Quests in this Chapter")]
        [Tooltip("Ordered list of quests to play through in this chapter.")]
        public List<QuestDefinition> quests = new List<QuestDefinition>();

        [Header("Chapter Completion")]
        [Tooltip("If true, automatically transitions to the next chapter when all quests are complete.")]
        public bool autoAdvanceToNextChapter = true;

        [Tooltip("Optional chapter clear sound effect.")]
        public AudioClip chapterClearSound;

        public QuestDefinition GetQuest(string id)
        {
            return quests.Find(q => q != null && string.Equals(q.questId, id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
