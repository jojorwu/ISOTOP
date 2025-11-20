using System;
using System.IO;
using System.Numerics;
using Arch.Core;
using ImGuiNET;
using Raylib_cs;
using Isotope.Client.Editor.Panels;
using Isotope.Core.Map;
using Isotope.Core.Components;

namespace Isotope.Client.Editor
{
    public class EditorLayer
    {
        private EditorContext _ctx;
        private Core.Systems.DoorSystem _doorSystem;
        private HierarchyPanel _hierarchyPanel;
        private InspectorPanel _inspectorPanel;
        private ConsolePanel _consolePanel;
        private ProjectPanel _projectPanel;
        private ToolbarPanel _toolbarPanel;
        private TilePalettePanel _tilePalettePanel;
        private ModebarPanel _modebarPanel;
        private Commands.HistoryManager _historyManager;

        private bool _showHierarchy = true;
        private bool _showInspector = true;
        private bool _showConsole = true;
        private bool _showProject = true;
        private bool _showToolbar = true;
        private bool _showTilePalette = true;
        private bool _showModebar = true;

        private RenderTexture2D _gameViewTexture;
        private bool _isPlaying = false;
        private Vector2 _lastWindowSize = new Vector2(800, 600);
        private bool _isDragging = false;
        private Vector2 _dragOffset;

        public EditorLayer(World world, Core.Systems.DoorSystem doorSystem)
        {
            _ctx = new EditorContext { World = world };
            _doorSystem = doorSystem;
            _hierarchyPanel = new HierarchyPanel(_ctx);
            _inspectorPanel = new InspectorPanel(_ctx);
            _consolePanel = new ConsolePanel();
            _projectPanel = new ProjectPanel(Directory.GetCurrentDirectory());
            _toolbarPanel = new ToolbarPanel(_ctx);
            _tilePalettePanel = new TilePalettePanel(_ctx);
            _modebarPanel = new ModebarPanel(_ctx);
            _historyManager = new Commands.HistoryManager();

            _gameViewTexture = Raylib.LoadRenderTexture(800, 600);
        }

        public void Init() { }

        public void Draw(World world, WorldMap map, Camera2D gameCamera, Action gameUpdate, Action gameRender, Action spawnEntities)
        {
            if (_isPlaying)
            {
                gameUpdate.Invoke();
            }
            else
            {
                EditorCameraUpdate();
            }

            Raylib.BeginTextureMode(_gameViewTexture);
            Raylib.ClearBackground(Color.Black);
            gameRender.Invoke();
            if (!_isPlaying) DrawEditorGizmos(world, gameCamera, map);
            Raylib.EndTextureMode();

            RlImGui.Begin(Raylib.GetFrameTime());
            DrawDockSpace();
            DrawMainMenuBar();

            if(_showHierarchy) _hierarchyPanel.Draw();
            if(_showInspector) _inspectorPanel.Draw();
            if(_showConsole) _consolePanel.Draw();
            if(_showProject) _projectPanel.Draw();
            if(_showToolbar) _toolbarPanel.Draw();
            if(_showTilePalette) _tilePalettePanel.Draw();
            if(_showModebar) _modebarPanel.Draw();

            DrawSceneView(world, gameCamera, map);
            HandleHotkeys();

            RlImGui.End();
        }

