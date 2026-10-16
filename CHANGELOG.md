# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2026-10-16

### Added
- Scripted events: `GameSequence` (steps played top to bottom, with conditions, play-once and a done flag), `SequenceDirector`, and the triggers `SequenceTriggerVolume` and `SequenceFlagTrigger`.
- Steps: Wait, Set Flag, If Flags, Play Sequence, Call Event, Play Sound, Show or Hide Objects, Move Object, Flicker Lights, Fade, Freeze or Free Player, Teleport Player, Quest. Add your own by deriving from `SequenceStep`.
- Story flags: `FlagDefinition` assets, the `GameFlags` store and `FlagCondition`.
- Editor: sequence inspector with an Add Step menu and Play/Stop in play mode, and the **Retro > Story Flags** window.
- `ISequencePlayer` and `ISequenceFader`, so sequences reach the player and screen through adapters. `GameSequence.BlockStart`, `HasFinished` and `RestoreFinished` for save systems.
- FirstPersonController adapter: `FpcSequenceBridge`, `SequenceInteractable` (press E to play) and the Set Lamps step.
- Dialogue adapter: `DialogueSequenceBridge` and the Show Dialogue step.

## [1.1.0] - 2026-10-16

### Added
- `QuestManager.Campaign` and `QuestManager.StartingChapter` read-only accessors, so save systems
  can find every quest.

## [1.0.0] - 2026-09-18

### Added
- Modular 90s-styled Quest & Objective Tracking system for Unity (URP).
- Hierarchical structure: Campaign -> Chapter -> Quest -> Objectives.
- Sequential objective reveal modes (`SequentialOnly`, `ShowCompletedAndCurrent`, `ShowAllAtOnce`).
- Cross-scene persistence via `DontDestroyOnLoad` QuestManager.
- Trigger integration with `QuestTrigger` component.
- Customizable QuestHUD Canvas prefab with audio chimes and animations.
- Standalone UPM package layout with assembly definition.
