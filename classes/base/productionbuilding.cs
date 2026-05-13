using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public abstract class ProductionBuilding : InteractableBuilding
{
    public ProductionState State { get; private set; } = ProductionState.WaitingForInput;
    private float _waitTimer = 0f;
    private int _selectedQty = 1;
    private int _pendingQty = 1;

    protected abstract float WaitTime { get; }
    protected abstract Item InputItem { get; }
    protected abstract Item OutputItem { get; }

    // Subclasses provide their own wording so the popup feels "animal" or "factory"
    protected abstract string IdleText { get; }       // e.g. "The animals are hungry."
    protected abstract string ProcessingVerb { get; } // e.g. "Feeding" or "Processing"
    protected abstract string ReadyText { get; }      // e.g. "Ready to collect!"
    protected abstract string ActionVerb { get; }     // e.g. "Feed" or "Process"

    public override bool RequiresPurchase => true;

    protected ProductionBuilding(Vector2 position) : base(position) { }

    public override void Update(float dt)
    {
        if (State == ProductionState.Processing)
        {
            _waitTimer += dt;
            if (_waitTimer >= WaitTime)
            {
                _waitTimer = 0f;
                State = ProductionState.Ready;
            }
        }
    }

    protected override void DrawPopupContent(int popupX, int popupY, int popupW, int popupH, Inventory inventory)
    {
        // Input counter (top-right)
        int inputCount = inventory.CountOf(InputItem);
        DrawTexturePro(
            InputItem.Icon,
            new Rectangle(0, 0, InputItem.Icon.Width, InputItem.Icon.Height),
            new Rectangle(popupX + popupW - 100, popupY + 60, 32, 32),
            new Vector2(0, 0), 0f, Color.White
        );
        DrawText($"x {inputCount}", popupX + popupW - 60, popupY + 66, 22, Color.White);

        // State text
        string stateText = State switch
        {
            ProductionState.WaitingForInput => IdleText,
            ProductionState.Processing => $"{ProcessingVerb} {_pendingQty}... {(WaitTime - _waitTimer):F0}s",
            ProductionState.Ready => $"{ReadyText} ({_pendingQty})",
            _ => ""
        };
        DrawText(stateText, popupX + 24, popupY + 130, 22, Color.LightGray);

        // Quantity selector — only shown when accepting input
        if (State == ProductionState.WaitingForInput)
        {
            int controlsY = popupY + popupH - 130;
            int centerX = popupX + popupW / 2;

            Rectangle minusBtn = new Rectangle(centerX - 80, controlsY, 32, 32);
            Rectangle plusBtn = new Rectangle(centerX + 48, controlsY, 32, 32);

            DrawRectangleRec(minusBtn, new Color(80, 80, 80, 255));
            DrawText("-", (int)minusBtn.X + 11, (int)minusBtn.Y + 4, 24, Color.White);

            string qtyText = $"{_selectedQty}";
            int qtyWidth = MeasureText(qtyText, 28);
            DrawText(qtyText, centerX - qtyWidth / 2, controlsY + 2, 28, Color.White);

            DrawRectangleRec(plusBtn, new Color(80, 80, 80, 255));
            DrawText("+", (int)plusBtn.X + 9, (int)plusBtn.Y + 4, 24, Color.White);

            if (CheckCollisionPointRec(GetMousePosition(), minusBtn) && IsMouseButtonPressed(MouseButton.Left))
                _selectedQty = System.Math.Max(1, _selectedQty - 1);
            if (CheckCollisionPointRec(GetMousePosition(), plusBtn) && IsMouseButtonPressed(MouseButton.Left))
                _selectedQty = System.Math.Min(3, _selectedQty + 1);
        }

        // Action button
        string buttonLabel = State == ProductionState.Ready
            ? $"Collect ({_pendingQty})"
            : $"{ActionVerb} ({_selectedQty})";

        bool buttonEnabled =
            (State == ProductionState.WaitingForInput && inputCount >= _selectedQty) ||
            (State == ProductionState.Ready);

        Rectangle buttonRect = new Rectangle(popupX + popupW / 2 - 80, popupY + popupH - 70, 160, 50);
        Color buttonColor = buttonEnabled ? new Color(80, 140, 60, 255) : new Color(70, 70, 70, 255);
        DrawRectangleRec(buttonRect, buttonColor);
        DrawRectangleLinesEx(buttonRect, 2, Color.White);

        int textWidth = MeasureText(buttonLabel, 24);
        DrawText(buttonLabel,
            (int)(buttonRect.X + (buttonRect.Width - textWidth) / 2),
            (int)(buttonRect.Y + 12),
            24, Color.White);

        if (buttonEnabled
            && CheckCollisionPointRec(GetMousePosition(), buttonRect)
            && IsMouseButtonPressed(MouseButton.Left))
        {
            if (State == ProductionState.WaitingForInput)
            {
                for (int i = 0; i < _selectedQty; i++)
                {
                    int slot = inventory.FindSlotWith(InputItem);
                    if (slot >= 0) inventory.RemoveOne(slot);
                }
                _pendingQty = _selectedQty;
                State = ProductionState.Processing;
                _waitTimer = 0f;
            }
            else if (State == ProductionState.Ready)
            {
                inventory.Add(OutputItem, _pendingQty);
                PlayerStats.AddXP(OutputItem.XpReward * _pendingQty);

                // Play animal-specific sound if this is an animal farm
                if (this is AnimalFarm af)
                    SoundManager.Play(af.CollectSound);

                State = ProductionState.WaitingForInput;
                _pendingQty = 1;
            }
        }
    }
}