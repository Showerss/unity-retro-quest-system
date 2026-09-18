using System;
using UnityEngine;

namespace Retro.QuestSystem
{
    [Serializable]
    public class QuestObjective
    {
        [Tooltip("Unique ID for this objective.")]
        public string id = "obj_1";

        [Tooltip("Display text in HUD.")]
        public string description = "Locate the vehicle";

        [Tooltip("Target count for progress tracking (e.g. collect 3 keys = 3, single action = 1).")]
        public int targetAmount = 1;

        [Tooltip("Current count achieved.")]
        public int currentAmount = 0;

        [Tooltip("Whether this objective is currently completed.")]
        public bool isCompleted = false;

        [Tooltip("Optional sound override when this specific objective completes.")]
        public AudioClip completionSound;

        [Tooltip("If true, this objective does not block quest advancement.")]
        public bool isOptional = false;

        public bool IsFulfilled => isCompleted || currentAmount >= targetAmount;

        public float ProgressNormalized => targetAmount > 0 ? Mathf.Clamp01((float)currentAmount / targetAmount) : 1f;

        public void ResetProgress()
        {
            currentAmount = 0;
            isCompleted = false;
        }
    }
}
