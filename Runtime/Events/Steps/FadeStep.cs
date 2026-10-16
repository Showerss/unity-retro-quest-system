using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Fades the screen to black, back in, or anywhere between. Dialogue still shows on a black screen.</summary>
    [Serializable]
    [SequenceStepMenu("Screen/Fade")]
    public sealed class FadeStep : SequenceStep
    {
        [Tooltip("How dark the screen ends up: 0 = clear, 1 = black.")]
        [Range(0f, 1f)]
        [SerializeField] private float _darkness = 1f;

        [Tooltip("How long the fade takes, in seconds. 0 cuts straight there.")]
        [Min(0f)]
        [SerializeField] private float _seconds = 1f;

        public override string Summary => _seconds <= 0f
            ? $"Cut to {(_darkness >= 1f ? "black" : _darkness <= 0f ? "clear" : $"{_darkness:0.##} dark")}"
            : $"Fade to {(_darkness >= 1f ? "black" : _darkness <= 0f ? "clear" : $"{_darkness:0.##} dark")} over {_seconds:0.##}s";

        public override IEnumerator Run(SequenceContext context)
        {
            if (context.Director.Fader == null) yield break;
            yield return context.Director.Fader.FadeTo(_darkness, _seconds);
        }
    }
}
