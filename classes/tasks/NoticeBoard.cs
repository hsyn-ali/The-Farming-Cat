using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class NoticeBoard : InteractableBuilding
{
    private const float OrderRefreshTime = 5f;

    // Current order: parallel lists. Empty when no order is active.
    private readonly List<Item> _orderItems = new();
    private readonly List<int> _orderQuantities = new();
    private int _coinReward = 0;
    private int _xpReward = 0;

    private float _timeUntilNewOrder = 0f;
    private readonly Random _random = new();

    protected override Texture2D Sprite => RunTime.NoticeBoardSprite;
    protected override int FrameWidth => 32;
    protected override int FrameHeight => 16;
    protected override float Scale => 4f;
    protected override string Title => "Notice Board";

    protected override int PopupWidth => 600;
    protected override int PopupHeight => 480;

    public override int RequiredLevel => 1;
    public override bool RequiresPurchase => false;

    public NoticeBoard(Vector2 position) : base(position)
    {
        GenerateNewOrder();
    }

    public override void Update(float dt)
    {
        if (_orderItems.Count == 0)
        {
            _timeUntilNewOrder -= dt;
            if (_timeUntilNewOrder <= 0f)
                GenerateNewOrder();
        }
    }

    private void GenerateNewOrder()
    {
        // Clear previous order
        _orderItems.Clear();
        _orderQuantities.Clear();
        _coinReward = 0;
        _xpReward = 0;

        // Build the list of items the player has unlocked
        List<Item> available = new();

        if (PlayerStats.Level >= 1)
            available.Add(RunTime.WheatHarvestedItem);
        if (PlayerStats.Level >= UnlockManager.CarrotSeedLevel)
            available.Add(RunTime.CarrotHarvestedItem);
        if (PlayerStats.Level >= UnlockManager.GoldenCarrotSeedLevel)
            available.Add(RunTime.GoldenCarrotHarvestedItem);
        if (PlayerStats.Level >= UnlockManager.BeetrootSeedLevel)
            available.Add(RunTime.BeetrootHarvestedItem);
        if (PlayerStats.Level >= UnlockManager.ChickenLevel)
            available.Add(RunTime.EggItem);
        if (PlayerStats.Level >= UnlockManager.CowLevel)
            available.Add(RunTime.MilkItem);
        if (PlayerStats.Level >= UnlockManager.SheepLevel)
            available.Add(RunTime.WoolItem);

        if (available.Count == 0)
        {
            _timeUntilNewOrder = OrderRefreshTime;
            return;
        }

        // Pick how many items in this order
        int itemCount = System.Math.Min(_random.Next(1, 4), available.Count);

        // Shuffle so picks are unique
        for (int i = available.Count - 1; i > 0; i--)
        {
            int swap = _random.Next(i + 1);
            (available[i], available[swap]) = (available[swap], available[i]);
        }

        // Take first N items
        for (int i = 0; i < itemCount; i++)
        {
            Item chosen = available[i];
            int qty = _random.Next(1, 11);
            _orderItems.Add(chosen);
            _orderQuantities.Add(qty);
            _coinReward += chosen.SellPrice * qty;
            _xpReward += chosen.XpReward * qty;
        }

        // Bonus scales with number of items
        _coinReward += 20 + (itemCount - 1) * 15;
        _xpReward += 25 + (itemCount - 1) * 15;
    }

    protected override void DrawPopupContent(int popupX, int popupY, int popupW, int popupH, Inventory inventory)
    {
        // No order yet
        if (_orderItems.Count == 0)
        {
            string waitText = $"Next order in: {_timeUntilNewOrder:F0}s";
            int waitWidth = MeasureText(waitText, 24);
            DrawText(waitText,
                popupX + (popupW - waitWidth) / 2,
                popupY + popupH / 2 - 12,
                24, Color.LightGray);
            return;
        }

        // Header
        DrawText("Today's Order:", popupX + 24, popupY + 80, 22, Color.White);

        // Each item row
        int rowStartY = popupY + 120;
        int rowHeight = 64;

        bool canComplete = true;
        for (int i = 0; i < _orderItems.Count; i++)
        {
            Item item = _orderItems[i];
            int qty = _orderQuantities[i];
            int rowY = rowStartY + i * rowHeight;

            DrawTexturePro(
                item.Icon,
                new Rectangle(0, 0, item.Icon.Width, item.Icon.Height),
                new Rectangle(popupX + 24, rowY, 48, 48),
                new Vector2(0, 0), 0f, Color.White
            );
            DrawText($"{qty}x {item.Name}", popupX + 90, rowY + 4, 22, Color.White);

            int owned = inventory.CountOf(item);
            bool itemMet = owned >= qty;
            if (!itemMet) canComplete = false;

            Color ownedColor = itemMet
                ? new Color(80, 220, 80, 255)
                : new Color(220, 100, 100, 255);
            DrawText($"You have: {owned}", popupX + 90, rowY + 28, 16, ownedColor);
        }

        // Reward
        int rewardY = rowStartY + _orderItems.Count * rowHeight + 16;
        DrawText($"Reward: {_coinReward} coins + {_xpReward} XP",
            popupX + 24, rewardY, 20, Color.Yellow);

        // Complete button
        Rectangle btn = new Rectangle(
            popupX + popupW / 2 - 80,
            popupY + popupH - 80,
            160, 50
        );
        Color btnColor = canComplete
            ? new Color(80, 140, 60, 255)
            : new Color(70, 70, 70, 255);
        DrawRectangleRec(btn, btnColor);
        DrawRectangleLinesEx(btn, 2, Color.White);

        string btnLabel = "Complete";
        int btnTextWidth = MeasureText(btnLabel, 24);
        DrawText(btnLabel,
            (int)(btn.X + (btn.Width - btnTextWidth) / 2),
            (int)(btn.Y + 12),
            24, Color.White);

        // Click handling
        if (canComplete
            && CheckCollisionPointRec(GetMousePosition(), btn)
            && IsMouseButtonPressed(MouseButton.Left))
        {
            // Consume all items
            for (int i = 0; i < _orderItems.Count; i++)
            {
                Item item = _orderItems[i];
                int qty = _orderQuantities[i];
                for (int n = 0; n < qty; n++)
                {
                    int slot = inventory.FindSlotWith(item);
                    if (slot >= 0) inventory.RemoveOne(slot);
                }
            }

            // Grant rewards
            PlayerStats.AddCoins(_coinReward);
            PlayerStats.AddXP(_xpReward);

            // Reset for next order
            _orderItems.Clear();
            _orderQuantities.Clear();
            _coinReward = 0;
            _xpReward = 0;
            _timeUntilNewOrder = OrderRefreshTime;
        }
    }
}