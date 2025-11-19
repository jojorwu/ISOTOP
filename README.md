# Isotope Engine

Isotope is a high-performance, data-driven 2D game engine built with C# and Raylib. It is designed to be a flexible foundation for tile-based games, with a focus on performance and moddability.

## Features

-   **Entity-Component-System (ECS)**: Built on the `Arch` ECS framework for high performance and a clean, data-driven architecture.
-   **Dynamic 2D Lighting**: A dynamic lighting system with shadow casting.
-   **Lua Scripting**: Easily add new game logic and content with Lua scripts.
-   **Data-Driven Design**: Define entities and their components in YAML files for easy content creation.
-   **In-Game Editor**: A simple in-game editor for creating and editing maps.

## Getting Started

### Prerequisites

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   A C# IDE, such as Visual Studio or JetBrains Rider.

### Building and Running

1.  Clone the repository:
    ```bash
    git clone https://github.com/your-username/isotope.git
    ```
2.  Open the `Isotope.sln` file in your IDE.
3.  Build the solution. This will restore the necessary NuGet packages.
4.  Set the `Isotope.Client` project as the startup project and run it.

## Documentation

The engine is fully documented with XML comments in the source code. In addition, the `docs` directory contains a series of Markdown files that provide a high-level overview of the engine's architecture and key systems:

-   [**01-architecture.md**](./docs/01-architecture.md): An overview of the project structure and key systems.
-   [**02-rendering.md**](./docs/02-rendering.md): An explanation of the rendering pipeline and lighting system.
-   [**03-ecs-guide.md**](./docs/03-ecs-guide.md): A guide to the Entity-Component-System implementation.
-   [**04-scripting-api.md**](./docs/04-scripting-api.md): Documentation for the Lua scripting API.

## Contributing

Contributions are welcome! If you find a bug or have an idea for a new feature, please open an issue or submit a pull request.
