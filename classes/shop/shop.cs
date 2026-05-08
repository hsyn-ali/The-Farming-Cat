using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class Shop : InteractableBuilding
{
    private enum Tab { Buy, Sell }
    private Tab _activeTab = Tab.Buy;

    // Per-item buy quantities (keyed by item name)
    private readonly Dictionary<string, int> _buyQuantities = new();
    // Per-item sell quantities (keyed by item name)
    private readonly Dictionary<string, int> _sellQuantities = new();

    protected override Texture2D Sprite => RunTime.ShopBuilding;
    protected override int FrameWidth => 64;
    protected override int FrameHeight => 96;
    protected override float Scale => 3f;
    protected override string Title => "Shop";

    protected override int PopupWidth => 800;
    protected override int PopupHeight => 550;

    public override int RequiredLevel => UnlockManager.ShopLevel;  // 2
    public override bool RequiresPurchase => false;

    public Shop(Vector2 position) : base(position) { }

    public override void Update(float dt) { }

    private record BuyEntry(Item Item, int Price, int RequiredLevel);
    private record SellEntry(Item Item, int Price);

    private List<BuyEntry> GetBuyableItems() => new()
    {
        new BuyEntry(RunTime.WheatSeedItem, 3, 1),
        new BuyEntry(RunTime.CarrotSeedItem, 8, UnlockManager.CarrotSeedLevel),
        new BuyEntry(RunTime.BeetrootSeedItem, 12, UnlockManager.BeetrootSeedLevel),
        new BuyEntry(RunTime.ChickenFeedItem, 10, UnlockManager.ChickenLevel),
        new BuyEntry(RunTime.CowFeedItem, 15, UnlockManager.CowLevel),
        new BuyEntry(RunTime.SheepFeedItem, 15, UnlockManager.SheepLevel),
        new BuyEntry(RunTime.GoldenCarrotSeedItem, 50, UnlockManager.GoldenCarrotSeedLevel)
    };

    private List<SellEntry> GetSellableItems() => new()
    {
        new SellEntry(RunTime.WheatHarvestedItem, 5),
        new SellEntry(RunTime.CarrotHarvestedItem, 10),
        new SellEntry(RunTime.BeetrootHarvestedItem, 15),
        new SellEntry(RunTime.EggItem, 20),
        new SellEntry(RunTime.MilkItem, 25),
        new SellEntry(RunTime.WoolItem, 25),
        new SellEntry(RunTime.GoldenCarrotHarvestedItem, 60),
    };

    protected override void DrawPopupContent(int popupX, int popupY, int popupW, int popupH, Inventory inventory)
    {
        // Coin display top-right
        string coinText = $"{PlayerStats.Coins} coins";
        int coinTextWidth = MeasureText(coinText, 22);
        DrawText(coinText, popupX + popupW - coinTextWidth - 60, popupY + 30, 22, Color.Yellow);

        // Tab buttons
        int tabY = popupY + 70;
        int tabHeight = 40;
        int tabWidth = 120;

        Rectangle buyTab = new Rectangle(popupX + 24, tabY, tabWidth, tabHeight);
        Rectangle sellTab = new Rectangle(popupX + 24 + tabWidth + 8, tabY, tabWidth, tabHeight);

        DrawTab(buyTab, "Buy", _activeTab == Tab.Buy);
        DrawTab(sellTab, "Sell", _activeTab == Tab.Sell);

        if (CheckCollisionPointRec(GetMousePosition(), buyTab) && IsMouseButtonPressed(MouseButton.Left))
            _activeTab = Tab.Buy;
        if (CheckCollisionPointRec(GetMousePosition(), sellTab) && IsMouseButtonPressed(MouseButton.Left))
            _activeTab = Tab.Sell;

        // Content area
        int contentX = popupX + 24;
        int contentY = tabY + tabHeight + 16;
        int rowHeight = 56;

        if (_activeTab == Tab.Buy)
            DrawBuyTab(contentX, contentY, popupW - 48, rowHeight, inventory);
        else
            DrawSellTab(contentX, contentY, popupW - 48, rowHeight, inventory);
    }

    private void DrawTab(Rectangle rect, string label, bool active)
    {
        Color bg = active ? new Color(120, 90, 60, 255) : new Color(70, 55, 40, 255);
        DrawRectangleRec(rect, bg);
        DrawRectangleLinesEx(rect, 2, Color.White);
        int textWidth = MeasureText(label, 22);
        DrawText(label,
            (int)(rect.X + (rect.Width - textWidth) / 2),
            (int)(rect.Y + 10),
            22, Color.White);
    }

    private void DrawBuyTab(int x, int y, int width, int rowHeight, Inventory inventory)
    {
        var items = GetBuyableItems();
        for (int i = 0; i < items.Count; i++)
        {
            var entry = items[i];
            int rowY = y + i * rowHeight;
            bool levelOk = PlayerStats.Level >= entry.RequiredLevel;

            if (!_buyQuantities.ContainsKey(entry.Item.Name))
                _buyQuantities[entry.Item.Name] = 1;

            int qty = _buyQuantities[entry.Item.Name];
            int totalPrice = qty * entry.Price;

            // Icon
            DrawTexturePro(
                entry.Item.Icon,
                new Rectangle(0, 0, entry.Item.Icon.Width, entry.Item.Icon.Height),
                new Rectangle(x, rowY, 40, 40),
                new Vector2(0, 0), 0f,
                levelOk ? Color.White : new Color(120, 120, 120, 200)
            );

            // Name + price
            Color textColor = levelOk ? Color.White : Color.Gray;
            DrawText(entry.Item.Name, x + 50, rowY + 4, 18, textColor);
            DrawText($"{entry.Price} coins each", x + 50, rowY + 24, 14, Color.LightGray);

            if (!levelOk)
            {
                DrawText($"Requires Level {entry.RequiredLevel}",
                    x + width - 200, rowY + 12, 18, new Color(220, 100, 100, 255));
                continue;
            }

            // Quantity controls
            int controlsX = x + width - 240;
            Rectangle minusBtn = new Rectangle(controlsX, rowY + 4, 32, 32);
            Rectangle plusBtn = new Rectangle(controlsX + 70, rowY + 4, 32, 32);

            DrawRectangleRec(minusBtn, new Color(80, 80, 80, 255));
            DrawText("-", (int)minusBtn.X + 11, (int)minusBtn.Y + 4, 24, Color.White);

            DrawText($"{qty}", controlsX + 42, rowY + 8, 22, Color.White);

            DrawRectangleRec(plusBtn, new Color(80, 80, 80, 255));
            DrawText("+", (int)plusBtn.X + 9, (int)plusBtn.Y + 4, 24, Color.White);

            if (CheckCollisionPointRec(GetMousePosition(), minusBtn) && IsMouseButtonPressed(MouseButton.Left))
                _buyQuantities[entry.Item.Name] = System.Math.Max(1, qty - 1);
            if (CheckCollisionPointRec(GetMousePosition(), plusBtn) && IsMouseButtonPressed(MouseButton.Left))
                _buyQuantities[entry.Item.Name] = qty + 1;

            // Buy button
            bool canAfford = PlayerStats.Coins >= totalPrice;
            Rectangle buyBtn = new Rectangle(controlsX + 110, rowY + 4, 110, 32);
            Color buyColor = canAfford ? new Color(80, 140, 60, 255) : new Color(70, 70, 70, 255);
            DrawRectangleRec(buyBtn, buyColor);
            DrawRectangleLinesEx(buyBtn, 1, Color.White);

            string buyLabel = $"Buy ({totalPrice})";
            int buyTextWidth = MeasureText(buyLabel, 16);
            DrawText(buyLabel,
                (int)(buyBtn.X + (buyBtn.Width - buyTextWidth) / 2),
                (int)(buyBtn.Y + 8),
                16, Color.White);

            if (canAfford
                && CheckCollisionPointRec(GetMousePosition(), buyBtn)
                && IsMouseButtonPressed(MouseButton.Left))
            {
                if (PlayerStats.SpendCoins(totalPrice))
                {
                    inventory.Add(entry.Item, qty);
                }
            }
        }
    }

    private void DrawSellTab(int x, int y, int width, int rowHeight, Inventory inventory)
    {
        var items = GetSellableItems();
        for (int i = 0; i < items.Count; i++)
        {
            var entry = items[i];
            int rowY = y + i * rowHeight;
            int owned = inventory.CountOf(entry.Item);

            if (!_sellQuantities.ContainsKey(entry.Item.Name))
                _sellQuantities[entry.Item.Name] = 1;

            int qty = System.Math.Min(_sellQuantities[entry.Item.Name], System.Math.Max(1, owned));
            _sellQuantities[entry.Item.Name] = qty;
            int totalPrice = qty * entry.Price;

            // Icon
            DrawTexturePro(
                entry.Item.Icon,
                new Rectangle(0, 0, entry.Item.Icon.Width, entry.Item.Icon.Height),
                new Rectangle(x, rowY, 40, 40),
                new Vector2(0, 0), 0f,
                owned > 0 ? Color.White : new Color(120, 120, 120, 200)
            );

            // Name + owned + price
            Color textColor = owned > 0 ? Color.White : Color.Gray;
            DrawText($"{entry.Item.Name} (x{owned})", x + 50, rowY + 4, 18, textColor);
            DrawText($"{entry.Price} coins each", x + 50, rowY + 24, 14, Color.LightGray);

            if (owned == 0) continue;

            // Quantity controls
            int controlsX = x + width - 240;
            Rectangle minusBtn = new Rectangle(controlsX, rowY + 4, 32, 32);
            Rectangle plusBtn = new Rectangle(controlsX + 70, rowY + 4, 32, 32);

            DrawRectangleRec(minusBtn, new Color(80, 80, 80, 255));
            DrawText("-", (int)minusBtn.X + 11, (int)minusBtn.Y + 4, 24, Color.White);

            DrawText($"{qty}", controlsX + 42, rowY + 8, 22, Color.White);

            DrawRectangleRec(plusBtn, new Color(80, 80, 80, 255));
            DrawText("+", (int)plusBtn.X + 9, (int)plusBtn.Y + 4, 24, Color.White);

            if (CheckCollisionPointRec(GetMousePosition(), minusBtn) && IsMouseButtonPressed(MouseButton.Left))
                _sellQuantities[entry.Item.Name] = System.Math.Max(1, qty - 1);
            if (CheckCollisionPointRec(GetMousePosition(), plusBtn) && IsMouseButtonPressed(MouseButton.Left))
                _sellQuantities[entry.Item.Name] = System.Math.Min(owned, qty + 1);

            // Sell button
            Rectangle sellBtn = new Rectangle(controlsX + 110, rowY + 4, 110, 32);
            DrawRectangleRec(sellBtn, new Color(180, 130, 50, 255));
            DrawRectangleLinesEx(sellBtn, 1, Color.White);

            string sellLabel = $"Sell (+{totalPrice})";
            int sellTextWidth = MeasureText(sellLabel, 16);
            DrawText(sellLabel,
                (int)(sellBtn.X + (sellBtn.Width - sellTextWidth) / 2),
                (int)(sellBtn.Y + 8),
                16, Color.White);

            if (CheckCollisionPointRec(GetMousePosition(), sellBtn) && IsMouseButtonPressed(MouseButton.Left))
            {
                // Remove items
                for (int q = 0; q < qty; q++)
                {
                    int slot = inventory.FindSlotWith(entry.Item);
                    if (slot < 0) break;
                    inventory.RemoveOne(slot);
                }
                PlayerStats.AddCoins(totalPrice);
                _sellQuantities[entry.Item.Name] = 1;
            }
        }
    }
}