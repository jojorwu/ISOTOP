using Raylib_cs;
using System.Numerics;

public class CameraService
{
    public Camera2D Camera;

    public CameraService(int screenWidth, int screenHeight)
    {
        Camera = new Camera2D
        {
            Target = new Vector2(screenWidth / 2f, screenHeight / 2f),
            Offset = new Vector2(screenWidth / 2f, screenHeight / 2f),
            Rotation = 0.0f,
            Zoom = 1.0f
        };
    }

    public void Update()
    {
        // Panning
        float panSpeed = 10.0f;
        if (Raylib.IsKeyDown(KeyboardKey.Up)) Camera.Target.Y -= panSpeed;
        if (Raylib.IsKeyDown(KeyboardKey.Down)) Camera.Target.Y += panSpeed;
        if (Raylib.IsKeyDown(KeyboardKey.Left)) Camera.Target.X -= panSpeed;
        if (Raylib.IsKeyDown(KeyboardKey.Right)) Camera.Target.X += panSpeed;

        // Zooming
        float zoomSpeed = 0.1f;
        float wheelMove = Raylib.GetMouseWheelMove();
        Camera.Zoom += wheelMove * zoomSpeed;
        if (Camera.Zoom < 0.1f) Camera.Zoom = 0.1f;
    }
}
