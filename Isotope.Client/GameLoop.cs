using System;
using Arch.Core;
using Isotope.Client.Editor;
using Isotope.Client.Rendering;
using Isotope.Client.Systems;
using Isotope.Core.Components;
using Isotope.Core.Map;
using Isotope.Core.Prototypes;
using Isotope.Core.Scripting;
using Isotope.Core.Systems;
using Isotope.Core.Tiles;
using Isotope.Scripting.Lua;
using Raylib_cs;
using sbf.raylib.imgui;
using System.Collections.Generic;
using System.Numerics;

/// <summary>
/// The main game loop, responsible for managing the game state, systems, and rendering.
/// </summary>
public class GameLoop
{
    private const double TickRate = 1.0 / 60.0;
    private const int ScreenWidth = 1280;
    private const int ScreenHeight = 720;

    private World _world;
    private WorldMap _map;
    private Camera2D _camera;

    private HierarchySystem _hierarchySystem;
    private PhysicsSystem _physicsSystem;
    private InputSystem _inputSystem;
    private EntityRenderSystem _entityRenderSystem;
    private LightingPass _lightingPass;
    private EditorSystem _editorSystem;
    private LuaSystem _luaSystem;
    private EngineApi _engineApi;

    private Dictionary<string, Texture2D> _tileTextureCache = new();

    /// <summary>
    /// Runs the main game loop.
    /// </summary>
    public void Run()
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, "ISOTOPE ENGINE [DEBUG]");
        Raylib.SetTargetFPS(144);
        RlImGui.Setup(true);

        _world = World.Create();
        _camera = new Camera2D
        {
            Zoom = 1.0f,
            Target = new Vector2(0,0),
            Offset = new Vector2(ScreenWidth / 2.0f, ScreenHeight / 2.0f),
        };

        LoadContent();
        InitializeMap();
        SpawnEntities();

        _hierarchySystem = new HierarchySystem(_world);
        _physicsSystem = new PhysicsSystem(_world, _map);
        _inputSystem = new InputSystem(_world);
        _entityRenderSystem = new EntityRenderSystem(_world);
        _lightingPass = new LightingPass(ScreenWidth, ScreenHeight);
        _editorSystem = new EditorSystem(_map, _world);

        double accumulator = 0.0;
        double lastTime = Raylib.GetTime();

        while (!Raylib.WindowShouldClose())
        {
            double currentTime = Raylib.GetTime();
            double frameTime = currentTime - lastTime;
            lastTime = currentTime;
            if (frameTime > 0.25) frameTime = 0.25;
            accumulator += frameTime;

            HandleInput();

            while (accumulator >= TickRate)
            {
                var deltaTime = (float)TickRate;
                _inputSystem.Update(in deltaTime);
                _hierarchySystem.Update(in deltaTime);
                _physicsSystem.Update(in deltaTime);
                accumulator -= TickRate;
            }

            var playerQuery = new QueryDescription().WithAll<PlayerTag, TransformComponent>();
            _world.Query(in playerQuery, (ref TransformComponent transform) => {
                _camera.Target = transform.WorldPosition;
            });

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Raylib.BeginMode2D(_camera);
            RenderMap();
            _entityRenderSystem.Update(0f);
            Raylib.EndMode2D();

            _lightingPass.DrawLights(_world, _map, _camera);
            _lightingPass.RenderToScreen();

            RlImGui.Begin();
            _editorSystem.Update(_camera);
            RlImGui.End();

            Raylib.DrawFPS(10, 10);
            Raylib.EndDrawing();
        }

        UnloadContent();
        Raylib.CloseWindow();
    }

    private void LoadContent()
    {
        _engineApi = new EngineApi(_world);
        _luaSystem = new LuaSystem(_engineApi);

        TileRegistry.Register(new TileDefinition { Name = "Floor_Plating", TexturePath = "Content/floors/plating.png" });
        TileRegistry.Register(new TileDefinition { Name = "Wall_Reinforced", TexturePath = "Content/walls/r_wall.png", IsSolid = true, IsOpaque = true });

        PrototypeLoader.LoadPrototypesFromDirectory("Data/Prototypes");
        _luaSystem.LoadScriptsFromDirectory("Mods");
    }

    private void UnloadContent()
    {
        foreach (var texture in _tileTextureCache.Values)
        {
            Raylib.UnloadTexture(texture);
        }
        _tileTextureCache.Clear();
        _lightingPass.Unload();
        RlImGui.Shutdown();
    }

    private void InitializeMap()
    {
        _map = new WorldMap(50, 50);
        for (int i = 0; i < _map.GetRawTiles().Length; i++)
        {
            _map.GetRawTiles()[i].FloorId = 1;
        }
    }

    private void SpawnEntities()
    {
        PrototypeManager.Spawn(_world, "Player", new Vector2(250, 250));
        PrototypeManager.Spawn(_world, "Toolbox", new Vector2(350, 250));
    }

    private void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.E))
        {
            Entity playerEntity = Entity.Null;
            var playerQuery = new QueryDescription().WithAll<PlayerTag>();
            _world.Query(in playerQuery, (Entity entity) => { playerEntity = entity; });

            if (playerEntity == Entity.Null) return;

            Entity closestToolbox = Entity.Null;
            float closestDist = 100f;

            var toolboxQuery = new QueryDescription().WithAll<SpriteComponent>();
            _world.Query(in toolboxQuery, (Entity entity, ref TransformComponent transform, ref SpriteComponent sprite) =>
            {
                if(sprite.TexturePath.Contains("toolbox"))
                {
                    float dist = Vector2.Distance(_world.Get<TransformComponent>(playerEntity).WorldPosition, transform.WorldPosition);
                    if(dist < closestDist)
                    {
                        closestDist = dist;
                        closestToolbox = entity;
                    }
                }
            });

            if(closestToolbox != Entity.Null)
            {
                ref var transform = ref _world.Get<TransformComponent>(closestToolbox);
                ref var sprite = ref _world.Get<SpriteComponent>(closestToolbox);

                transform.Parent = playerEntity;
                transform.LocalPosition = new Vector2(0, -20);
                sprite.Visible = false;
            }
        }
    }

    private void RenderMap()
    {
        for (int y = 0; y < _map.Height; y++) {
            for (int x = 0; x < _map.Width; x++) {
                ref readonly var tile = ref _map.GetTile(x, y);

                if (tile.FloorId != 0)
                {
                    var floorDef = TileRegistry.Get(tile.FloorId);
                    if(floorDef != null)
                    {
                        var texture = GetTileTexture(floorDef.TexturePath);
                        Raylib.DrawTexture(texture, x * WorldMap.TILE_SIZE, y * WorldMap.TILE_SIZE, Color.White);
                    }
                }

                if (tile.WallId != 0)
                {
                    var wallDef = TileRegistry.Get(tile.WallId);
                     if(wallDef != null)
                    {
                        var texture = GetTileTexture(wallDef.TexturePath);
                        Raylib.DrawTexture(texture, x * WorldMap.TILE_SIZE, y * WorldMap.TILE_SIZE, Color.White);
                    }
                }
            }
        }
    }

    private Texture2D GetTileTexture(string path)
    {
        if (_tileTextureCache.TryGetValue(path, out var texture)) {
            return texture;
        }

        var newTexture = Raylib.LoadTexture(path);
        _tileTextureCache[path] = newTexture;
        return newTexture;
    }
}
