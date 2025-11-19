# Rendering and Lighting

This document explains how the Isotope engine handles rendering and lighting.

## Rendering Pipeline

The rendering pipeline is managed by the `GameLoop` class in the `Isotope.Client` project. It uses the Raylib library for all rendering operations. The rendering process is as follows:

1.  **Clear the screen**: The screen is cleared to a black color at the beginning of each frame.
2.  **Render the map**: The `RenderMap` method in `GameLoop` iterates through all the tiles in the `WorldMap` and draws their corresponding textures.
3.  **Render entities**: The `EntityRenderSystem` is responsible for drawing all entities that have a `SpriteComponent`. It also handles caching of textures to avoid loading them from disk every frame.
4.  **Lighting pass**: The `LightingPass` class is responsible for drawing all light sources and their shadows to a separate render texture, which is then blended with the main scene to create the final lighting effect.

## Lighting System

The lighting system is designed to be both dynamic and performant. It is composed of two main parts:

-   **`LightSource` component**: This component can be added to any entity to make it a light source. It has properties for the light's color, radius, and intensity.

-   **`LightingPass`**: This class manages the rendering of all light sources. It uses a `ShadowCaster` to calculate a light polygon for each light source, taking into account any walls that might block the light. The light polygons are then drawn to a separate render texture, which is blended with the main scene using a `Multiply` blend mode.

## Shadow Casting

The `ShadowCaster` class is responsible for calculating the visibility polygon for a given light source. It works by:

1.  **Collecting wall segments**: It first collects all the wall segments in a certain radius around the light source.
2.  **Casting rays**: It then casts rays from the light source to all the corners of the wall segments.
3.  **Finding intersections**: For each ray, it finds the closest intersection with a wall segment.
4.  **Sorting points**: The intersection points are then sorted by angle around the light source.
5.  **Creating the polygon**: Finally, the sorted points are used to create the light polygon, which is then rendered by the `LightingPass`.
