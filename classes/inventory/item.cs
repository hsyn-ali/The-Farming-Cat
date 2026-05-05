using Raylib_cs;

namespace Game;

public class Item
{
    public string Name { get; }
    public Texture2D Icon { get; }
    public int MaxStack { get; }

    public Item(string name, Texture2D icon, int maxStack = 99)
    {
        Name = name;
        Icon = icon;
        MaxStack = maxStack;
    }
}