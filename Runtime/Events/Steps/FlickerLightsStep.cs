using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Makes lights stutter on and off for a while, then leaves them on or off.</summary>
    [Serializable]
    [SequenceStepMenu("Lights/Flicker Lights")]
    public sealed class FlickerLightsStep : SequenceStep
    {
        [Tooltip("Lights to flicker.")]
        [SerializeField] private Light[] _lights = new Light[0];

        [Tooltip("How long the flickering lasts, in seconds.")]
        [Min(0f)]
        [SerializeField] private float _seconds = 1.5f;

        [Tooltip("Shortest and longest time between flickers, in seconds.")]
        [SerializeField] private Vector2 _interval = new Vector2(0.03f, 0.18f);

        [Tooltip("Whether the lights stay on once the flickering stops.")]
        [SerializeField] private bool _endOn;

        public override string Summary => $"Flicker {_lights?.Length ?? 0} light(s) for {_seconds:0.##}s, then {(_endOn ? "on" : "off")}";

        public override IEnumerator Run(SequenceContext context)
        {
            bool on = true;
            float elapsed = 0f;
            while (elapsed < _seconds)
            {
                on = !on;
                SetAll(on);
                float wait = UnityEngine.Random.Range(Mathf.Max(0.01f, _interval.x), Mathf.Max(0.01f, _interval.y));
                elapsed += wait;
                yield return new WaitForSeconds(wait);
            }
            SetAll(_endOn);
        }

        private void SetAll(bool on)
        {
            foreach (Light light in _lights)
            {
                if (light != null) light.enabled = on;
            }
        }
    }
}
