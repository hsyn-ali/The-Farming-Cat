using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class Inventory
{
    public int SlotCount { get; }
    public Slot[] Slots { get; }
    public int SelectedIndex { get; set; } = 0;

    public Inventory(int slotCount = 8)
    {
        SlotCount = slotCount;
        Slots = new Slot[slotCount];
        for (int i = 0; i < slotCount; i++)
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

    public int CountOf(Item item)
    {
        int total = 0;
        for (int i = 0; i < SlotCount; i++)
            if (Slots[i].Item == item) total += Slots[i].Count;
        return total;
    }

    public int FindSlotWith(Item item)
    {
        for (int i = 0; i < SlotCount; i++)
            if (Slots[i].Item == item) return i;
        return -1;
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
    private const int SlotSize = 96;
    private const int BottomMargin = 20;

    private readonly Inventory _inventory;

    public Hotbar(Inventory inventory)
    {
        _inventory = inventory;
    }

    public void Update()
    {
        for (int i = 0; i < _inventory.SlotCount; i++)
        {
            if (IsKeyPressed(KeyboardKey.One + i))
                _inventory.SelectedIndex = i;
        }
    }

    public void Draw(int screenWidth, int screenHeight)
    {
        int totalWidth = _inventory.SlotCount * SlotSize;
        int startX = (screenWidth - totalWidth) / 2;
        int y = screenHeight - SlotSize - BottomMargin;

        DrawTexturePro(
            RunTime.Hotbar,
            new Rectangle(0, 0, RunTime.Hotbar.Width, RunTime.Hotbar.Height),
            new Rectangle(startX, y, totalWidth, SlotSize),
            new Vector2(0, 0),
            0f,
            Color.White
        );

        for (int i = 0; i < _inventory.SlotCount; i++)
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

        Slot selected = _inventory.Selected;
        if (selected.Item != null)
        {
            int fontSize = 24;
            int textWidth = MeasureText(selected.Item.Name, fontSize);

            int textX = (screenWidth - textWidth) / 2;
            int textY = y - fontSize - 10;

            int padding = 10;
            DrawRectangle(
                textX - padding,
                textY - 4,
                textWidth + padding * 2,
                fontSize + 8,
                new Color(0, 0, 0, 150)
            );

            DrawText(selected.Item.Name, textX, textY, fontSize, Color.White);
        }
    }
}