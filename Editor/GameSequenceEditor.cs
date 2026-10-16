using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Retro.QuestSystem.Events;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Retro.QuestSystem.Editor
{
    /// <summary>
    /// Shows a sequence's steps as a reorderable list titled by what each step does, with an Add Step menu built from
    /// every <see cref="SequenceStep"/> type, and a Play button in play mode.
    /// </summary>
    [CustomEditor(typeof(GameSequence))]
    public sealed class GameSequenceEditor : UnityEditor.Editor
    {
        private const float Padding = 4f;

        private SerializedProperty _director;
        private SerializedProperty _steps;
        private ReorderableList _list;

        private void OnEnable()
        {
            _director = serializedObject.FindProperty("_director");
            _steps = serializedObject.FindProperty("_steps");
            AssignSceneDirectorIfMissing();

            _list = new ReorderableList(serializedObject, _steps, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Steps (played top to bottom)"),
                elementHeightCallback = ElementHeight,
                drawElementCallback = DrawElement,
                onAddDropdownCallback = (rect, list) => ShowAddMenu(),
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty property = serializedObject.GetIterator();
            property.NextVisible(true); // m_Script
            using (new EditorGUI.DisabledScope(true)) EditorGUILayout.PropertyField(property);
            while (property.NextVisible(false))
            {
                if (property.name == "_steps")
                {
                    EditorGUILayout.Space();
                    _list.DoLayoutList();
                    continue;
                }
                EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
            {
                var sequence = (GameSequence)target;
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(sequence.IsPlaying ? "Playing..." : "Play now")) sequence.Play();
                    if (GUILayout.Button("Stop")) sequence.Stop();
                }
                if (!sequence.CanPlay && !sequence.IsPlaying)
                    EditorGUILayout.HelpBox("Won't play right now: already played, done flag set, or a condition is false.", MessageType.Info);
            }
        }

        private float ElementHeight(int index)
        {
            SerializedProperty element = _steps.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, true) + Padding;
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _steps.GetArrayElementAtIndex(index);
            rect.y += Padding * 0.5f;
            rect.x += 10f;
            rect.width -= 10f;
            rect.height = EditorGUI.GetPropertyHeight(element, true);

            string title = element.managedReferenceValue is SequenceStep step
                ? $"{index + 1}. {step.Summary}{(step.DontWait ? "  (don't wait)" : string.Empty)}"
                : $"{index + 1}. (missing step type)";
            EditorGUI.PropertyField(rect, element, new GUIContent(title), true);
        }

        private void ShowAddMenu()
        {
            var menu = new GenericMenu();
            foreach ((string path, Type type) in StepTypes())
            {
                menu.AddItem(new GUIContent(path), false, () => AddStep(type));
            }
            menu.ShowAsContext();
        }

        private void AddStep(Type type)
        {
            serializedObject.Update();
            int index = _steps.arraySize;
            _steps.arraySize++;
            SerializedProperty element = _steps.GetArrayElementAtIndex(index);
            element.managedReferenceValue = Activator.CreateInstance(type);
            element.isExpanded = true;
            serializedObject.ApplyModifiedProperties();
        }

        private static IEnumerable<(string path, Type type)> StepTypes()
        {
            return TypeCache.GetTypesDerivedFrom<SequenceStep>()
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .Select(type => (type.GetCustomAttribute<SequenceStepMenuAttribute>()?.Path ?? "Other/" + ObjectNames.NicifyVariableName(type.Name), type))
                .OrderBy(entry => entry.Item1);
        }

        // A new sequence usually wants the one director in its scene; fill it in so nobody has to drag it every time.
        private void AssignSceneDirectorIfMissing()
        {
            if (_director.objectReferenceValue != null || Application.isPlaying) return;
            var sequence = (GameSequence)target;
            if (!sequence.gameObject.scene.IsValid()) return;

            foreach (GameObject root in sequence.gameObject.scene.GetRootGameObjects())
            {
                var director = root.GetComponentInChildren<SequenceDirector>(true);
                if (director == null) continue;
                serializedObject.Update();
                _director.objectReferenceValue = director;
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
    }
}
