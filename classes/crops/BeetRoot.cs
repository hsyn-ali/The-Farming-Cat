using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;
public class BeetrootCrop : Crop
{
    protected override Texture2D Sprite => RunTime.BeetrootPlanted;
    protected override float TimePerStage => 20f;
    public override Item HarvestedItem => RunTime.BeetrootHarvestedItem;
}