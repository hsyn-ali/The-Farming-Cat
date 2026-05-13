using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class Chest : InteractableBuilding
{
    public Inventory Storage { get; } = new Inventory(24);

    protected override Texture2D Sprite => RunTime.ChestSprite;
    protected override int FrameWidth => 18;
    protected override int FrameHeight => 13;
    protected override float Scale => 6f;
    protected override string Title => "Chest";

    protected override int PopupWidth => 720;
    protected override int PopupHeight => 560;

    public override int RequiredLevel => 1;
    public override bool RequiresPurchase => false;

    public Chest(Vector2 position) : base(position) { }

    public override void Update(float dt) { }

    protected override void DrawPopupContent(int popupX, int popupY, int popupW, int popupH, Inventory inventory)
    {
        const int slotSize = 56;
        const int slotGap = 8;
        const int storageCols = 6;
        const int storageRows = 4;
        const int invCols = 8;

        // STORAGE GRID (top)
        int storageWidth = storageCols * slotSize + (storageCols - 1) * slotGap;
        int storageX = popupX + (popupW - storageWidth) / 2;
        int storageY = popupY + 80;

        DrawText("Storage", storageX, storageY - 28, 22, Color.White);
        DrawGrid(Storage, storageX, storageY, slotSize, slotGap, storageCols, fromGrid => Transfer(fromGrid, Storage, inventory));

        // INVENTORY GRID (bottom)
        int invWidth = invCols * slotSize + (invCols - 1) * slotGap;
        int invX = popupX + (popupW - invWidth) / 2;
        int invY = storageY + storageRows * (slotSize + slotGap) + 50;

        DrawText("Inventory", invX, invY - 28, 22, Color.White);
        DrawGrid(inventory, invX, invY, slotSize, slotGap, invCols, fromGrid => Transfer(fromGrid, inventory, Storage));

        // Hint text
        string hint = "Left-click: 1 item   |   Right-click: whole stack";
        int hintWidth = MeasureText(hint, 16);
        DrawText(hint, popupX + (popupW - hintWidth) / 2, popupY + popupH - 40, 16, Color.LightGray);
    }

    private void DrawGrid(Inventory grid, int startX, int startY, int slotSize, int gap, int cols, System.Action<int> onClick)
    {
        for (int i = 0; i < grid.SlotCount; i++)
        {
            int row = i / cols;
            int col = i % cols;
            int x = startX + col * (slotSize + gap);
            int y = startY + row * (slotSize + gap);

            // Slot background
            DrawRectangle(x, y, slotSize, slotSize, new Color(40, 30, 20, 220));
            DrawRectangleLines(x, y, slotSize, slotSize, new Color(120, 90, 60, 255));

            Slot slot = grid.Slots[i];
            if (slot.Item != null)
            {
                int pad = 6;
                DrawTexturePro(
                    slot.Item.Icon,
                    new Rectangle(0, 0, slot.Item.Icon.Width, slot.Item.Icon.Height),
                    new Rectangle(x + pad, y + pad, slotSize - pad * 2, slotSize - pad * 2),
                    new Vector2(0, 0), 0f, Color.White
                );
                DrawText($"{slot.Count}", x + slotSize - 18, y + slotSize - 18, 14, Color.White);
            }

            // Click detection
            Rectangle slotRect = new Rectangle(x, y, slotSize, slotSize);
            if (CheckCollisionPointRec(GetMousePosition(), slotRect) && slot.Item != null)
            {
                if (IsMouseButtonPressed(MouseButton.Left)) onClick(i);
                else if (IsMouseButtonPressed(MouseButton.Right))
                {
                    int stackCount = slot.Count;
                    for (int n = 0; n < stackCount; n++) onClick(i);
                }
            }
        }
    }

    private void Transfer(int fromIndex, Inventory from, Inventory to)
    {
        Slot slot = from.Slots[fromIndex];
        if (slot.Item == null) return;
        Item item = slot.Item;
        if (to.Add(item, 1) > 0)
            from.RemoveOne(fromIndex);
    }
}