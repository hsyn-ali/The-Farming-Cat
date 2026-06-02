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

    public int RequiredLevel { get; }
    public int PurchaseCost { get; }
    public bool RequiresPurchase => PurchaseCost > 0;
    public bool IsBought { get; private set; }

    public bool IsAvailable => PlayerStats.Level >= RequiredLevel && (!RequiresPurchase || IsBought);

    public bool IsPopupOpen { get; private set; }
    public void OpenPopup() => IsPopupOpen = true;
    public void ClosePopup() => IsPopupOpen = false;

    private const int PopupWidth = 500;
    private const int PopupHeight = 300;
    private const string Title = "Farm Plot";

    public Rectangle Area => new Rectangle(StartX, StartY, Cols * TileSize, Rows * TileSize);

    private readonly Crop?[,] _crops = new Crop?[Cols, Rows];

    public Farm(int startX, int startY, int requiredLevel = 1, int purchaseCost = 0)
    {
        StartX = startX;
        StartY = startY;
        RequiredLevel = requiredLevel;
        PurchaseCost = purchaseCost;
        IsBought = (purchaseCost == 0);  // free farms are unlocked by default
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

    public bool ContainsPoint(Vector2 point)
    {
        return CheckCollisionPointRec(point, Area);
    }

    public bool TryPlant(int col, int row, Item item)
    {
        if (!IsAvailable) return false;
        if (!item.IsPlantable) return false;
        if (_crops[col, row] != null) return false;

        _crops[col, row] = item.CreateCrop!();
        return true;
    }

    public Item? TryHarvest(int col, int row)
    {
        if (!IsAvailable) return null;

        Crop? crop = _crops[col, row];
        if (crop == null || !crop.IsFullyGrown) return null;

        Item harvested = crop.HarvestedItem;
        PlayerStats.AddXP(harvested.XpReward);

        _crops[col, row] = null;
        return harvested;
    }

    public void Update(float dt)
    {
        if (!IsAvailable) return;

        for (int c = 0; c < Cols; c++)
            for (int r = 0; r < Rows; r++)
                _crops[c, r]?.Update(dt);
    }

    public void Draw(Vector2 playerFeet)
    {
        // Locked: grey overlay 
        if (!IsAvailable)
        {
            DrawRectangleRec(Area, new Color(0, 0, 0, 100));
            DrawRectangleLinesEx(Area, 2, new Color(120, 120, 120, 255));
            return;
        }

        //DrawRectangleLinesEx(Area, 2, Color.Yellow); //debug

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

    // Same pattern as InteractableBuilding.DrawPopup
    public void DrawPopup(int screenW, int screenH)
    {
        if (!IsPopupOpen) return;

        // Dim background
        DrawRectangle(0, 0, screenW, screenH, new Color(0, 0, 0, 120));

        // Popup window
        int popupX = (screenW - PopupWidth) / 2;
        int popupY = (screenH - PopupHeight) / 2;
        Rectangle popupRect = new Rectangle(popupX, popupY, PopupWidth, PopupHeight);

        DrawRectangleRec(popupRect, new Color(50, 40, 30, 240));
        DrawRectangleLinesEx(popupRect, 4, new Color(120, 90, 60, 255));

        // Title
        DrawText(Title, popupX + 24, popupY + 24, 28, Color.White);

        // Close button
        Rectangle closeBtn = new Rectangle(popupX + PopupWidth - 44, popupY + 12, 32, 32);
        DrawRectangleRec(closeBtn, new Color(150, 50, 50, 255));
        DrawText("X", (int)closeBtn.X + 10, (int)closeBtn.Y + 6, 24, Color.White);
        if (CheckCollisionPointRec(GetMousePosition(), closeBtn) && IsMouseButtonPressed(MouseButton.Left))
        {
            SoundManager.Play(RunTime.ClickSound);
            ClosePopup();
            return;
        }

        // Gate check 
        DrawLockedContent(popupX, popupY, PopupWidth, PopupHeight);
    }

    // Same logic as InteractableBuilding.DrawLockedContent
    private void DrawLockedContent(int popupX, int popupY, int popupW, int popupH)
    {
        if (PlayerStats.Level < RequiredLevel)
        {
            string msg = $"Requires Level {RequiredLevel}";
            int msgWidth = MeasureText(msg, 28);
            DrawText(msg, popupX + (popupW - msgWidth) / 2, popupY + popupH / 2 - 20, 28, new Color(220, 100, 100, 255));

            string sub = $"You are Level {PlayerStats.Level}";
            int subWidth = MeasureText(sub, 20);
            DrawText(sub, popupX + (popupW - subWidth) / 2, popupY + popupH / 2 + 20, 20, Color.LightGray);
            return;
        }

        string costText = $"Cost: {PurchaseCost} coins";
        int costWidth = MeasureText(costText, 26);
        DrawText(costText, popupX + (popupW - costWidth) / 2, popupY + 110, 26, Color.White);

        string coinsText = $"You have: {PlayerStats.Coins} coins";
        int coinsWidth = MeasureText(coinsText, 20);
        DrawText(coinsText, popupX + (popupW - coinsWidth) / 2, popupY + 150, 20, Color.LightGray);

        bool canAfford = PlayerStats.Coins >= PurchaseCost;
        Rectangle buyBtn = new Rectangle(popupX + popupW / 2 - 80, popupY + popupH - 70, 160, 50);
        Color buyColor = canAfford ? new Color(80, 140, 60, 255) : new Color(70, 70, 70, 255);
        DrawRectangleRec(buyBtn, buyColor);
        DrawRectangleLinesEx(buyBtn, 2, Color.White);

        string buyLabel = "Buy";
        int buyTextWidth = MeasureText(buyLabel, 24);
        DrawText(buyLabel,
            (int)(buyBtn.X + (buyBtn.Width - buyTextWidth) / 2),
            (int)(buyBtn.Y + 12),
            24, Color.White);

        if (canAfford
            && CheckCollisionPointRec(GetMousePosition(), buyBtn)
            && IsMouseButtonPressed(MouseButton.Left))
        {
            if (PlayerStats.SpendCoins(PurchaseCost))
            {
                IsBought = true;
                SoundManager.Play(RunTime.BuySellSound);
                ClosePopup();
            }
        }
    }
}