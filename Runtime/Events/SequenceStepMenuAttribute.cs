using System;

namespace Retro.QuestSystem.Events
{
    /// <summary>Where a step appears in a sequence's Add Step menu, e.g. "Audio/Play Sound".</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class SequenceStepMenuAttribute : Attribute
    {
        public SequenceStepMenuAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
