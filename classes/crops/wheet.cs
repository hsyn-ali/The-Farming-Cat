using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;

public class WheatCrop : Crop
{
    protected override Texture2D Sprite => RunTime.WheatPlanted;
    protected override float TimePerStage => 10f;
    public override Item HarvestedItem => RunTime.WheatHarvestedItem;
}