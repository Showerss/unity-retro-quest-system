using UnityEngine;

namespace Retro.QuestSystem.Events
{
    /// <summary>
    /// How sequences reach the player: freeze them, teleport them, and recognise them walking into a trigger.
    /// Put a component implementing it on the <see cref="SequenceDirector"/>, e.g. the controller adapter's FpcSequenceBridge.
    /// </summary>
    public interface ISequencePlayer
    {
        /// <summary>Stops (true) or restores (false) the player's movement, look and interaction.</summary>
        void SetLocked(bool locked);

        /// <summary>Puts the player standing on <paramref name="marker"/>'s position, facing its forward direction.</summary>
        void Teleport(Transform marker);

        /// <summary>Whether <paramref name="other"/> belongs to the player.</summary>
        bool IsPlayer(Collider other);
    }
}
