using Raylib_cs;
using Arch.Core;
using Isotope.Core;
using System.Numerics;

public class GameLoop
{
    private const double TickRate = 1.0 / 60.0;
    private const int TileSize = 32;

    private World _world;
    private TileRegistry _tileRegistry;
    private Map _map;
    private CameraService _cameraService;

    public void Run()
    {
        const int screenWidth = 1280;
        const int screenHeight = 720;
        Raylib.InitWindow(screenWidth, screenHeight, "ISOTOPE ENGINE [DEBUG]");
        Raylib.SetTargetFPS(144);

        _world = World.Create();
        _cameraService = new CameraService(screenWidth, screenHeight);

        // --- Map Initialization ---
        _tileRegistry = new TileRegistry();
        _tileRegistry.RegisterTiles();

        _map = new Map(50, 30);
        _map.GenerateSimpleTestMap(_tileRegistry);

        double accumulator = 0.0;
        double lastTime = Raylib.GetTime();

        while (!Raylib.WindowShouldClose())
        {
            // --- Input and Updates ---
            _cameraService.Update();

            double currentTime = Raylib.GetTime();
            double frameTime = currentTime - lastTime;
            lastTime = currentTime;

            if (frameTime > 0.25) frameTime = 0.25;

            accumulator += frameTime;

            while (accumulator >= TickRate)
            {
                // Logic updates here
                accumulator -= TickRate;
            }

            double alpha = accumulator / TickRate;

            // --- Rendering ---
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Raylib.BeginMode2D(_cameraService.Camera);
            RenderMap();
            Raylib.EndMode2D();

            Raylib.DrawFPS(10, 10);
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    private void RenderMap()
    {
        for (int y = 0; y < _map.Height; y++)
        {
            for (int x = 0; x < _map.Width; x++)
            {
                int index = y * _map.Width + x;
                ushort tileId = _map.Tiles[index];
                var tileDef = _tileRegistry.GetTile(tileId);

                Color color = tileDef.IsSolid ? Color.Gray : Color.DarkGray;

                Raylib.DrawRectangle(x * TileSize, y * TileSize, TileSize, TileSize, color);
            }
        }
    }
}
