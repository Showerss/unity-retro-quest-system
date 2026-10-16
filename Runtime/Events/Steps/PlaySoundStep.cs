using System;
using System.Collections;
using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>Plays a sound: in the player's head (stingers), from a point in the world, or through a chosen AudioSource.</summary>
    [Serializable]
    [SequenceStepMenu("Audio/Play Sound")]
    public sealed class PlaySoundStep : SequenceStep
    {
        public enum Where { InPlayersHead, AtPoint, OnSource }

        [Tooltip("The sound to play.")]
        [SerializeField] private AudioClip _clip;

        [Tooltip("In Player's Head: not positioned (stingers). At Point: from Point in the world. On Source: through Source with its own settings.")]
        [SerializeField] private Where _where = Where.InPlayersHead;

        [Tooltip("Used by At Point: where the sound comes from.")]
        [SerializeField] private Transform _point;

        [Tooltip("Used by On Source: the AudioSource to play it through.")]
        [SerializeField] private AudioSource _source;

        [Tooltip("Loudness, 0 to 1.")]
        [Range(0f, 1f)]
        [SerializeField] private float _volume = 1f;

        public override string Summary => $"Play sound {NameOf(_clip)}";

        public override IEnumerator Run(SequenceContext context)
        {
            if (_clip == null) yield break;

            switch (_where)
            {
                case Where.AtPoint:
                    AudioSource.PlayClipAtPoint(_clip, _point != null ? _point.position : context.Sequence.transform.position, _volume);
                    break;
                case Where.OnSource when _source != null:
                    _source.PlayOneShot(_clip, _volume);
                    break;
                default:
                    if (context.Director.AudioSource != null) context.Director.AudioSource.PlayOneShot(_clip, _volume);
                    break;
            }

            yield return new WaitForSeconds(_clip.length);
        }
    }
}
