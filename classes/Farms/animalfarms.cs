using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public abstract class AnimalFarm : InteractableBuilding
{
    protected override int FrameWidth => 48;
    protected override int FrameHeight => 48;
    protected override float Scale => 4f;

    public AnimalFarmState State { get; private set; } = AnimalFarmState.Hungry;
    private float _waitTimer = 0f;

    protected abstract float WaitTime { get; }
    protected abstract Item ProducedItem { get; }
    protected abstract Item FeedItem { get; }
    public override bool RequiresPurchase => true;

    protected AnimalFarm(Vector2 position) : base(position) { }

    public override void Update(float dt)
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

    protected override void DrawPopupContent(int popupX, int popupY, int popupW, int popupH, Inventory inventory)
    {
        // Feed counter
        int feedCount = inventory.CountOf(FeedItem);
        DrawTexturePro(
            FeedItem.Icon,
            new Rectangle(0, 0, FeedItem.Icon.Width, FeedItem.Icon.Height),
            new Rectangle(popupX + popupW - 100, popupY + 60, 32, 32),
            new Vector2(0, 0),
            0f,
            Color.White
        );
        DrawText($"x {feedCount}", popupX + popupW - 60, popupY + 66, 22, Color.White);

        // State text
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
            (State == AnimalFarmState.Hungry && feedCount > 0) ||
            (State == AnimalFarmState.Ready);

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
            if (State == AnimalFarmState.Hungry)
            {
                int feedSlot = inventory.FindSlotWith(FeedItem);
                if (feedSlot >= 0)
                {
                    inventory.RemoveOne(feedSlot);
                    State = AnimalFarmState.Waiting;
                    _waitTimer = 0f;
                }
            }
            else if (State == AnimalFarmState.Ready)
            {
                inventory.Add(ProducedItem, 1);
                PlayerStats.AddXP(ProducedItem.XpReward);
                State = AnimalFarmState.Hungry;

            }
        }
    }
}