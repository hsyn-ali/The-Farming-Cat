using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Game;
public class CarrotCrop : Crop
{
    protected override Texture2D Sprite => RunTime.CarrotPlanted;
    protected override float TimePerStage => 15f;
    public override Item HarvestedItem => RunTime.CarrotHarvestedItem;
}