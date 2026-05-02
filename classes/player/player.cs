using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class Player
{
    private const int FrameWidth = 48;
    private const int FrameHeight = 48;
    private const int TotalFrames = 4;
    private const float Scale = 3f;
    private const float Speed = 120f;
    private const float AnimSpeed = 0.15f;

    public Vector2 Position;

    private int _currentFrame = 0;
    private float _animTimer = 0f;
    private Direction _direction = Direction.Down;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
    }

    public Rectangle Hitbox => new Rectangle(
        Position.X + FrameWidth * Scale * 0.33f,
        Position.Y + FrameHeight * Scale * 0.3f,
        FrameWidth * Scale * 0.33f,
        FrameHeight * Scale * 0.4f
    );

    public void Update(float dt)
    {
        Vector2 input = new Vector2(0f, 0f);

        if (IsKeyDown(KeyboardKey.W)) { input.Y -= 1; _direction = Direction.Up; }
        if (IsKeyDown(KeyboardKey.S)) { input.Y += 1; _direction = Direction.Down; }
        if (IsKeyDown(KeyboardKey.A)) { input.X -= 1; _direction = Direction.Left; }
        if (IsKeyDown(KeyboardKey.D)) { input.X += 1; _direction = Direction.Right; }

        if (input.X != 0f || input.Y != 0f)
        {
            Vector2 velocity = Vector2.Normalize(input) * Speed;
            Position += velocity * dt;

            _animTimer += dt;
            if (_animTimer >= AnimSpeed)
            {
                _animTimer = 0f;
                _currentFrame = (_currentFrame + 1) % TotalFrames;
            }
        }
        else
        {
            _currentFrame = 0;
            _animTimer = 0f;
        }
    }

    public void Draw()
    {
        Rectangle src = new Rectangle(_currentFrame * FrameWidth, (int)_direction * FrameHeight, FrameWidth, FrameHeight);
        Rectangle dst = new Rectangle(Position.X, Position.Y, FrameWidth * Scale, FrameHeight * Scale);
        DrawTexturePro(RunTime.PlayerRun, src, dst, new Vector2(0f, 0f), 0f, Color.White);

        DrawRectangleLinesEx(Hitbox, 2, Color.Red);
    }
}