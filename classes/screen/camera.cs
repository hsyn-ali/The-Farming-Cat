using Raylib_cs;
using System.Numerics;

namespace Game;

public class Camera
{
    public Camera2D Raw;

    private readonly Vector2 _screenSize;

    public Camera(Vector2 target, Vector2 screenSize)
    {
        _screenSize = screenSize;
        Raw = new Camera2D
        {
            Target = target,
            Offset = screenSize / 2f,
            Rotation = 0f,
            Zoom = 1f
        };
    }

    public void Follow(Vector2 target, int mapWidth, int mapHeight)
    {
        float halfW = _screenSize.X / 2f;
        float halfH = _screenSize.Y / 2f;

        // Clamp target so camera view stays inside the map
        float clampedX = Math.Clamp(target.X, halfW, mapWidth - halfW);
        float clampedY = Math.Clamp(target.Y, halfH, mapHeight - halfH);

        Raw.Target = new Vector2(clampedX, clampedY);
    }
}