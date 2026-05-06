using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;
public class SheepHouse : AnimalFarm
{
    public SheepHouse(Vector2 position) : base(position) { }

    protected override Texture2D Sprite => RunTime.SheepHouse;
    protected override float WaitTime => 30f;
    protected override Item ProducedItem => RunTime.WoolItem;
    protected override Item FeedItem => RunTime.SheepFeedItem;
    protected override string Title => "Sheep House";
}