using System;
using System.Collections.Generic;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>
    /// The live values of every story flag, shared by all scenes. Flags nobody has set read as 0.
    /// Cleared each time play mode or the game starts; <see cref="Snapshot"/> and <see cref="Restore"/> are for the save system.
    /// </summary>
    public static class GameFlags
    {
        private static readonly Dictionary<string, int> _values = new Dictionary<string, int>();

        /// <summary>Raised after a flag's value actually changes, with its id and new value.</summary>
        public static event Action<string, int> Changed;

        public static int Get(FlagDefinition flag) => flag == null ? 0 : Get(flag.Id);

        public static int Get(string id) => id != null && _values.TryGetValue(id, out int value) ? value : 0;

        public static bool IsSet(FlagDefinition flag) => Get(flag) != 0;

        public static void Set(FlagDefinition flag, int value)
        {
            if (flag != null) Set(flag.Id, value);
        }

        public static void Set(string id, int value)
        {
            if (string.IsNullOrEmpty(id) || Get(id) == value) return;

            if (value == 0) _values.Remove(id);
            else _values[id] = value;
            Changed?.Invoke(id, value);
        }

        public static void Add(FlagDefinition flag, int amount)
        {
            if (flag != null) Set(flag.Id, Get(flag.Id) + amount);
        }

        /// <summary>A copy of every flag that is set, for saving.</summary>
        public static Dictionary<string, int> Snapshot() => new Dictionary<string, int>(_values);

        /// <summary>Replaces every flag with <paramref name="values"/>, e.g. when a save is loaded.</summary>
        public static void Restore(IReadOnlyDictionary<string, int> values)
        {
            var previous = new List<string>(_values.Keys);
            _values.Clear();
            if (values != null)
            {
                foreach (KeyValuePair<string, int> pair in values)
                {
                    if (pair.Value != 0) _values[pair.Key] = pair.Value;
                }
            }

            foreach (string id in previous)
            {
                if (!_values.ContainsKey(id)) Changed?.Invoke(id, 0);
            }
            foreach (KeyValuePair<string, int> pair in _values)
            {
                Changed?.Invoke(pair.Key, pair.Value);
            }
        }

        // Static state survives play mode when domain reload is off, so start every run clean.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForNewRun()
        {
            _values.Clear();
            Changed = null;
        }
    }
}
