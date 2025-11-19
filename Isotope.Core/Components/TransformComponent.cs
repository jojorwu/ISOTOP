using Arch.Core;
using System;
using System.Numerics;

namespace Isotope.Core.Components;

public struct TransformComponent
{
    public Vector2 LocalPosition;
    public Entity Parent;

    [NonSerialized]
    public Vector2 WorldPosition;

    [NonSerialized]
    public float Rotation;

    [NonSerialized]
    public int Layer;
}
