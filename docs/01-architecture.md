# Architecture Overview

This document provides a high-level overview of the Isotope engine's architecture.

## Project Structure

The engine is divided into four main projects:

-   **`Isotope.Core`**: This is the heart of the engine. It contains all the core systems and data structures that are shared between the client and server. This includes the Entity-Component-System (ECS) framework, the tile and map systems, physics, and the base scripting API. The goal of this project is to be as game-agnostic as possible, providing a solid foundation for any 2D tile-based game.

-   **`Isotope.Client`**: This project is responsible for everything that the player sees and interacts with. It handles rendering, input, and the user interface (including the in-game editor). It references `Isotope.Core` to get access to the core engine systems.

-   **`Isotope.Server`**: This project is currently a placeholder, but it is intended to handle the server-side logic for a multiplayer game. It would reference `Isotope.Core` to use the same core systems as the client, but it would not have any rendering or input code.

-   **`Isotope.Scripting`**: This project provides the Lua scripting implementation. It uses the NLua library to embed a Lua interpreter and exposes the `EngineApi` from `Isotope.Core` to the Lua environment. This allows game logic to be written in Lua, making it easy to mod the game without recompiling the engine.

## Key Systems

-   **Entity-Component-System (ECS)**: The engine is built around the ECS pattern using the `Arch` library. This allows for a data-driven design that is both flexible and performant.

-   **Rendering**: The client uses the Raylib library for rendering. The rendering pipeline is designed to be extensible, with a `LightingPass` that provides dynamic 2D lighting and shadows.

-   **Physics**: A simple physics system is implemented in `Isotope.Core` that handles entity movement and collision with the tilemap.

-   **Scripting**: Game logic can be written in Lua, thanks to the `Isotope.Scripting` project. This allows for a great deal of flexibility and moddability.
