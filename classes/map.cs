using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class Map
{
    public int Width { get; }
    public int Height { get; }

    private Texture2D _texture;

    public Map(string texturePath)
    {
        _texture = LoadTexture(texturePath);
        Width = _texture.Width;
        Height = _texture.Height;
    }

    public void Draw()
    {
        DrawTexture(_texture, 0, 0, Color.White);
    }

    public void Unload()
    {
        UnloadTexture(_texture);
    }
}