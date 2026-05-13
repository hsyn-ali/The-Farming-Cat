using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;
public class GoldenCarrotCrop : Crop
{
    protected override Texture2D Sprite => RunTime.GoldenCarrotPlanted;
    protected override float TimePerStage => 40f;
    public override Item HarvestedItem => RunTime.GoldenCarrotHarvestedItem;
}