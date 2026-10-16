using System.Collections.Generic;
using System.Linq;
using Retro.QuestSystem.Events;
using UnityEditor;
using UnityEngine;

namespace Retro.QuestSystem.Editor
{
    /// <summary>Lists every story flag with its live value during play, and lets you change them to test branches.</summary>
    public sealed class StoryFlagsWindow : EditorWindow
    {
        private Vector2 _scroll;
        private List<FlagDefinition> _flags = new List<FlagDefinition>();

        [MenuItem("Retro/Story Flags")]
        private static void Open() => GetWindow<StoryFlagsWindow>("Story Flags");

        private void OnEnable() => RefreshFlagAssets();

        private void OnProjectChange() => RefreshFlagAssets();

        // Polled rather than subscribed: GameFlags drops its listeners each time play mode starts.
        private void OnInspectorUpdate()
        {
            if (Application.isPlaying) Repaint();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
                EditorGUILayout.HelpBox("Enter play mode to see and change live values. All flags start at 0.", MessageType.Info);

            Dictionary<string, int> live = GameFlags.Snapshot();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (FlagDefinition flag in _flags)
            {
                if (flag == null) continue;
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.ObjectField(flag, typeof(FlagDefinition), false, GUILayout.Width(220f));
                    using (new EditorGUI.DisabledScope(!Application.isPlaying))
                    {
                        live.TryGetValue(flag.Id, out int value);
                        int edited = EditorGUILayout.DelayedIntField(value);
                        if (edited != value) GameFlags.Set(flag, edited);
                    }
                }
            }

            // Flags set by id that have no asset (e.g. from an old save).
            foreach (KeyValuePair<string, int> pair in live.Where(p => _flags.All(f => f == null || f.Id != p.Key)))
            {
                EditorGUILayout.LabelField($"{pair.Key} (no asset)", pair.Value.ToString());
            }
            EditorGUILayout.EndScrollView();
        }

        private void RefreshFlagAssets()
        {
            _flags = AssetDatabase.FindAssets("t:" + nameof(FlagDefinition))
                .Select(guid => AssetDatabase.LoadAssetAtPath<FlagDefinition>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(flag => flag != null)
                .OrderBy(flag => flag.name)
                .ToList();
        }
    }
}
