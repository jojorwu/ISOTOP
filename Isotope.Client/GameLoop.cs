using System;
using Arch.Core;
using Isotope.Client.Rendering;
using Isotope.Client.Systems;
using Isotope.Core.Components;
using Isotope.Core.Map;
using Isotope.Core.Prototypes;
using Isotope.Core.Scripting;
using Isotope.Core.Systems;
using Isotope.Core.Tiles;
using Isotope.Scripting.NLua;
using Raylib_cs;
using Isotope.Client;
using System.Collections.Generic;
using System.Numerics;

public class GameLoop
{
    private const double TickRate = 1.0 / 60.0;

    public World World { get; private set; }
    public Camera2D Camera { get; private set; }

    private WorldMap _map;
    private HierarchySystem _hierarchySystem;
    private PhysicsSystem _physicsSystem;
    private InputSystem _inputSystem;
    private EntityRenderSystem _entityRenderSystem;
    private LightingPass _lightingPass;
    private LuaSystem _luaSystem;
    private EngineApi _engineApi;

    private Dictionary<string, Texture2D> _tileTextureCache = new();

    private double _accumulator = 0.0;

    public void Init()
    {
        World = World.Create();
        var camera = new Camera2D
        {
            Zoom = 1.0f,
            Target = new Vector2(0, 0),
            Offset = new Vector2(1280 / 2.0f, 720 / 2.0f),
        };
        Camera = camera;

        LoadContent();
        InitializeMap();
        SpawnEntities();

        _hierarchySystem = new HierarchySystem(World);
        _physicsSystem = new PhysicsSystem(World, _map);
        _inputSystem = new InputSystem(World);
        _entityRenderSystem = new EntityRenderSystem(World);
        _lightingPass = new LightingPass(1280, 720);
    }

    public void Update()
    {
        // Using a fixed timestep for physics and logic
        _accumulator += Raylib.GetFrameTime();

        HandleInput();

        while (_accumulator >= TickRate)
        {
            var deltaTime = (float)TickRate;
            _inputSystem.Update(in deltaTime);
            _hierarchySystem.Update(in deltaTime);
            _physicsSystem.Update(in deltaTime);
            _accumulator -= TickRate;
        }

        var playerQuery = new QueryDescription().WithAll<PlayerTag, TransformComponent>();
        World.Query(in playerQuery, (ref TransformComponent transform) =>
        {
            var camera = Camera;
            camera.Target = transform.WorldPosition;
            Camera = camera;
        });
    }

    public void Render()
    {
        Raylib.BeginMode2D(Camera);
        RenderMap();
        _entityRenderSystem.Update(0f);
        Raylib.EndMode2D();

        _lightingPass.DrawLights(World, _map, Camera);
        _lightingPass.RenderToScreen();
    }

    public void Shutdown()
    {
        UnloadContent();
    }

    private void LoadContent()
    {
        _engineApi = new EngineApi(World);
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
        PrototypeManager.Spawn(World, "Player", new Vector2(250, 250));
        PrototypeManager.Spawn(World, "Toolbox", new Vector2(350, 250));
    }

    private void HandleInput()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.E))
        {
            Entity playerEntity = Entity.Null;
            var playerQuery = new QueryDescription().WithAll<PlayerTag>();
            World.Query(in playerQuery, (Entity entity) => { playerEntity = entity; });

            if (playerEntity == Entity.Null) return;

            Entity closestToolbox = Entity.Null;
            float closestDist = 100f;

            var toolboxQuery = new QueryDescription().WithAll<SpriteComponent>();
            World.Query(in toolboxQuery, (Entity entity, ref TransformComponent transform, ref SpriteComponent sprite) =>
            {
                if (sprite.TexturePath.Contains("toolbox"))
                {
                    float dist = Vector2.Distance(World.Get<TransformComponent>(playerEntity).WorldPosition, transform.WorldPosition);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestToolbox = entity;
                    }
                }
            });

            if (closestToolbox != Entity.Null)
            {
                ref var transform = ref World.Get<TransformComponent>(closestToolbox);
                ref var sprite = ref World.Get<SpriteComponent>(closestToolbox);

                transform.Parent = playerEntity;
                transform.LocalPosition = new Vector2(0, -20);
                sprite.Visible = false;
            }
        }
    }

    private void RenderMap()
    {
        for (int y = 0; y < _map.Height; y++)
        {
            for (int x = 0; x < _map.Width; x++)
            {
                ref readonly var tile = ref _map.GetTile(x, y);

                if (tile.FloorId != 0)
                {
                    var floorDef = TileRegistry.Get(tile.FloorId);
                    if (floorDef != null)
                    {
                        var texture = GetTileTexture(floorDef.TexturePath);
                        Raylib.DrawTexture(texture, x * WorldMap.TILE_SIZE, y * WorldMap.TILE_SIZE, Color.White);
                    }
                }

                if (tile.WallId != 0)
                {
                    var wallDef = TileRegistry.Get(tile.WallId);
                    if (wallDef != null)
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
        if (_tileTextureCache.TryGetValue(path, out var texture))
        {
            return texture;
        }

        var newTexture = Raylib.LoadTexture(path);
        _tileTextureCache[path] = newTexture;
        return newTexture;
    }
}
