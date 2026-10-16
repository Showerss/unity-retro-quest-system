using System.Collections;

namespace Retro.QuestSystem.Events
{
    /// <summary>
    /// The full-screen black overlay sequences fade. Put a component implementing it on the <see cref="SequenceDirector"/>,
    /// e.g. the controller adapter's FpcSequenceBridge.
    /// </summary>
    public interface ISequenceFader
    {
        /// <summary>Fades from the current darkness to <paramref name="darkness"/> (0 clear, 1 black); 0 seconds cuts.</summary>
        IEnumerator FadeTo(float darkness, float seconds);
    }
}
