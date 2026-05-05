using Raylib_cs;
using System;

namespace Game;

public class Item
{
    public string Name { get; }
    public Texture2D Icon { get; }
    public int MaxStack { get; }
    public Func<Crop>? CreateCrop { get; }

    public Item(string name, Texture2D icon, int maxStack = 99, Func<Crop>? createCrop = null)
    {
        Name = name;
        Icon = icon;
        MaxStack = maxStack;
        CreateCrop = createCrop;
    }

    public bool IsPlantable => CreateCrop != null;
}