using System.Numerics;

namespace Game;

public abstract class Factory : ProductionBuilding
{
    protected override int FrameWidth => 76;
    protected override int FrameHeight => 125;
    protected override float Scale => 2.5f;

    protected override string IdleText => $"Insert {InputItem.Name} to start.";
    protected override string ProcessingVerb => "Processing";
    protected override string ReadyText => $"{OutputItem.Name} ready!";
    protected override string ActionVerb => "Process";

    protected Factory(Vector2 position) : base(position) { }
}