        private void DrawMainMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New Scene")) _ctx.World.Clear();
                    if (ImGui.MenuItem("Open Scene...")) Core.Serialization.SceneSerializer.LoadScene(_ctx.World, "assets/scenes/level1.json");
                    if (ImGui.MenuItem("Save Scene")) Core.Serialization.SceneSerializer.SaveScene(_ctx.World, "assets/scenes/level1.json");
                    ImGui.Separator();
                    if (ImGui.MenuItem("Exit")) Raylib.CloseWindow();
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo", "Ctrl+Z")) _historyManager.Undo();
                    if (ImGui.MenuItem("Redo", "Ctrl+Y")) _historyManager.Redo();
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("View"))
                {
                    ImGui.MenuItem("Hierarchy", "", ref _showHierarchy);
                    ImGui.MenuItem("Inspector", "", ref _showInspector);
                    ImGui.MenuItem("Console", "", ref _showConsole);
                    ImGui.MenuItem("Project", "", ref _showProject);
                    ImGui.MenuItem("Toolbar", "", ref _showToolbar);
                    ImGui.MenuItem("Tile Palette", "", ref _showTilePalette);
                    ImGui.MenuItem("Modebar", "", ref _showModebar);
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("GameObject"))
                {
                    if (ImGui.MenuItem("Create Empty"))
                    {
                        var e = _ctx.World.Create(new TransformComponent(), new NameComponent { Name = "Empty" });
                        _ctx.Select(e);
                    }
                    if (ImGui.MenuItem("Light Source"))
                    {
                        var e = _ctx.World.Create(
                            new TransformComponent { Position = GetCameraCenter() },
                            new LightSource { Color = Color.White, Radius = 200 },
                            new NameComponent { Name = "Point Light" }
                        );
                        _ctx.Select(e);
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }

        private Vector2 GetCameraCenter()
        {
            // This is a placeholder. A proper implementation would get the actual camera position.
            return new Vector2(400, 300);
        }

        private void HandleHotkeys()
        {
            if (ImGui.GetIO().KeyCtrl)
            {
                if (ImGui.IsKeyPressed(ImGuiKey.Z)) _historyManager.Undo();
                if (ImGui.IsKeyPressed(ImGuiKey.Y)) _historyManager.Redo();
            }
        }

        private void DrawSceneView(World world, Camera2D gameCamera, WorldMap map)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.Begin("Scene View");

            Vector2 size = ImGui.GetContentRegionAvail();

            if (size.X > 0 && size.Y > 0)
            {
                if (size.X != _lastWindowSize.X || size.Y != _lastWindowSize.Y)
                {
                    Raylib.UnloadRenderTexture(_gameViewTexture);
                    _gameViewTexture = Raylib.LoadRenderTexture((int)size.X, (int)size.Y);
                    _lastWindowSize = size;
                }

                RlImGui.ImageRenderTextureFit(_gameViewTexture, size.X, size.Y);

                if (ImGui.IsItemHovered())
                {
                    UpdateSceneInput(world, gameCamera, map);
                }
            }

            ImGui.End();
            ImGui.PopStyleVar();
        }

