# Entity-Component-System (ECS) Guide

This document explains how the Isotope engine uses the Entity-Component-System (ECS) pattern.

## ECS Framework

The engine uses the `Arch` library, a high-performance ECS framework for C#. This allows for a data-driven design that is both flexible and easy to work with.

## Components

Components are simple data structures that hold the data for an entity. They are defined as `struct`s in the `Isotope.Core/Components` directory. For example, the `TransformComponent` looks like this:

```csharp
public struct TransformComponent
{
    public Vector2 LocalPosition;
    public Entity Parent;
    public Vector2 WorldPosition;
    public float Rotation;
    public int Layer;
}
```

## Entities

Entities are simple identifiers that represent an object in the game world. They are created and managed by the `World` object from the `Arch` library.

## Systems

Systems are classes that contain the logic for updating entities. They are defined in the `Isotope.Core/Systems` and `Isotope.Client/Systems` directories. For example, the `PhysicsSystem` is responsible for updating the position of all entities that have a `TransformComponent` and a `BodyComponent`.

## Prototypes

Entities are created from prototypes, which are defined in YAML files in the `Data/Prototypes` directory. This allows for a data-driven approach to entity creation, where new entity types can be defined without having to write any new code.

A prototype definition looks like this:

```yaml
- id: Player
  name: Player
  components:
    - type: Sprite
      values:
        texturePath: "Content/player.png"
    - type: Body
      values:
        width: 16
        height: 16
    - type: PlayerTag
```

This prototype defines a "Player" entity with a `SpriteComponent`, a `BodyComponent`, and a `PlayerTag` component. The `PrototypeManager` is responsible for loading these prototypes and spawning entities from them.
