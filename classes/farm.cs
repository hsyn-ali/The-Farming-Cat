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

    public Farm(int startX, int startY)
    {
        StartX = startX;
        StartY = startY;
    }

    // Returns (col, row) of the tile at this world point, or null if outside the farm
    public (int col, int row)? GetTileAt(Vector2 worldPoint)
    {
        int col = (int)((worldPoint.X - StartX) / TileSize);
        int row = (int)((worldPoint.Y - StartY) / TileSize);

        if (col < 0 || col >= Cols || row < 0 || row >= Rows)
            return null;

        return (col, row);
    }

    // World-space rectangle for a given tile
    public Rectangle GetTileRect(int col, int row)
    {
        return new Rectangle(
            StartX + col * TileSize,
            StartY + row * TileSize,
            TileSize,
            TileSize
        );
    }

    public void Draw(Vector2 playerFeet)
    {
        // Debug outline of the plantable area
        DrawRectangleLinesEx(Area, 2, Color.Yellow);

        // Highlight under the player's feet, if on a tile
        var tile = GetTileAt(playerFeet);
        if (tile.HasValue)
        {
            Rectangle highlight = GetTileRect(tile.Value.col, tile.Value.row);
            DrawRectangleLinesEx(highlight, 2, Color.White);
        }
    }
}