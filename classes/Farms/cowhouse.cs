using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;
public class CowHouse : AnimalFarm
{
    public CowHouse(Vector2 position) : base(position) { }

    protected override Texture2D Sprite => RunTime.CowHouse;
    protected override float WaitTime => 30f;
    protected override Item ProducedItem => RunTime.MilkItem;
    protected override string Title => "Cow House";
}