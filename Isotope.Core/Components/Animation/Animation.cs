using System.Collections.Generic;

namespace Isotope.Core.Components.Animation
{
    /// <summary>
    /// Defines a single animation sequence.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// The name of the animation (e.g., "walk", "idle", "attack").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of frame indices to be played in sequence from the sprite sheet.
        /// </summary>
        public List<int> Frames { get; set; }

        /// <summary>
        /// The duration of each frame in seconds.
        /// </summary>
        public float FrameDuration { get; set; }

        /// <summary>
        /// Whether the animation should loop back to the beginning after finishing.
        /// </summary>
        public bool IsLooping { get; set; }
    }
}
