using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;
public class SheepFactory : Factory
{
    public SheepFactory(Vector2 position) : base(position) { }

    protected override Texture2D Sprite => RunTime.SheepFactory;
    protected override float WaitTime => 60f;
    protected override Item InputItem => RunTime.BeetrootHarvestedItem;
    protected override Item OutputItem => RunTime.SheepFeedItem;
    protected override string Title => "Sheep Feed Factory";
    public override int RequiredLevel => UnlockManager.SheepLevel;  // 7
    public override int PurchaseCost => 350;
}