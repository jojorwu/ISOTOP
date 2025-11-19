using Raylib_cs;
using Arch.Core;
using Isotope.Core.Map;
using Isotope.Core.Tiles;
using System.Collections.Generic;

public class GameLoop
{
    // 60 тиков в секунду для физики/логики (как в файтингах или CS2)
    const double TickRate = 1.0 / 60.0;
    private World _world;
    private WorldMap _map;
    private Dictionary<string, Texture2D> _textureCache = new();

    public void Run()
    {
        Raylib.InitWindow(1280, 720, "ISOTOPE ENGINE [DEBUG]");
        Raylib.SetTargetFPS(144); // Рендер максимально плавный, отдельно от физики

        _world = World.Create();

        LoadContent();
        InitializeMap();

        double accumulator = 0.0;
        double lastTime = Raylib.GetTime();

        while (!Raylib.WindowShouldClose())
        {
            double currentTime = Raylib.GetTime();
            double frameTime = currentTime - lastTime;
            lastTime = currentTime;

            // Защита от "спирали смерти" (если лагает, не пытаемся догнать вечность)
            if (frameTime > 0.25) frameTime = 0.25;

            accumulator += frameTime;

            // --- ФИЗИКА И ЛОГИКА (FIXED UPDATE) ---
            while (accumulator >= TickRate)
            {
                // Тут считаем столкновения, атмосферу, хим. реакции
                // RunSystems(_world, TickRate);
                accumulator -= TickRate;
            }

            // --- РЕНДЕР (INTERPOLATION) ---
            double alpha = accumulator / TickRate;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            RenderMap();
            Raylib.DrawFPS(10, 10);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    private void LoadContent()
    {
        TileRegistry.Register(new TileDefinition {
            Name = "Floor_Plating",
            TexturePath = "Content/floors/plating.png",
            Friction = 1.0f
        });

        TileRegistry.Register(new TileDefinition {
            Name = "Wall_Reinforced",
            TexturePath = "Content/walls/r_wall.png",
            IsSolid = true,
            IsOpaque = true
        });
    }

    private void InitializeMap()
    {
        _map = new WorldMap(40, 22);
        for (int x = 0; x < _map.Width; x++)
        {
            for (int y = 0; y < _map.Height; y++)
            {
                if (x == 0 || x == _map.Width - 1 || y == 0 || y == _map.Height - 1)
                {
                    _map.SetTileId(x, y, 2); // Wall
                }
                else
                {
                    _map.SetTileId(x, y, 1); // Floor
                }
            }
        }
    }

    private void RenderMap()
    {
        for (int x = 0; x < _map.Width; x++)
        {
            for (int y = 0; y < _map.Height; y++)
            {
                ushort tileId = _map.GetTileId(x, y);
                if (tileId == 0) continue;

                TileDefinition def = TileRegistry.Get(tileId);
                Texture2D texture = GetTexture(def.TexturePath);
                Raylib.DrawTexture(texture, x * 32, y * 32, Color.White);
            }
        }
    }

    private Texture2D GetTexture(string path)
    {
        if (_textureCache.TryGetValue(path, out var texture))
        {
            return texture;
        }

        var newTexture = Raylib.LoadTexture(path);
        _textureCache[path] = newTexture;
        return newTexture;
    }
}
