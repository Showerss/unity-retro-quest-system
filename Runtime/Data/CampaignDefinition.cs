using System.Collections.Generic;
using UnityEngine;

namespace Retro.QuestSystem
{
    [CreateAssetMenu(fileName = "Campaign_", menuName = "Retro Quest System/Campaign Definition", order = 0)]
    public class CampaignDefinition : ScriptableObject
    {
        [Header("Campaign Info")]
        [Tooltip("Title of your game campaign.")]
        public string campaignTitle = "RETRO RETRIEVAL";

        [Header("Chapters Sequence")]
        [Tooltip("Ordered list of chapters that make up the campaign.")]
        public List<ChapterDefinition> chapters = new List<ChapterDefinition>();

        public ChapterDefinition GetChapter(int chapterNumber)
        {
            return chapters.Find(c => c != null && c.chapterNumber == chapterNumber);
        }

        public ChapterDefinition GetChapterById(string id)
        {
            return chapters.Find(c => c != null && string.Equals(c.chapterId, id, System.StringComparison.OrdinalIgnoreCase));
        }

        public int GetNextChapterIndex(ChapterDefinition current)
        {
            int idx = chapters.IndexOf(current);
            if (idx >= 0 && idx + 1 < chapters.Count)
            {
                return idx + 1;
            }
            return -1;
        }
    }
}
