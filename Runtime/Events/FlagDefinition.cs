using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>
    /// One fact about the story so far, e.g. "found the letter" or "times the bell has rung".
    /// Flags are numbers: 0 means not set, anything else means set, so the same flag can be a yes/no or a counter.
    /// The live value is held by <see cref="GameFlags"/>; this asset is only the name you pick in the inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "SO_Flag_", menuName = "Retro Quest System/Story Flag", order = 10)]
    public sealed class FlagDefinition : ScriptableObject
    {
        [Tooltip("Key the value is stored and saved under. Filled from the asset name. Don't change it once saves exist, or they lose this flag.")]
        [SerializeField] private string _id;

        [Tooltip("What this flag means in the story, for whoever is writing or wiring scenes.")]
        [TextArea(2, 5)]
        [SerializeField] private string _notes;

        public string Id => string.IsNullOrEmpty(_id) ? name : _id;

        public int Value => GameFlags.Get(this);
        public bool IsSet => GameFlags.IsSet(this);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id)) _id = name;
        }
#endif
    }
}
