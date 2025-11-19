using System;
using System.Numerics;
using Arch.Core;
using ImGuiNET;
using Raylib_cs;
using Isotope.Client;
using Isotope.Core.Map;
using Isotope.Core.Components;

namespace Isotope.Client.Editor
{
    public class EditorLayer
    {
        // Это наш "виртуальный экран" игры
        private RenderTexture2D _gameViewTexture;
        private bool _isPlaying = false;
        private string _currentMapPath = "maps/test_level.json";
        private Entity _selectedEntity = Entity.Null;

        // Размер окна Scene View (чтобы ресайзить игру)
        private Vector2 _lastWindowSize = new Vector2(800, 600);

        public void Init()
        {
            // Создаем текстуру начального размера
            _gameViewTexture = Raylib.LoadRenderTexture(800, 600);
        }

        public void Draw(World world, WorldMap map, Camera2D gameCamera, Action gameUpdate, Action gameRender)
        {
            // --- 1. ЛОГИКА (UPDATE) ---
            if (_isPlaying)
            {
                gameUpdate.Invoke(); // Тикаем ECS только если нажали Play
            }
            else
            {
                // Тут можно тикать логику редактора (полет камеры без физики)
                EditorCameraUpdate();
            }

            // --- 2. РЕНДЕР ИГРЫ В ТЕКСТУРУ ---
            Raylib.BeginTextureMode(_gameViewTexture);
            Raylib.ClearBackground(Color.Black);

            gameRender.Invoke(); // Рисуем игру (Тайлы, Спрайты)

            // Если мы в редакторе, рисуем сетку поверх игры
            if (!_isPlaying) DrawEditorGizmos(world);

            Raylib.EndTextureMode();

            // --- 3. РЕНДЕР ИНТЕРФЕЙСА (IMGUI) ---
            RlImGui.Begin(Raylib.GetFrameTime());
            DrawDockSpace(); // Включаем докинг окон

            DrawToolbar(world, map);   // Кнопки Play/Stop
            DrawSceneView(world, gameCamera); // Само окно с игрой
            DrawInspector(world); // Свойства объектов

            RlImGui.End();
        }

        private void DrawToolbar(World world, WorldMap map)
        {
            ImGui.Begin("Toolbar", ImGuiWindowFlags.NoDecoration);

            if (!_isPlaying)
            {
                if (ImGui.Button("PLAY (F5)"))
                {
                    MapSerializer.Save(map, "temp_autosave.json");
                    _isPlaying = true;
                }
            }
            else
            {
                if (ImGui.Button("STOP (Esc)"))
                {
                    _isPlaying = false;
                    world.Clear();
                    MapSerializer.Load(map, "temp_autosave.json");
                }
            }

            ImGui.End();
        }

        private void DrawSceneView(World world, Camera2D gameCamera)
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

                if (ImGui.IsItemHovered() && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    Vector2 mousePos = GetMousePositionInGame(gameCamera);
                    var query = new QueryDescription().WithAll<TransformComponent>();
                    world.Query(in query, (Entity entity, ref TransformComponent transform) =>
                    {
                        var rect = new Rectangle(transform.Position.X, transform.Position.Y, 32, 32);
                        if (Raylib.CheckCollisionPointRec(mousePos, rect))
                        {
                            _selectedEntity = entity;
                        }
                    });
                }
            }

            ImGui.End();
            ImGui.PopStyleVar();
        }

        private void DrawInspector(World world)
        {
            ImGui.Begin("Inspector");
            ImGui.Text("Inspector");
            ImGui.End();
        }

        private void DrawDockSpace()
        {
            ImGui.DockSpaceOverViewport();
        }

        private void EditorCameraUpdate()
        {
            // Placeholder for editor camera logic
        }

        private void DrawEditorGizmos(World world)
        {
            if (_selectedEntity != Entity.Null)
            {
                ref var t = ref world.Get<TransformComponent>(_selectedEntity);
                // Рисуем желтую рамку вокруг объекта
                Raylib.DrawRectangleLines((int)t.Position.X, (int)t.Position.Y, 32, 32, Color.Yellow);
            }
        }

        public Vector2 GetMousePositionInGame(Camera2D gameCamera)
        {
            // Позиция мыши на экране
            Vector2 screenMouse = ImGui.GetMousePos();

            // Позиция верхнего левого угла окна "Scene View"
            Vector2 windowPos = ImGui.GetItemRectMin();

            // Локальная позиция внутри окна
            Vector2 localMouse = screenMouse - windowPos;

            // Перевод в мировые координаты через камеру Raylib
            return Raylib.GetScreenToWorld2D(localMouse, gameCamera);
        }
    }
}
