using System.Collections.Generic;

namespace Isotope.Core.Components.Animation
{
    /// <summary>
    /// A component that manages the animation state of a sprite.
    /// </summary>
    public struct AnimatedSpriteComponent
    {
        /// <summary>
        /// A dictionary containing all available animations for this entity, keyed by name.
        /// </summary>
        public Dictionary<string, Animation> Animations { get; set; }

        /// <summary>
        /// The name of the currently playing animation.
        /// </summary>
        public string CurrentAnimation { get; set; }

        /// <summary>
        /// The index of the current frame within the current animation's frame list.
        /// </summary>
        public int CurrentFrameIndex { get; set; }

        /// <summary>
        /// A timer to track the elapsed time for the current frame.
        /// </summary>
        public float FrameTimer { get; set; }

        /// <summary>
        /// Whether the animation is currently playing.
        /// </summary>
        public bool IsPlaying { get; set; }
    }
}
