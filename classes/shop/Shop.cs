using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class Shop : InteractableBuilding
{
    private bool _buyTab = true;
    private readonly Dictionary<string, int> _qty = new();

    protected override Texture2D Sprite => RunTime.ShopBuilding;
    protected override int FrameWidth => 64;
    protected override int FrameHeight => 96;
    protected override float Scale => 3f;
    protected override string Title => "Shop";
    protected override int PopupWidth => 800;
    protected override int PopupHeight => 550;

    public override int RequiredLevel => UnlockManager.ShopLevel;
    public override bool RequiresPurchase => false;

    public Shop(Vector2 position) : base(position) { }
    public override void Update(float dt) { }

    protected override void DrawPopupContent(int px, int py, int pw, int ph, Inventory inventory)
    {
        var mouse = GetMousePosition();
        bool click = IsMouseButtonPressed(MouseButton.Left);

        // Coins (top right)
        string coins = $"{PlayerStats.Coins} coins";
        DrawText(coins, px + pw - MeasureText(coins, 22) - 60, py + 30, 22, Color.Yellow);

        // Buy tab
        var buyTab = new Rectangle(px + 24, py + 70, 120, 40);
        DrawRectangleRec(buyTab, _buyTab ? new Color(120, 90, 60, 255) : new Color(70, 55, 40, 255));
        DrawRectangleLinesEx(buyTab, 2, Color.White);
        DrawText("Buy", (int)buyTab.X + (120 - MeasureText("Buy", 22)) / 2, (int)buyTab.Y + 10, 22, Color.White);
        if (CheckCollisionPointRec(mouse, buyTab) && click) _buyTab = true;

        // Sell tab
        var sellTab = new Rectangle(px + 152, py + 70, 120, 40);
        DrawRectangleRec(sellTab, !_buyTab ? new Color(120, 90, 60, 255) : new Color(70, 55, 40, 255));
        DrawRectangleLinesEx(sellTab, 2, Color.White);
        DrawText("Sell", (int)sellTab.X + (120 - MeasureText("Sell", 22)) / 2, (int)sellTab.Y + 10, 22, Color.White);
        if (CheckCollisionPointRec(mouse, sellTab) && click) _buyTab = false;

        // Build rows for active tab
        // Each row: (Item, price, requiredLevel, maxQty)
        // For buy: price = item price, max = int.MaxValue
        // For sell: price = sell price, max = owned count, requiredLevel = 1 if owned > 0 else -1
        var rows = new List<(Item item, int price, int reqLevel, int max, bool isBuy)>();

        if (_buyTab)
        {
            rows.Add((RunTime.WheatSeedItem, 3, 1, int.MaxValue, true));
            rows.Add((RunTime.CarrotSeedItem, 8, UnlockManager.CarrotSeedLevel, int.MaxValue, true));
            rows.Add((RunTime.BeetrootSeedItem, 12, UnlockManager.BeetrootSeedLevel, int.MaxValue, true));
            rows.Add((RunTime.ChickenFeedItem, 10, UnlockManager.ChickenLevel, int.MaxValue, true));
            rows.Add((RunTime.CowFeedItem, 15, UnlockManager.CowLevel, int.MaxValue, true));
            rows.Add((RunTime.SheepFeedItem, 20, UnlockManager.SheepLevel, int.MaxValue, true));
            rows.Add((RunTime.GoldenCarrotSeedItem, 50, UnlockManager.GoldenCarrotSeedLevel, int.MaxValue, true));
        }
        else
        {
            Item[] sellables = {
                RunTime.WheatHarvestedItem, RunTime.CarrotHarvestedItem, RunTime.BeetrootHarvestedItem,
                RunTime.EggItem, RunTime.MilkItem, RunTime.WoolItem, RunTime.GoldenCarrotHarvestedItem,
            };
            foreach (var item in sellables)
                rows.Add((item, item.SellPrice, 1, inventory.CountOf(item), false));
        }

        // Draw rows
        int x = px + 24;
        int w = pw - 48;
        int baseY = py + 70 + 56;

        for (int i = 0; i < rows.Count; i++)
        {
            var (item, price, reqLevel, max, isBuy) = rows[i];
            int y = baseY + i * 56;
            bool available = isBuy ? PlayerStats.Level >= reqLevel : max > 0;

            // Icon
            DrawTexturePro(item.Icon,
                new Rectangle(0, 0, item.Icon.Width, item.Icon.Height),
                new Rectangle(x, y, 40, 40),
                Vector2.Zero, 0f,
                available ? Color.White : new Color(120, 120, 120, 200));

            // Name + price
            string name = isBuy ? item.Name : $"{item.Name} (x{max})";
            DrawText(name, x + 50, y + 4, 18, available ? Color.White : Color.Gray);
            DrawText($"{price} coins each", x + 50, y + 24, 14, Color.LightGray);

            // Locked: show requirement and skip controls
            if (isBuy && !available)
            {
                DrawText($"Requires Level {reqLevel}", x + w - 200, y + 12, 18, new Color(220, 100, 100, 255));
                continue;
            }
            if (!isBuy && max == 0) continue;

            // Init / clamp quantity
            if (!_qty.ContainsKey(item.Name)) _qty[item.Name] = 1;
            _qty[item.Name] = System.Math.Clamp(_qty[item.Name], 1, max);
            int qty = _qty[item.Name];
            int total = qty * price;

            // Quantity controls
            int ctrlX = x + w - 240;
            var minusBtn = new Rectangle(ctrlX, y + 4, 32, 32);
            var plusBtn = new Rectangle(ctrlX + 70, y + 4, 32, 32);

            DrawRectangleRec(minusBtn, new Color(80, 80, 80, 255));
            DrawText("-", (int)minusBtn.X + 11, (int)minusBtn.Y + 4, 24, Color.White);
            DrawText($"{qty}", ctrlX + 42, y + 8, 22, Color.White);
            DrawRectangleRec(plusBtn, new Color(80, 80, 80, 255));
            DrawText("+", (int)plusBtn.X + 9, (int)plusBtn.Y + 4, 24, Color.White);

            if (CheckCollisionPointRec(mouse, minusBtn) && click)
                _qty[item.Name] = System.Math.Max(1, qty - 1);
            if (CheckCollisionPointRec(mouse, plusBtn) && click)
                _qty[item.Name] = System.Math.Min(max, qty + 1);

            // Action button
            bool canAfford = !isBuy || PlayerStats.Coins >= total;
            var actionBtn = new Rectangle(ctrlX + 110, y + 4, 110, 32);
            Color btnColor = isBuy
                ? (canAfford ? new Color(80, 140, 60, 255) : new Color(70, 70, 70, 255))
                : new Color(180, 130, 50, 255);
            DrawRectangleRec(actionBtn, btnColor);
            DrawRectangleLinesEx(actionBtn, 1, Color.White);

            string label = isBuy ? $"Buy ({total})" : $"Sell (+{total})";
            DrawText(label,
                (int)(actionBtn.X + (actionBtn.Width - MeasureText(label, 16)) / 2),
                (int)(actionBtn.Y + 8), 16, Color.White);

            // Handle click
            if (CheckCollisionPointRec(mouse, actionBtn) && click)
            {
                if (isBuy && canAfford && PlayerStats.SpendCoins(total))
                {
                    inventory.Add(item, qty);
                    SoundManager.Play(RunTime.BuySellSound);
                }
                else if (!isBuy)
                {
                    for (int q = 0; q < qty; q++)
                    {
                        int slot = inventory.FindSlotWith(item);
                        if (slot < 0) break;
                        inventory.RemoveOne(slot);
                    }
                    PlayerStats.AddCoins(total);
                    SoundManager.Play(RunTime.BuySellSound);
                    _qty[item.Name] = 1;
                }
            }
        }
    }
}