using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Retro.QuestSystem.Events
{
    /// <summary>
    /// A scripted event: a list of steps played top to bottom (wait, play a sound, lights off, show dialogue, set a flag...).
    /// Started by a trigger component (walk-in volume, interact, flag change), by another sequence, or by any UnityEvent calling <see cref="Play"/>.
    /// A save system can remember whether it finished through <see cref="HasFinished"/> and <see cref="RestoreFinished"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameSequence : MonoBehaviour
    {
        [Tooltip("The scene's sequence director (under --- SYSTEMS ---).")]
        [SerializeField] private SequenceDirector _director;

        [Tooltip("Play as soon as the scene starts.")]
        [SerializeField] private bool _playOnStart;

        [Tooltip("Only ever play once.")]
        [SerializeField] private bool _playOnce = true;

        [Tooltip("Optional. Set when the sequence finishes, and it won't play again while set. Use this for one-time events so they stay done after loading a save.")]
        [SerializeField] private FlagDefinition _doneFlag;

        [Tooltip("Every condition must be true for the sequence to play.")]
        [SerializeField] private FlagCondition[] _conditions = new FlagCondition[0];

        [SerializeReference] private List<SequenceStep> _steps = new List<SequenceStep>();

        [Tooltip("Called after the last step finishes.")]
        [SerializeField] private UnityEvent _onFinished = new UnityEvent();

        private bool _hasPlayed;
        private bool _hasFinished;
        private SequenceContext _context;

        /// <summary>How many sequences are playing right now, across all scenes. Autosaves wait for this to reach 0.</summary>
        public static int PlayingCount { get; private set; }

        /// <summary>
        /// While this returns true no sequence starts, e.g. while a save is loading. Set by a save system.
        /// Cleared each time play mode or the game starts.
        /// </summary>
        public static Func<bool> BlockStart { get; set; }

        public SequenceDirector Director => _director;
        public bool IsPlaying { get; private set; }

        /// <summary>Whether the sequence has played to the end at least once. A run stopped partway doesn't count.</summary>
        public bool HasFinished => _hasFinished;
        public IReadOnlyList<SequenceStep> Steps => _steps;

        public bool CanPlay =>
            !IsPlaying
            && !(_playOnce && _hasPlayed)
            && !(_doneFlag != null && _doneFlag.IsSet)
            && FlagCondition.AllMet(_conditions);

        private void Start()
        {
            if (_playOnStart) Play();
        }

        /// <summary>Plays the sequence if its conditions allow. Safe to hook up to any UnityEvent.</summary>
        public void Play() => TryPlay();

        /// <summary>Plays the sequence if its conditions allow, and says whether it started.</summary>
        public bool TryPlay()
        {
            if (!CanPlay || !isActiveAndEnabled || (BlockStart != null && BlockStart())) return false;
            if (_director == null)
            {
                Debug.LogError($"[GameSequence] '{name}' has no Sequence Director.", this);
                return false;
            }

            StartCoroutine(RunSteps());
            return true;
        }

        /// <summary>Stops partway, freeing the player if a step had frozen them.</summary>
        public void Stop()
        {
            if (!IsPlaying) return;
            StopAllCoroutines();
            SetPlaying(false);
            if (_context != null && _context.PlayerLocked) _context.SetPlayerLocked(false);
        }

        private IEnumerator RunSteps()
        {
            SetPlaying(true);
            _hasPlayed = true;
            _context = new SequenceContext(_director, this);

            foreach (SequenceStep step in _steps)
            {
                if (step == null) continue;
                if (step.DontWait) StartCoroutine(step.Run(_context));
                else yield return StartCoroutine(step.Run(_context));
            }

            if (_context.PlayerLocked) _context.SetPlayerLocked(false);
            if (_doneFlag != null) GameFlags.Set(_doneFlag, 1);
            _hasFinished = true;
            SetPlaying(false);
            _onFinished.Invoke();
        }

        private void SetPlaying(bool playing)
        {
            if (IsPlaying == playing) return;
            IsPlaying = playing;
            PlayingCount += playing ? 1 : -1;
        }

        /// <summary>For loading a save: marks the sequence finished (so a Play Once sequence won't replay) or not.</summary>
        public void RestoreFinished(bool finished)
        {
            _hasFinished = finished;
            _hasPlayed = finished;
        }

        // Static state survives play mode when domain reload is off.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            PlayingCount = 0;
            BlockStart = null;
        }

        private void OnDisable()
        {
            Stop();
        }
    }
}
