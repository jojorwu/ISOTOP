using Raylib_cs;
using Arch.Core;

public class GameLoop
{
    // 60 тиков в секунду для физики/логики (как в файтингах или CS2)
    const double TickRate = 1.0 / 60.0;
    private World _world;

    public void Run()
    {
        Raylib.InitWindow(1280, 720, "ISOTOPE ENGINE [DEBUG]");
        Raylib.SetTargetFPS(144); // Рендер максимально плавный, отдельно от физики

        _world = World.Create();
        // Тут инициализируем системы: MovementSystem, InputSystem, RenderSystem

        double accumulator = 0.0;
        double lastTime = Raylib.GetTime();

        while (!Raylib.WindowShouldClose())
        {
            double currentTime = Raylib.GetTime();
            double frameTime = currentTime - lastTime;
            lastTime = currentTime;

            // Защита от "спирали смерти" (если лагает, не пытаемся догнать вечность)
            if (frameTime > 0.25) frameTime = 0.25;

            accumulator += frameTime;

            // --- ФИЗИКА И ЛОГИКА (FIXED UPDATE) ---
            while (accumulator >= TickRate)
            {
                // Тут считаем столкновения, атмосферу, хим. реакции
                // RunSystems(_world, TickRate);
                accumulator -= TickRate;
            }

            // --- РЕНДЕР (INTERPOLATION) ---
            // alpha нужен, чтобы плавно рисовать движение между тиками
            double alpha = accumulator / TickRate;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // RenderSystems(_world, alpha);
            Raylib.DrawFPS(10, 10);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
