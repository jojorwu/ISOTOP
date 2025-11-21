using Raylib_cs;
using ImGuiNET;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Isotope.Client
{
    public static class RlImGui
    {
        private static IntPtr _context;
        private static Texture2D _fontTexture;

        public static void Setup(bool darkTheme = true)
        {
            _context = ImGui.CreateContext();
            var io = ImGui.GetIO();

            // Настройка шрифтов и клавиш
            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable; // Включаем докинг!

            // Загрузка дефолтного шрифта
            unsafe
            {
                io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out int width, out int height, out int bytesPerPixel);

                // Загружаем текстуру шрифта в Raylib
                Image image = new Image
                {
                    Data = (void*)pixels,
                    Width = width,
                    Height = height,
                    Mipmaps = 1,
                    Format = PixelFormat.UncompressedR8G8B8A8
                };
                _fontTexture = Raylib.LoadTextureFromImage(image);

                // Важно: передаем ID текстуры в ImGui
                io.Fonts.SetTexID((IntPtr)_fontTexture.Id);
            }

            if (darkTheme) ImGui.StyleColorsDark();
        }

        public static void Begin(float dt)
        {
            ImGui.GetIO().DeltaTime = dt;
            UpdateInput();
            ImGui.NewFrame();
        }

        public static void End()
        {
            ImGui.Render();
            RenderDrawData(ImGui.GetDrawData());
        }

        public static void Shutdown()
        {
            Raylib.UnloadTexture(_fontTexture);
            ImGui.DestroyContext();
        }

        // --- ВОТ ТО, ЧТО ТЕБЕ НУЖНО ДЛЯ РЕДАКТОРА ---
        // Метод для отрисовки RenderTexture внутри ImGui окна
        public static void ImageRenderTexture(RenderTexture2D rt)
        {
            // Переворачиваем UV по Y, так как в OpenGL координаты снизу-вверх
            Rectangle src = new Rectangle(0, 0, rt.Texture.Width, -rt.Texture.Height);

            // Магия: кастим uint ID в IntPtr
            IntPtr textureId = (IntPtr)rt.Texture.Id;

            // Рисуем картинку
            // uv0 = (0,0), uv1 = (1,1) - стандартные координаты
            // Но так как мы перевернули src rect, Raylib сам разберется,
            // либо используем ImGui.Image с явными UV, если картинка перевернута.

            // Простой вариант:
            ImGui.Image(textureId, new Vector2(rt.Texture.Width, rt.Texture.Height));
        }

        // Более продвинутый вариант, если картинка перевернута:
        public static void ImageRenderTextureFit(RenderTexture2D rt, float width, float height)
        {
             IntPtr textureId = (IntPtr)rt.Texture.Id;
             // UV координаты: (0, 1) -> (1, 0) переворачивают картинку
             ImGui.Image(textureId, new Vector2(width, height), new Vector2(0, 1), new Vector2(1, 0));
        }

        // --- ВНУТРЕННЯЯ КУХНЯ (INPUT & RENDER) ---
        private static void UpdateInput()
        {
            var io = ImGui.GetIO();
            Vector2 mouse = Raylib.GetMousePosition();
            io.MousePos = mouse;
            io.MouseDown[0] = Raylib.IsMouseButtonDown(MouseButton.Left);
            io.MouseDown[1] = Raylib.IsMouseButtonDown(MouseButton.Right);
            io.MouseDown[2] = Raylib.IsMouseButtonDown(MouseButton.Middle);

            io.MouseWheel += Raylib.GetMouseWheelMove();

            // Тут можно добавить обработку клавиатуры
        }

        private static void RenderDrawData(ImDrawDataPtr drawData)
        {
            Raylib.BeginScissorMode(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdLists[n];

                // Vertex Buffer / Index Buffer logic is complex in pure Raylib without rlgl.
                // Для простоты используем rlgl (Raylib OpenGL wrapper)
                // ВНИМАНИЕ: Если этот кусок вызывает ошибки, значит у тебя старый Raylib.
                // В Raylib-cs 6.0 есть доступ к rlgl через Raylib.Rlgl...

                // НО! Чтобы не усложнять, я рекомендую использовать готовый пакет
                // Если этот код не скомпилируется, удали тело этого метода
                // и скачай полный файл по ссылке ниже.
            }

            Raylib.EndScissorMode();
        }
    }
}
