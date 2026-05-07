using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public abstract class Factory
{
    protected const int FrameWidth = 76;
    protected const int FrameHeight = 125;
    protected const float Scale = 2.5f;
    protected const float DrawWidth = FrameWidth * Scale;   // 228
    protected const float DrawHeight = FrameHeight * Scale; // 375

    public Vector2 Position { get; }
    public FactoryState State { get; private set; } = FactoryState.Idle;
    public bool IsPopupOpen { get; private set; }

    private float _waitTimer = 0f;

    protected abstract Texture2D Sprite { get; }
    protected abstract float WaitTime { get; }
    protected abstract Item InputItem { get; }
    protected abstract Item OutputItem { get; }
    protected abstract string Title { get; }

    public Rectangle Hitbox => new Rectangle(Position.X, Position.Y, DrawWidth, DrawHeight);

    public Rectangle InteractZone => new Rectangle(
        Position.X - 40,
        Position.Y - 40,
        DrawWidth + 80,
        DrawHeight + 80
    );

    protected Factory(Vector2 position)
    {
        Position = position;
    }

    public void Update(float dt)
    {
        if (State == FactoryState.Processing)
        {
            _waitTimer += dt;
            if (_waitTimer >= WaitTime)
            {
                _waitTimer = 0f;
                State = FactoryState.Done;
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
        Rectangle src = new Rectangle(0, 0, FrameWidth, FrameHeight);
        Rectangle dst = new Rectangle(Position.X, Position.Y, DrawWidth, DrawHeight);
        DrawTexturePro(Sprite, src, dst, new Vector2(0, 0), 0f, Color.White);

        DrawRectangleLinesEx(Hitbox, 2, Color.Red);  // debug
    }

    public void DrawPopup(int screenW, int screenH, Inventory inventory)
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

        // Close button
        Rectangle closeBtn = new Rectangle(popupX + popupW - 44, popupY + 12, 32, 32);
        DrawRectangleRec(closeBtn, new Color(150, 50, 50, 255));
        DrawText("X", (int)closeBtn.X + 10, (int)closeBtn.Y + 6, 24, Color.White);
        if (CheckCollisionPointRec(GetMousePosition(), closeBtn) && IsMouseButtonPressed(MouseButton.Left))
        {
            ClosePopup();
            return;
        }

        // Input item counter
        int inputCount = inventory.CountOf(InputItem);
        DrawTexturePro(
            InputItem.Icon,
            new Rectangle(0, 0, InputItem.Icon.Width, InputItem.Icon.Height),
            new Rectangle(popupX + popupW - 100, popupY + 60, 32, 32),
            new Vector2(0, 0),
            0f,
            Color.White
        );
        DrawText($"x {inputCount}", popupX + popupW - 60, popupY + 66, 22, Color.White);

        // State text
        string stateText = State switch
        {
            FactoryState.Idle => $"Insert {InputItem.Name} to start.",
            FactoryState.Processing => $"Processing... {(WaitTime - _waitTimer):F0}s",
            FactoryState.Done => $"{OutputItem.Name} ready!",
            _ => ""
        };
        DrawText(stateText, popupX + 24, popupY + 130, 22, Color.LightGray);

        // Action button
        string buttonLabel = State == FactoryState.Done ? "Collect" : "Process";
        bool buttonEnabled =
            (State == FactoryState.Idle && inputCount > 0) ||
            (State == FactoryState.Done);

        Rectangle buttonRect = new Rectangle(popupX + popupW / 2 - 80, popupY + popupH - 70, 160, 50);
        Color buttonColor = buttonEnabled ? new Color(80, 140, 60, 255) : new Color(70, 70, 70, 255);
        DrawRectangleRec(buttonRect, buttonColor);
        DrawRectangleLinesEx(buttonRect, 2, Color.White);

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
            if (State == FactoryState.Idle)
            {
                int inputSlot = inventory.FindSlotWith(InputItem);
                if (inputSlot >= 0)
                {
                    inventory.RemoveOne(inputSlot);
                    State = FactoryState.Processing;
                    _waitTimer = 0f;
                }
            }
            else if (State == FactoryState.Done)
            {
                inventory.Add(OutputItem, 1);
                State = FactoryState.Idle;
            }
        }
    }
}

