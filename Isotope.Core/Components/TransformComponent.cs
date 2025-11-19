using Arch.Core;
using System;
using System.Numerics;

namespace Isotope.Core.Components;

/// <summary>
/// Represents the position and orientation of an entity in the game world.
/// </summary>
public struct TransformComponent
{
    /// <summary>
    /// The position of the entity relative to its parent.
    /// </summary>
    public Vector2 LocalPosition;

    /// <summary>
    /// The parent entity of this entity.
    /// </summary>
    public Entity Parent;

    /// <summary>
    /// The absolute position of the entity in the game world.
    /// </summary>
    [NonSerialized]
    public Vector2 WorldPosition;

    /// <summary>
    /// The rotation of the entity in radians.
    /// </summary>
    [NonSerialized]
    public float Rotation;

    /// <summary>
    /// The layer the entity is on.
    /// </summary>
    [NonSerialized]
    public int Layer;
}
