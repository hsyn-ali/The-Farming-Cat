using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class Inventory
{
    public const int SlotCount = 8;

    public Slot[] Slots { get; } = new Slot[SlotCount];
    public int SelectedIndex { get; set; } = 0;

    public Inventory()
    {
        for (int i = 0; i < SlotCount; i++)
            Slots[i] = new Slot();
    }

    public int Add(Item item, int count)
    {
        int remaining = count;

        for (int i = 0; i < SlotCount && remaining > 0; i++)
        {
            if (Slots[i].Item == item && Slots[i].Count < item.MaxStack)
            {
                int space = item.MaxStack - Slots[i].Count;
                int toAdd = remaining < space ? remaining : space;
                Slots[i].Count += toAdd;
                remaining -= toAdd;
            }
        }

        for (int i = 0; i < SlotCount && remaining > 0; i++)
        {
            if (Slots[i].Item == null)
            {
                int toAdd = remaining < item.MaxStack ? remaining : item.MaxStack;
                Slots[i].Item = item;
                Slots[i].Count = toAdd;
                remaining -= toAdd;
            }
        }

        return count - remaining;
    }

    public void RemoveOne(int slotIndex)
{
    Slot slot = Slots[slotIndex];
    if (slot.Item == null) return;

    slot.Count--;
    if (slot.Count <= 0)
    {
        slot.Item = null;
        slot.Count = 0;
    }
}

    public Slot Selected => Slots[SelectedIndex];
}

public class Slot
{
    public Item? Item;
    public int Count;
}

public class Hotbar
{
    private const int SlotSize = 96;       // 32 source × 3 scale
    private const int BottomMargin = 20;

    private readonly Inventory _inventory;

    public Hotbar(Inventory inventory)
    {
        _inventory = inventory;
    }

    public void Update()
    {
        for (int i = 0; i < Inventory.SlotCount; i++)
        {
            if (IsKeyPressed(KeyboardKey.One + i))
                _inventory.SelectedIndex = i;
        }
    }

    public void Draw(int screenWidth, int screenHeight)
    {
        int totalWidth = Inventory.SlotCount * SlotSize;
        int startX = (screenWidth - totalWidth) / 2;
        int y = screenHeight - SlotSize - BottomMargin;

        // Draw the whole hotbar sprite once, scaled
        DrawTexturePro(
            RunTime.Hotbar,
            new Rectangle(0, 0, RunTime.Hotbar.Width, RunTime.Hotbar.Height),
            new Rectangle(startX, y, totalWidth, SlotSize),
            new Vector2(0, 0),
            0f,
            Color.White
        );

        // Draw items + selection on top, slot by slot
        for (int i = 0; i < Inventory.SlotCount; i++)
        {
            int x = startX + i * SlotSize;
            Rectangle slotRect = new Rectangle(x, y, SlotSize, SlotSize);

            Slot slot = _inventory.Slots[i];
            if (slot.Item != null)
            {
                DrawTexturePro(
                    slot.Item.Icon,
                    new Rectangle(0, 0, slot.Item.Icon.Width, slot.Item.Icon.Height),
                    slotRect,
                    new Vector2(0, 0),
                    0f,
                    Color.White
                );

                if (slot.Count > 1)
                {
                    string countText = slot.Count.ToString();
                    DrawText(countText, x + SlotSize - 22, y + SlotSize - 22, 20, Color.White);
                }
            }

            if (i == _inventory.SelectedIndex)
                DrawRectangleLinesEx(slotRect, 4, Color.Yellow);
        }
    }

    
}