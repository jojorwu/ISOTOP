using Isotope.Client.Editor;
using Raylib_cs;
using Isotope.Client;

namespace Isotope.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Raylib.InitWindow(1600, 900, "Isotope Engine");
            Raylib.SetWindowState(ConfigFlags.ResizableWindow);
            RlImGui.Setup(true);

            var game = new GameLoop();
            game.Init();

            var editor = new EditorLayer();
            editor.Init();

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DarkGray);

                editor.Draw(
                    game.World,
                    gameUpdate: () => game.Update(),
                    gameRender: () => game.Render()
                );

                Raylib.EndDrawing();
            }

            game.Shutdown();
            RlImGui.Shutdown();
            Raylib.CloseWindow();
        }
    }
}
