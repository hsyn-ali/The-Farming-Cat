using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public abstract class Factory : InteractableBuilding
{
    protected override int FrameWidth => 76;
    protected override int FrameHeight => 125;
    protected override float Scale => 2.5f;

    public FactoryState State { get; private set; } = FactoryState.Idle;
    private float _waitTimer = 0f;

    protected abstract float WaitTime { get; }
    protected abstract Item InputItem { get; }
    protected abstract Item OutputItem { get; }
    public override bool RequiresPurchase => true;

    protected Factory(Vector2 position) : base(position) { }

    public override void Update(float dt)
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

    protected override void DrawPopupContent(int popupX, int popupY, int popupW, int popupH, Inventory inventory)
    {
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