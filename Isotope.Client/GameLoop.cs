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
using Isotope.Core.Atmos;
using Isotope.Core.Chemistry;

using Isotope.Client.Editor;

public class GameLoop
{
    private const double TickRate = 1.0 / 60.0;

    public World World { get; private set; }
    public Camera2D Camera { get; private set; }
    public WorldMap Map { get; private set; }
    private GasMap _gasMap;
    private EditorLayer _editor;
    private HierarchySystem _hierarchySystem;
    private PhysicsSystem _physicsSystem;
    private InputSystem _inputSystem;
    private EntityRenderSystem _entityRenderSystem;
    private AtmosRenderSystem _atmosRenderSystem;
    private LightingPass _lightingPass;
    private AtmosSystem _atmosSystem;
    private ScriptSystem _scriptSystem;
    private InteractionSystem _interactionSystem;
    private LuaSystem _luaSystem;
    private EngineApi _engineApi;

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

        // Systems must be created before EditorLayer so they can be passed in
        _hierarchySystem = new HierarchySystem(World);
        _physicsSystem = new PhysicsSystem(World, Map);
        _inputSystem = new InputSystem(World);
        _entityRenderSystem = new EntityRenderSystem(World);
        _atmosRenderSystem = new AtmosRenderSystem(_gasMap);
        _lightingPass = new LightingPass(1280, 720);
        _atmosSystem = new AtmosSystem(_gasMap, Map);
        _scriptSystem = new ScriptSystem(World, _engineApi);
        _interactionSystem = new InteractionSystem(World, _scriptSystem, Camera);

        _editor = new EditorLayer(World);
        _editor.Init();

        SpawnEntities();
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
            _atmosSystem.Update(deltaTime);
            _scriptSystem.Update(in deltaTime);
            _interactionSystem.Update();
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
        _editor.Draw(
            World,
            Map,
            Camera,
            gameUpdate: () => Update(),
            gameRender: () => {
                Raylib.BeginMode2D(Camera);
                RenderMap();
                _entityRenderSystem.Update(0f);
                _atmosRenderSystem.Draw(Camera);
                Raylib.EndMode2D();
                _lightingPass.DrawLights(World, Map, Camera);
                _lightingPass.RenderToScreen();
            },
            spawnEntities: () => SpawnEntities()
        );
    }

    public void Shutdown()
    {
        UnloadContent();
    }

    private void LoadContent()
    {
        _engineApi = new EngineApi(World);
        _luaSystem = new LuaSystem(_engineApi);

        ReagentRegistry.RegisterDefaults();
        TileRegistry.Register(new TileDefinition { Name = "Floor_Plating", TexturePath = "Content/floors/plating.png" });
        TileRegistry.Register(new TileDefinition { Name = "Wall_Reinforced", TexturePath = "Content/walls/r_wall.png", IsSolid = true, IsOpaque = true });

        PrototypeLoader.LoadPrototypesFromDirectory("Data/Prototypes");
        _luaSystem.LoadScriptsFromDirectory("Mods");
    }

    private void UnloadContent()
    {
        ResourceManager.UnloadAll();
        _lightingPass.Unload();
    }

    private void InitializeMap()
    {
        Map = new WorldMap(50, 50);
        _gasMap = new GasMap(50, 50);
        for (int i = 0; i < Map.GetRawTiles().Length; i++)
        {
            Map.GetRawTiles()[i].FloorId = 1;
        }
        _gasMap.Plasma[25 * 50 + 25] = 5000;
    }

    public void SpawnEntities()
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
        for (int y = 0; y < Map.Height; y++)
        {
            for (int x = 0; x < Map.Width; x++)
            {
                ref readonly var tile = ref Map.GetTile(x, y);

                if (tile.FloorId != 0)
                {
                    var floorDef = TileRegistry.Get(tile.FloorId);
                    if (floorDef != null)
                    {
                        var texture = ResourceManager.GetTexture(floorDef.TexturePath);
                        Raylib.DrawTexture(texture, x * WorldMap.TILE_SIZE, y * WorldMap.TILE_SIZE, Color.White);
                    }
                }

                if (tile.WallId != 0)
                {
                    var wallDef = TileRegistry.Get(tile.WallId);
                    if (wallDef != null)
                    {
                        var texture = ResourceManager.GetTexture(wallDef.TexturePath);
                        Raylib.DrawTexture(texture, x * WorldMap.TILE_SIZE, y * WorldMap.TILE_SIZE, Color.White);
                    }
                }
            }
        }
    }

}
