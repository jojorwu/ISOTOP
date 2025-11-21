using Arch.Core;
using Isotope.Core.Components;
using Isotope.Core.Components.Animation;
using Raylib_cs;

namespace Isotope.Core.Systems;

/// <summary>
/// A system that processes entities with <see cref="AnimatedSpriteComponent"/>
/// to update their animations and apply the correct frame to their <see cref="SpriteComponent"/>.
/// </summary>
public class AnimationSystem
{
    private readonly World _world;

    public AnimationSystem(World world)
    {
        _world = world;
    }

    public void Update(in float dt)
    {
        var query = new QueryDescription().WithAll<AnimatedSpriteComponent, SpriteComponent>();

        _world.Query(in query, (ref AnimatedSpriteComponent anim, ref SpriteComponent sprite) =>
        {
            if (!anim.IsPlaying || string.IsNullOrEmpty(anim.CurrentAnimation) || !anim.Animations.TryGetValue(anim.CurrentAnimation, out var currentAnim))
            {
                return;
            }

            anim.FrameTimer += dt;

            if (anim.FrameTimer >= currentAnim.FrameDuration)
            {
                anim.FrameTimer -= currentAnim.FrameDuration;
                anim.CurrentFrameIndex++;

                if (anim.CurrentFrameIndex >= currentAnim.Frames.Count)
                {
                    if (currentAnim.IsLooping)
                    {
                        anim.CurrentFrameIndex = 0;
                    }
                    else
                    {
                        anim.CurrentFrameIndex = currentAnim.Frames.Count - 1;
                        anim.IsPlaying = false;
                    }
                }
            }

            // Assuming the sprite sheet is a horizontal strip for now.
            // A more robust implementation would need sprite sheet metadata.
            if (sprite.Texture.Id == 0 || currentAnim.Frames.Count == 0) return;

            var frame = currentAnim.Frames[anim.CurrentFrameIndex];
            var frameWidth = sprite.Texture.Width / currentAnim.Frames.Count; // This is a simplification
            var frameHeight = sprite.Texture.Height;

            sprite.SourceRect = new Rectangle(frame * frameWidth, 0, frameWidth, frameHeight);
        });
    }
}
