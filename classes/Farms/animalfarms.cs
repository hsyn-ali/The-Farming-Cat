using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public enum AnimalFarmState { Hungry, Waiting, Ready }

public abstract class AnimalFarm
{
    protected const int FrameSize = 48;
    protected const float Scale = 4f;
    protected const float DrawSize = FrameSize * Scale;  // 192

    public Vector2 Position { get; }
    public AnimalFarmState State { get; private set; } = AnimalFarmState.Hungry;
    public bool IsPopupOpen { get; private set; }

    private float _waitTimer = 0f;

    protected abstract Texture2D Sprite { get; }
    protected abstract float WaitTime { get; }
    protected abstract Item ProducedItem { get; }
    protected abstract string Title { get; }

    public Rectangle Hitbox => new Rectangle(Position.X, Position.Y, DrawSize, DrawSize);

    // Area around the hitbox where the player can press E to open the popup
    public Rectangle InteractZone => new Rectangle(
        Position.X - 40,
        Position.Y - 40,
        DrawSize + 80,
        DrawSize + 80
    );

    protected AnimalFarm(Vector2 position)
    {
        Position = position;
    }

    public void Update(float dt)
    {
        if (State == AnimalFarmState.Waiting)
        {
            _waitTimer += dt;
            if (_waitTimer >= WaitTime)
            {
                _waitTimer = 0f;
                State = AnimalFarmState.Ready;
            }
        }
    }

    public bool CanPlayerInteract(Rectangle playerHitbox)
    {
        return CheckCollisionRecs(playerHitbox, InteractZone);
    }

    public void OpenPopup() => IsPopupOpen = true;
    public void ClosePopup() => IsPopupOpen = false;

    public void Draw()
    {
        Rectangle src = new Rectangle(0, 0, FrameSize, FrameSize);
        Rectangle dst = new Rectangle(Position.X, Position.Y, DrawSize, DrawSize);
        DrawTexturePro(Sprite, src, dst, new Vector2(0, 0), 0f, Color.White);

        DrawRectangleLinesEx(Hitbox, 2, Color.Red);  // debug
    }

    // Draws the popup in screen space and handles button clicks.
    // Called outside BeginMode2D so it stays fixed on screen.
    // Returns true if the popup consumed a button click this frame.
    public void DrawPopup(int screenW, int screenH, Inventory inventory, Item feedItem)
    {
        if (!IsPopupOpen) return;

        // Dim background
        DrawRectangle(0, 0, screenW, screenH, new Color(0, 0, 0, 120));

        // Popup window
        int popupW = 500;
        int popupH = 300;
        int popupX = (screenW - popupW) / 2;
        int popupY = (screenH - popupH) / 2;
        Rectangle popupRect = new Rectangle(popupX, popupY, popupW, popupH);

        DrawRectangleRec(popupRect, new Color(50, 40, 30, 240));
        DrawRectangleLinesEx(popupRect, 4, new Color(120, 90, 60, 255));

        // Title
        DrawText(Title, popupX + 24, popupY + 24, 28, Color.White);

        // Close button (top-right X)
        Rectangle closeBtn = new Rectangle(popupX + popupW - 44, popupY + 12, 32, 32);
        DrawRectangleRec(closeBtn, new Color(150, 50, 50, 255));
        DrawText("X", (int)closeBtn.X + 10, (int)closeBtn.Y + 6, 24, Color.White);
        if (CheckCollisionPointRec(GetMousePosition(), closeBtn) && IsMouseButtonPressed(MouseButton.Left))
        {
            ClosePopup();
            return;
        }

        // Wheat counter (top-right under close button)
        int wheatCount = inventory.CountOf(feedItem);
        DrawTexturePro(
            feedItem.Icon,
            new Rectangle(0, 0, feedItem.Icon.Width, feedItem.Icon.Height),
            new Rectangle(popupX + popupW - 100, popupY + 60, 32, 32),
            new Vector2(0, 0),
            0f,
            Color.White
        );
        DrawText($"x {wheatCount}", popupX + popupW - 60, popupY + 66, 22, Color.White);

        // State text in middle
        string stateText = State switch
        {
            AnimalFarmState.Hungry => "The animals are hungry.",
            AnimalFarmState.Waiting => $"Feeding... {(WaitTime - _waitTimer):F0}s",
            AnimalFarmState.Ready => "Ready to collect!",
            _ => ""
        };
        DrawText(stateText, popupX + 24, popupY + 130, 22, Color.LightGray);

        // Action button
        string buttonLabel = State == AnimalFarmState.Ready ? "Collect" : "Feed";
        bool buttonEnabled =
            (State == AnimalFarmState.Hungry && wheatCount > 0) ||
            (State == AnimalFarmState.Ready);

        Rectangle buttonRect = new Rectangle(popupX + popupW / 2 - 80, popupY + popupH - 70, 160, 50);
        Color buttonColor = buttonEnabled ? new Color(80, 140, 60, 255) : new Color(70, 70, 70, 255);
        DrawRectangleRec(buttonRect, buttonColor);
        DrawRectangleLinesEx(buttonRect, 2, Color.White);

        // Center the button text
        int textWidth = MeasureText(buttonLabel, 24);
        DrawText(
            buttonLabel,
            (int)(buttonRect.X + (buttonRect.Width - textWidth) / 2),
            (int)(buttonRect.Y + 12),
            24,
            Color.White
        );

        // Click handling
        if (buttonEnabled
            && CheckCollisionPointRec(GetMousePosition(), buttonRect)
            && IsMouseButtonPressed(MouseButton.Left))
        {
            if (State == AnimalFarmState.Hungry)
            {
                // Feed: consume 1 wheat, start waiting
                int feedSlot = inventory.FindSlotWith(feedItem);
                if (feedSlot >= 0)
                {
                    inventory.RemoveOne(feedSlot);
                    State = AnimalFarmState.Waiting;
                    _waitTimer = 0f;
                }
            }
            else if (State == AnimalFarmState.Ready)
            {
                // Collect: add 1 produced item, back to hungry
                inventory.Add(ProducedItem, 1);
                State = AnimalFarmState.Hungry;
            }
        }
    }
}

public class ChickenHouse : AnimalFarm
{
    public ChickenHouse(Vector2 position) : base(position) { }

    protected override Texture2D Sprite => RunTime.ChickenHouse;
    protected override float WaitTime => 30f;
    protected override Item ProducedItem => RunTime.EggItem;
    protected override string Title => "Chicken House";
}