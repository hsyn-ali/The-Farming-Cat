using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public abstract class InteractableBuilding
{
    public Vector2 Position { get; }
    public bool IsPopupOpen { get; private set; }

    protected abstract Texture2D Sprite { get; }
    protected abstract int FrameWidth { get; }
    protected abstract int FrameHeight { get; }
    protected abstract float Scale { get; }
    protected abstract string Title { get; }

    // Purchase gating — defaults mean "always available" for things like Shop/NoticeBoard
    public virtual int RequiredLevel => 1;
    public virtual int PurchaseCost => 0;
    public virtual bool RequiresPurchase => false;
    protected virtual int PopupWidth => 500;
    protected virtual int PopupHeight => 300;
    public bool IsBought { get; protected set; } = false;

    public bool IsAvailable => PlayerStats.Level >= RequiredLevel && (!RequiresPurchase || IsBought);

    public float DrawWidth => FrameWidth * Scale;
    public float DrawHeight => FrameHeight * Scale;

    public Rectangle Hitbox => new Rectangle(Position.X, Position.Y, DrawWidth, DrawHeight);

    public Rectangle InteractZone => new Rectangle(
        Position.X - 40,
        Position.Y - 40,
        DrawWidth + 80,
        DrawHeight + 80
    );

    protected InteractableBuilding(Vector2 position)
    {
        Position = position;
    }

    public abstract void Update(float dt);

    public bool CanPlayerInteract(Rectangle playerHitbox)
    {
        return CheckCollisionRecs(playerHitbox, InteractZone);
    }

    public void OpenPopup() => IsPopupOpen = true;
    public void ClosePopup() => IsPopupOpen = false;

    public void Draw()
    {
        // Grey-tint the sprite if locked
        Color tint = IsAvailable ? Color.White : new Color(120, 120, 120, 200);

        Rectangle src = new Rectangle(0, 0, FrameWidth, FrameHeight);
        Rectangle dst = new Rectangle(Position.X, Position.Y, DrawWidth, DrawHeight);
        DrawTexturePro(Sprite, src, dst, new Vector2(0, 0), 0f, tint);

        //DrawRectangleLinesEx(Hitbox, 2, Color.Red);  // debug
    }

    public void DrawInteractionPrompt()
    {
        string text = "Press E";

        int fontSize = 24;
        int textWidth = MeasureText(text, fontSize);

        float textX =
            Position.X +
            DrawWidth / 2f -
            textWidth / 2f;

        float textY = Position.Y - 35;

        DrawRectangle(
            (int)textX - 10,
            (int)textY - 5,
            textWidth + 20,
            34,
            new Color(0, 0, 0, 180)
        );

        DrawText(text, (int)textX, (int)textY, fontSize, Color.White);
    }

    public void DrawPopup(int screenW, int screenH, Inventory inventory)
    {
        if (!IsPopupOpen) return;

        // Dim background
        DrawRectangle(0, 0, screenW, screenH, new Color(0, 0, 0, 120));

        // Popup window
        int popupW = PopupWidth;
        int popupH = PopupHeight;
        int popupX = (screenW - popupW) / 2;
        int popupY = (screenH - popupH) / 2;
        Rectangle popupRect = new Rectangle(popupX, popupY, popupW, popupH);

        DrawRectangleRec(popupRect, new Color(50, 40, 30, 240));
        DrawRectangleLinesEx(popupRect, 4, new Color(120, 90, 60, 255));

        // Title
        DrawText(Title, popupX + 24, popupY + 24, 28, Color.White);

        // Close button
        Rectangle closeBtn = new Rectangle(popupX + popupW - 44, popupY + 12, 32, 32);
        DrawRectangleRec(closeBtn, new Color(150, 50, 50, 255));
        DrawText("X", (int)closeBtn.X + 10, (int)closeBtn.Y + 6, 24, Color.White);
        if (CheckCollisionPointRec(GetMousePosition(), closeBtn) && IsMouseButtonPressed(MouseButton.Left))
        {
            SoundManager.Play(RunTime.ClickSound);
            ClosePopup();
            return;
        }

        // Gate check: locked or unpurchased buildings show purchase/locked UI instead
        if (!IsAvailable)
        {
            DrawLockedContent(popupX, popupY, popupW, popupH);
            return;
        }

        // Subclass-specific content
        DrawPopupContent(popupX, popupY, popupW, popupH, inventory);
    }

    protected abstract void DrawPopupContent(int popupX, int popupY, int popupW, int popupH, Inventory inventory);

    private void DrawLockedContent(int popupX, int popupY, int popupW, int popupH)
    {
        if (PlayerStats.Level < RequiredLevel)
        {
            // Level-locked: no buy option
            string msg = $"Requires Level {RequiredLevel}";
            int msgWidth = MeasureText(msg, 28);
            DrawText(msg, popupX + (popupW - msgWidth) / 2, popupY + popupH / 2 - 20, 28, new Color(220, 100, 100, 255));

            string sub = $"You are Level {PlayerStats.Level}";
            int subWidth = MeasureText(sub, 20);
            DrawText(sub, popupX + (popupW - subWidth) / 2, popupY + popupH / 2 + 20, 20, Color.LightGray);
            return;
        }

        // Level reached but not bought — show purchase UI
        string costText = $"Cost: {PurchaseCost} coins";
        int costWidth = MeasureText(costText, 26);
        DrawText(costText, popupX + (popupW - costWidth) / 2, popupY + 110, 26, Color.White);

        string coinsText = $"You have: {PlayerStats.Coins} coins";
        int coinsWidth = MeasureText(coinsText, 20);
        DrawText(coinsText, popupX + (popupW - coinsWidth) / 2, popupY + 150, 20, Color.LightGray);

        // Buy button
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
