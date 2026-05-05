using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public abstract class Crop
{
    protected const int FrameWidth = 16;
    protected const int FrameHeight = 16;
    protected const int TotalStages = 3;

    private float _timer = 0f;
    private int _stage = 0;

    protected abstract Texture2D Sprite { get; }
    protected abstract float TimePerStage { get; }
    public abstract Item HarvestedItem { get; }

    public bool IsFullyGrown => _stage >= TotalStages - 1;

    public void Update(float dt)
    {
        if (IsFullyGrown) return;

        _timer += dt;
        if (_timer >= TimePerStage)
        {
            _timer = 0f;
            _stage++;
            if (_stage >= TotalStages) _stage = TotalStages - 1;
        }
    }

    public void Draw(Rectangle tileRect)
    {
        Rectangle src = new Rectangle(_stage * FrameWidth, 0, FrameWidth, FrameHeight);
        DrawTexturePro(Sprite, src, tileRect, new Vector2(0, 0), 0f, Color.White);
    }
}