using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class Farm
{
    public const int TileSize = 80;
    public const int Cols = 4;
    public const int Rows = 5;

    public int StartX { get; }
    public int StartY { get; }

    public Rectangle Area => new Rectangle(StartX, StartY, Cols * TileSize, Rows * TileSize);

    private readonly Crop?[,] _crops = new Crop?[Cols, Rows];

    public Farm(int startX, int startY)
    {
        StartX = startX;
        StartY = startY;
    }

    public (int col, int row)? GetTileAt(Vector2 worldPoint)
    {
        int col = (int)((worldPoint.X - StartX) / TileSize);
        int row = (int)((worldPoint.Y - StartY) / TileSize);
        if (col < 0 || col >= Cols || row < 0 || row >= Rows) return null;
        return (col, row);
    }

    public Rectangle GetTileRect(int col, int row)
    {
        return new Rectangle(StartX + col * TileSize, StartY + row * TileSize, TileSize, TileSize);
    }

    public bool TryPlant(int col, int row, Item item)
    {
        if (!item.IsPlantable) return false;
        if (_crops[col, row] != null) return false;

        _crops[col, row] = item.CreateCrop!();
        return true;
    }

    // Returns the harvested item, or null if no harvest happened.
    public Item? TryHarvest(int col, int row)
    {
        Crop? crop = _crops[col, row];

        if (crop == null || !crop.IsFullyGrown)
            return null;

        Item harvested = crop.HarvestedItem;

        // Give XP based on harvested crop
        PlayerStats.AddXP(harvested.XpReward);

        _crops[col, row] = null;

        return harvested;
    }

    public void Update(float dt)
    {
        for (int c = 0; c < Cols; c++)
            for (int r = 0; r < Rows; r++)
                _crops[c, r]?.Update(dt);
    }

    public void Draw(Vector2 playerFeet)
    {
        DrawRectangleLinesEx(Area, 2, Color.Yellow);

        for (int c = 0; c < Cols; c++)
        {
            for (int r = 0; r < Rows; r++)
            {
                Crop? crop = _crops[c, r];
                if (crop != null)
                    crop.Draw(GetTileRect(c, r));
            }
        }

        var tileUnder = GetTileAt(playerFeet);
        if (tileUnder.HasValue)
        {
            Rectangle highlight = GetTileRect(tileUnder.Value.col, tileUnder.Value.row);
            DrawRectangleLinesEx(highlight, 2, Color.White);
        }
    }
}