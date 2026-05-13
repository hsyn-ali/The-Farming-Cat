using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;
public class CowFactory : Factory
{
    public CowFactory(Vector2 position) : base(position) { }

    protected override Texture2D Sprite => RunTime.CowFactory;
    protected override float WaitTime => 45f;
    protected override Item InputItem => RunTime.CarrotHarvestedItem;
    protected override Item OutputItem => RunTime.CowFeedItem;
    protected override string Title => "Cow Feed Factory";
    public override int RequiredLevel => UnlockManager.CowLevel;  // 6
    public override int PurchaseCost => 250;
}