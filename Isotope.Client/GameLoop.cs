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
using Isotope.Core.Components.Animation;
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
    private AnimationSystem _animationSystem;
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
        _animationSystem = new AnimationSystem(World);
        _interactionSystem = new InteractionSystem(World, _scriptSystem, Camera);

        _editor = new EditorLayer(World);
        _editor.Init();

        SpawnEntities();
    }

    public void Update()
    {
        // Using a fixed timestep for physics and logic
        _accumulator += Raylib.GetFrameTime();

        while (_accumulator >= TickRate)
        {
            var deltaTime = (float)TickRate;
            _inputSystem.Update(in deltaTime);
            _hierarchySystem.Update(in deltaTime);
            _physicsSystem.Update(in deltaTime);
            _atmosSystem.Update(deltaTime);
            _scriptSystem.Update(in deltaTime);
            _animationSystem.Update(in deltaTime);
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
                _entityRenderSystem.Update(in Camera);
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
        var player = PrototypeManager.Spawn(World, "Player", new Vector2(250, 250));
        var toolbox = PrototypeManager.Spawn(World, "Toolbox", new Vector2(350, 250));

        // TODO: This is a temporary solution to the problem of loading textures for spawned entities.
        // A better solution would be to have a proper asset management system that handles this automatically.
        ref var playerSprite = ref World.Get<SpriteComponent>(player);
        playerSprite.Texture = ResourceManager.GetTexture(playerSprite.TexturePath);

        ref var toolboxSprite = ref World.Get<SpriteComponent>(toolbox);
        toolboxSprite.Texture = ResourceManager.GetTexture(toolboxSprite.TexturePath);
    }

    private void RenderMap()
    {
        var cameraWorldTopLeft = Raylib.GetScreenToWorld2D(new Vector2(0, 0), Camera);
        var cameraWorldBottomRight = Raylib.GetScreenToWorld2D(new Vector2(1280, 720), Camera);

        int startX = (int)(cameraWorldTopLeft.X / WorldMap.TILE_SIZE) - 1;
        int startY = (int)(cameraWorldTopLeft.Y / WorldMap.TILE_SIZE) - 1;
        int endX = (int)(cameraWorldBottomRight.X / WorldMap.TILE_SIZE) + 1;
        int endY = (int)(cameraWorldBottomRight.Y / WorldMap.TILE_SIZE) + 1;

        startX = Math.Max(0, startX);
        startY = Math.Max(0, startY);
        endX = Math.Min(Map.Width, endX);
        endY = Math.Min(Map.Height, endY);

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
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
