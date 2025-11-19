# Scripting API

This document explains how to use the Lua scripting API to interact with the Isotope engine.

## Lua Environment

The engine uses the NLua library to embed a Lua interpreter. The `LuaSystem` class is responsible for creating the Lua environment and loading all the Lua scripts from the `Mods` directory.

## EngineApi

The `EngineApi` class is exposed to the Lua environment as a global variable named `Game`. This allows you to call C# methods from your Lua scripts.

The `EngineApi` provides the following methods:

-   **`Spawn(prototypeId, x, y)`**: Spawns an entity from a prototype at the specified coordinates.
    -   `prototypeId` (string): The ID of the prototype to spawn.
    -   `x` (number): The x-coordinate to spawn the entity at.
    -   `y` (number): The y-coordinate to spawn the entity at.
    -   Returns: The ID of the spawned entity.

-   **`Log(message)`**: Logs a message to the console.
    -   `message` (string): The message to log.

## Example Script

Here is an example of a simple Lua script that spawns a toolbox entity when the game starts:

```lua
-- This script is executed when the game starts.

-- Spawn a toolbox at the coordinates (100, 100).
local toolboxId = Game.Spawn("Toolbox", 100, 100)

-- Log a message to the console.
Game.Log("Spawned a toolbox with ID " .. toolboxId)
```

This script would be placed in a file with a `.lua` extension in the `Mods` directory. The `LuaSystem` will automatically load and execute it when the game starts.
