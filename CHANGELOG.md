# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-09-18

### Added
- Modular 90s-styled Quest & Objective Tracking system for Unity (URP).
- Hierarchical structure: Campaign -> Chapter -> Quest -> Objectives.
- Sequential objective reveal modes (`SequentialOnly`, `ShowCompletedAndCurrent`, `ShowAllAtOnce`).
- Cross-scene persistence via `DontDestroyOnLoad` QuestManager.
- Trigger integration with `QuestTrigger` component.
- Customizable QuestHUD Canvas prefab with audio chimes and animations.
- Standalone UPM package layout with assembly definition.
