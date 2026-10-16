namespace Retro.QuestSystem.Events
{
    /// <summary>What a step can reach while its sequence plays: the scene's director, the sequence itself, and shared state between steps.</summary>
    public sealed class SequenceContext
    {
        public SequenceContext(SequenceDirector director, GameSequence sequence)
        {
            Director = director;
            Sequence = sequence;
        }

        public SequenceDirector Director { get; }
        public GameSequence Sequence { get; }

        /// <summary>Whether a Lock Player step froze the player. The sequence frees them when it ends.</summary>
        public bool PlayerLocked { get; private set; }

        public void SetPlayerLocked(bool locked)
        {
            PlayerLocked = locked;
            Director.SetPlayerLocked(locked);
        }
    }
}
