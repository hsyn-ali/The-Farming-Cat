using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;
public class ChickenHouse : AnimalFarm
{
    public ChickenHouse(Vector2 position) : base(position) { }

    protected override Texture2D Sprite => RunTime.ChickenHouse;
    protected override float WaitTime => 30f;
    protected override Item ProducedItem => RunTime.EggItem;
    protected override Item FeedItem => RunTime.ChickenFeedItem;
    protected override string Title => "Chicken House";
    public override Sound CollectSound => RunTime.ChickenSound;

    public override int RequiredLevel => UnlockManager.ChickenLevel;  // 4
    public override int PurchaseCost => 200;
}