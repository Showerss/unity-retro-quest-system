# Retro Quest System

A modular, standalone 90s-styled Quest & Objective Tracking system designed for Unity (URP).

## Architecture Overview

```
CampaignDefinition (Entire Game)
└── ChapterDefinition (e.g. Chapter 1: Industrial Complex, Chapter 2: Neighborhood)
    └── QuestDefinition (e.g. "Escape The Sector", "Restore The Generator")
        └── QuestObjective[] (Sequential or All-at-Once tasks)
```

### Key Features
1. **Chapter Hierarchy:** Break your game down into distinct Chapters. Each Chapter contains an ordered list of Quests.
2. **Sequential Objective Reveal:** Quests can be set to `SequentialOnly`, `ShowCompletedAndCurrent`, or `ShowAllAtOnce`. With `SequentialOnly`, players only see their immediate task. When crossed off, it plays a chime and automatically reveals the next objective!
3. **Cross-Scene Persistence:** `QuestManager` is marked with `DontDestroyOnLoad`. When the player transitions levels (e.g. gets into the car and loads into the Neighborhood scene), their active quest and chapter state seamlessly carry over.
4. **Trigger Integration:** Drop `QuestTrigger` onto any collider or interactable to complete objectives by ID or advance counters.
5. **UI HUD Included:** Includes a preconfigured `QuestHUD_Canvas.prefab` ready to drop into any scene.

---

## Installation

### Option 1: Via Git URL (Package Manager)
1. In Unity, open **Window > Package Manager**.
2. Click the **`+`** icon in the upper-left corner and choose **Add package from git URL...**
3. Enter:
   ```text
   https://github.com/Showerss/unity-retro-quest-system.git
   ```

### Option 2: Via `manifest.json`
Add this line to your project's `Packages/manifest.json` under `"dependencies"`:
```json
"com.retro.questsystem": "https://github.com/Showerss/unity-retro-quest-system.git"
```
Or for local development:
```json
"com.retro.questsystem": "file:../path/to/Packages/com.retro.questsystem"
```

## Quick Start

1. Create a `CampaignDefinition` ScriptableObject via **Assets > Create > Quest System > Campaign**.
2. Create Chapters and Quests, and assign them in the campaign hierarchy.
3. Drop the `QuestHUD_Canvas.prefab` into your first scene.
4. Assign your Campaign to the `QuestManager` component on the canvas.
5. Add `QuestTrigger` to interactable objects or trigger volumes to complete tasks as the player progresses.

## Scripted events and story flags

For story beats that are more than "complete an objective": a `GameSequence` is a list of steps
(wait, play a sound, kill the lights, fade to black, freeze the player, show dialogue, set a
flag, complete an objective...) played top to bottom. Start it with a `SequenceTriggerVolume`
(walk in), a `SequenceInteractable` (press E), a `SequenceFlagTrigger` (when flags say so), or
any UnityEvent calling `Play()`.

1. Put one `SequenceDirector` in each scene, with an `FpcSequenceBridge` (player and fader) and a
   `DialogueSequenceBridge` (dialogue box) next to it. Those come from the adapters, which compile
   only when the first-person controller and dialogue packages are installed.
2. Create story flags with **Assets > Create > Retro Quest System > Story Flag**; watch and edit
   them live in **Retro > Story Flags**.
3. Add steps with the sequence's **+** menu. For a new step type, derive from `SequenceStep` and
   mark it `[Serializable]` and `[SequenceStepMenu("Group/Name")]`.

## License

[MIT License](LICENSE.md) © 2026 Phillip Banky
