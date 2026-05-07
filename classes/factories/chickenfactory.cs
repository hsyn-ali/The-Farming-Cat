using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;
public class ChickenFactory : Factory
{
    public ChickenFactory(Vector2 position) : base(position) { }

    protected override Texture2D Sprite => RunTime.ChickenFactory;
    protected override float WaitTime => 10f;
    protected override Item InputItem => RunTime.WheatHarvestedItem;
    protected override Item OutputItem => RunTime.ChickenFeedItem;
    protected override string Title => "Chicken Feed Factory";
}