        private void DrawDockSpace()
        {
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);
        }

        private void EditorCameraUpdate()
        {
            // Placeholder for editor camera logic
        }

        private void DrawGrid(int spacing, Color color)
        {
            // Very simple grid drawing. A more optimized version would only draw visible lines.
            for (int i = 0; i < 5000; i += spacing)
            {
                Raylib.DrawLine(i, 0, i, 5000, color);
                Raylib.DrawLine(0, i, 5000, i, color);
            }
        }

        private void DrawEditorGizmos(World world, Camera2D gameCamera, WorldMap map)
        {
            DrawGrid(32, Color.Gray);

            if (_ctx.CurrentMode == EditorMode.Tile)
            {
                Vector2 mouseWorld = GetMousePositionInGame(gameCamera);
                var (gx, gy) = map.WorldToGrid(mouseWorld.X, mouseWorld.Y);
                _toolbarPanel.ActiveTool.OnDrawGizmos(map, new Vector2(gx, gy));
            }
            else if (_ctx.CurrentMode == EditorMode.Object)
            {
                // Draw selection gizmo for any object
                if (_ctx.SelectedEntity != Entity.Null && world.IsAlive(_ctx.SelectedEntity))
                {
                    ref var t = ref world.Get<TransformComponent>(_ctx.SelectedEntity);

                    float w = 32, h = 32;
                    if (_ctx.World.Has<SpriteComponent>(_ctx.SelectedEntity))
                    {
                        var s = _ctx.World.Get<SpriteComponent>(_ctx.SelectedEntity);
                        var tex = Rendering.ResourceManager.GetTexture(s.TexturePath);
                        w = tex.Width;
                        h = tex.Height;
                    }

                    float alpha = (float)Math.Abs(Math.Sin(Raylib.GetTime() * 4));
                    Color selectionColor = new Color(255, 255, 0, (int)(100 + alpha * 155));

                    Raylib.DrawRectangleLinesEx(new Rectangle(t.Position.X, t.Position.Y, w, h), 2.0f, selectionColor);

                    if (_ctx.World.Has<NameComponent>(_ctx.SelectedEntity))
                    {
                        var name = _ctx.World.Get<NameComponent>(_ctx.SelectedEntity).Name;
                        Raylib.DrawText(name, (int)t.Position.X, (int)t.Position.Y - 20, 10, Color.Yellow);
                    }
                }

                // Draw gizmos for all light sources
                var query = new QueryDescription().WithAll<TransformComponent, LightSource>();
                world.Query(in query, (Entity e, ref TransformComponent t, ref LightSource l) =>
                {
                    Color gizmoColor = (_ctx.SelectedEntity == e) ? Color.Yellow : new Color(255, 255, 0, 100);

                    Raylib.DrawCircleLines((int)t.Position.X, (int)t.Position.Y, 10, gizmoColor);
                    Raylib.DrawCircleLines((int)t.Position.X, (int)t.Position.Y, l.Radius, new Color(255, 255, 0, 50));

                    if (_ctx.SelectedEntity == e)
                    {
                        Raylib.DrawCircle((int)(t.Position.X + l.Radius), (int)t.Position.Y, 5, Color.White);
                    }
                });
            }
        }

        public Vector2 GetMousePositionInGame(Camera2D gameCamera)
        {
            Vector2 screenMouse = ImGui.GetMousePos();
            Vector2 windowPos = ImGui.GetItemRectMin();
            Vector2 localMouse = screenMouse - windowPos;
            return Raylib.GetScreenToWorld2D(localMouse, gameCamera);
        }

        private Entity FindEntityAt(Vector2 worldPos)
        {
            Entity result = Entity.Null;
            var query = new QueryDescription().WithAll<TransformComponent, SpriteComponent>();

            _ctx.World.Query(in query, (Entity e, ref TransformComponent t, ref SpriteComponent s) =>
            {
                var tex = Rendering.ResourceManager.GetTexture(s.TexturePath);
                Rectangle rect = new Rectangle(t.Position.X, t.Position.Y, tex.Width, tex.Height);

                if (Raylib.CheckCollisionPointRec(worldPos, rect))
                {
                    result = e;
                }
            });

            return result;
        }

        private void UpdateSceneInput(World world, Camera2D gameCamera, WorldMap map)
        {
            if (ImGui.GetIO().WantCaptureMouse) return;

            if (_ctx.CurrentMode == EditorMode.Tile)
            {
                Vector2 mouseWorld = GetMousePositionInGame(gameCamera);
                var (gx, gy) = map.WorldToGrid(mouseWorld.X, mouseWorld.Y);
                Vector2 gridPos = new Vector2(gx, gy);
                _toolbarPanel.ActiveTool.OnUpdate(world, map, gridPos, _historyManager, _ctx);
            }
            else if (_ctx.CurrentMode == EditorMode.Object)
            {
                Vector2 mouseWorld = GetMousePositionInGame(gameCamera);

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    Entity clickedEntity = FindEntityAt(mouseWorld);

                    if (clickedEntity != Entity.Null)
                    {
                        if (world.Has<DoorComponent>(clickedEntity))
                        {
                            _doorSystem.ToggleDoor(clickedEntity);
                        }

                        _ctx.Select(clickedEntity);
                        _isDragging = true;
                        ref var t = ref _ctx.World.Get<TransformComponent>(clickedEntity);
                        _dragOffset = t.Position - mouseWorld;
                    }
                    else
                    {
                        _ctx.Select(Entity.Null);
                    }
                }

                if (_isDragging && ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    if (_ctx.SelectedEntity != Entity.Null && _ctx.World.IsAlive(_ctx.SelectedEntity))
                    {
                        ref var t = ref _ctx.World.Get<TransformComponent>(_ctx.SelectedEntity);

                        if (ImGui.GetIO().KeyCtrl)
                        {
                            float snap = 32.0f;
                            Vector2 rawPos = mouseWorld + _dragOffset;
                            t.Position = new Vector2(
                                (float)Math.Round(rawPos.X / snap) * snap,
                                (float)Math.Round(rawPos.Y / snap) * snap
                            );
                        }
                        else
                        {
                            t.Position = mouseWorld + _dragOffset;
                        }
                    }
                }
                else
                {
                    _isDragging = false;
                }
            }
        }
    }
}
