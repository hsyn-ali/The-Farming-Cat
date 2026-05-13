using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class NoticeBoard : InteractableBuilding
{
    private const float OrderRefreshTime = 5f; 

    private record OrderItem(Item Item, int Quantity);
    private record Order(List<OrderItem> Items, int CoinReward, int XpReward);

    private Order? _currentOrder;
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
        if (_currentOrder == null)
        {
            _timeUntilNewOrder -= dt;
            if (_timeUntilNewOrder <= 0f)
                GenerateNewOrder();
        }
    }

    private void GenerateNewOrder()
    {
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
            _currentOrder = null;
            _timeUntilNewOrder = OrderRefreshTime;
            return;
        }

        // Pick how many items in this order — 1, 2, or 3 (capped by what's unlocked)
        int desiredItemCount = _random.Next(1, 4); // 1..3
        int itemCount = System.Math.Min(desiredItemCount, available.Count);

        // Pick unique items by shuffling and taking the first N
        List<Item> shuffled = new(available);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int swap = _random.Next(i + 1);
            (shuffled[i], shuffled[swap]) = (shuffled[swap], shuffled[i]);
        }

        List<OrderItem> orderItems = new();
        int totalCoins = 0;
        int totalXp = 0;

        for (int i = 0; i < itemCount; i++)
        {
            Item chosen = shuffled[i];
            int qty = _random.Next(1, 11); // 1..10 inclusive
            orderItems.Add(new OrderItem(chosen, qty));
            totalCoins += chosen.SellPrice * qty;
            totalXp += chosen.XpReward * qty;
        }

        // Bonus scales with number of items: 1 -> +20/+25, 2 -> +35/+40, 3 -> +50/+55
        int coinBonus = 20 + (itemCount - 1) * 15;
        int xpBonus = 25 + (itemCount - 1) * 15;

        totalCoins += coinBonus;
        totalXp += xpBonus;

        _currentOrder = new Order(orderItems, totalCoins, totalXp);
    }

    protected override void DrawPopupContent(int popupX, int popupY, int popupW, int popupH, Inventory inventory)
    {
        // No order yet (waiting for next refresh)
        if (_currentOrder == null)
        {
            string waitText = $"Next order in: {_timeUntilNewOrder:F0}s";
            int waitWidth = MeasureText(waitText, 24);
            DrawText(waitText,
                popupX + (popupW - waitWidth) / 2,
                popupY + popupH / 2 - 12,
                24, Color.LightGray);
            return;
        }

        var order = _currentOrder;

        // Header
        DrawText("Today's Order:", popupX + 24, popupY + 80, 22, Color.White);

        // Each item row
        int rowStartY = popupY + 120;
        int rowHeight = 64;

        bool canComplete = true;
        for (int i = 0; i < order.Items.Count; i++)
        {
            var orderItem = order.Items[i];
            int rowY = rowStartY + i * rowHeight;

            DrawTexturePro(
                orderItem.Item.Icon,
                new Rectangle(0, 0, orderItem.Item.Icon.Width, orderItem.Item.Icon.Height),
                new Rectangle(popupX + 24, rowY, 48, 48),
                new Vector2(0, 0), 0f, Color.White
            );
            DrawText($"{orderItem.Quantity}x {orderItem.Item.Name}",
                popupX + 90, rowY + 4, 22, Color.White);

            int owned = inventory.CountOf(orderItem.Item);
            bool itemMet = owned >= orderItem.Quantity;
            if (!itemMet) canComplete = false;

            Color ownedColor = itemMet
                ? new Color(80, 220, 80, 255)
                : new Color(220, 100, 100, 255);
            DrawText($"You have: {owned}", popupX + 90, rowY + 28, 16, ownedColor);
        }

        // Reward
        int rewardY = rowStartY + order.Items.Count * rowHeight + 16;
        DrawText($"Reward: {order.CoinReward} coins + {order.XpReward} XP",
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
            foreach (var orderItem in order.Items)
            {
                for (int i = 0; i < orderItem.Quantity; i++)
                {
                    int slot = inventory.FindSlotWith(orderItem.Item);
                    if (slot >= 0) inventory.RemoveOne(slot);
                }
            }

            // Grant rewards
            PlayerStats.AddCoins(order.CoinReward);
            PlayerStats.AddXP(order.XpReward);

            // Reset for next order
            _currentOrder = null;
            _timeUntilNewOrder = OrderRefreshTime;
        }
    }
}