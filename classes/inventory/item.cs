using Raylib_cs;
using System;

namespace Game;

public class Item
{
    public string Name { get; }
    public Texture2D Icon { get; }
    public int MaxStack { get; }
    public Func<Crop>? CreateCrop { get; }

    // XP earned when collecting/harvesting this item
    public int XpReward { get; }

    public Item(
        string name,
        Texture2D icon,
        int maxStack = 99,
        Func<Crop>? createCrop = null,
        int xpReward = 0)
    {
        Name = name;
        Icon = icon;
        MaxStack = maxStack;
        CreateCrop = createCrop;
        XpReward = xpReward;
    }

    public bool IsPlantable => CreateCrop != null;
}