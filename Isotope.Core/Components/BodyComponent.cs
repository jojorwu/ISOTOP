using System.Numerics;

namespace Isotope.Core.Components;

/// <summary>
/// Represents the physical properties of an entity for collision and movement.
/// </summary>
public struct BodyComponent
{
    /// <summary>
    /// The current velocity of the entity.
    /// </summary>
    public Vector2 Velocity;

    /// <summary>
    /// The size of the hitbox, e.g., 16x16.
    /// </summary>
    public Vector2 Size;

    /// <summary>
    /// A value indicating whether the entity is static, such as a wall.
    /// </summary>
    public bool IsStatic;
}
