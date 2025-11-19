using System;
using System.Numerics;
using Arch.Core;
using ImGuiNET;
using Raylib_cs;
using Isotope.Client;

namespace Isotope.Client.Editor
{
    public class EditorLayer
    {
        // Это наш "виртуальный экран" игры
        private RenderTexture2D _gameViewTexture;
        private bool _isPlaying = false;

        // Размер окна Scene View (чтобы ресайзить игру)
        private Vector2 _lastWindowSize = new Vector2(800, 600);

        public void Init()
        {
            // Создаем текстуру начального размера
            _gameViewTexture = Raylib.LoadRenderTexture(800, 600);
        }

        public void Draw(World world, Action gameUpdate, Action gameRender)
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
            if (!_isPlaying) DrawEditorGizmos();

            Raylib.EndTextureMode();

            // --- 3. РЕНДЕР ИНТЕРФЕЙСА (IMGUI) ---
            RlImGui.Begin(Raylib.GetFrameTime());
            DrawDockSpace(); // Включаем докинг окон

            DrawToolbar();   // Кнопки Play/Stop
            DrawSceneView(); // Само окно с игрой
            DrawInspector(world); // Свойства объектов

            RlImGui.End();
        }

        private void DrawToolbar()
        {
            ImGui.Begin("Toolbar", ImGuiWindowFlags.NoDecoration);

            if (!_isPlaying)
            {
                if (ImGui.Button("PLAY (F5)"))
                {
                    // Тут можно сохранить состояние мира (Snapshot), чтобы потом откатить
                    _isPlaying = true;
                }
            }
            else
            {
                if (ImGui.Button("STOP (Esc)"))
                {
                    _isPlaying = false;
                    // Тут перезагружаем карту или восстанавливаем Snapshot
                }
            }

            ImGui.End();
        }

        private void DrawSceneView()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero); // Убираем отступы
            ImGui.Begin("Scene View");

            // Получаем размер текущего окна ImGui
            Vector2 size = ImGui.GetContentRegionAvail();

            // Если размер окна изменился — пересоздаем текстуру игры
            if (size.X != _lastWindowSize.X || size.Y != _lastWindowSize.Y)
            {
                Raylib.UnloadRenderTexture(_gameViewTexture);
                _gameViewTexture = Raylib.LoadRenderTexture((int)size.X, (int)size.Y);
                _lastWindowSize = size;
            }

            // РИСУЕМ ТЕКСТУРУ ИГРЫ ВНУТРИ ОКНА
            RlImGui.ImageRenderTextureFit(_gameViewTexture, size.X, size.Y);

            // Обработка фокуса: если мышь над окном игры, перехватываем ввод
            if (ImGui.IsItemHovered())
            {
                // Тут нужно хитро транслировать координаты мыши
                // ImGui Mouse -> Window Local -> Game World
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

        private void DrawEditorGizmos()
        {
            // Placeholder for editor gizmos
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
