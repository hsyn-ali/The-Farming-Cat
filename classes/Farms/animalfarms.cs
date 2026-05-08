using System.Numerics;

namespace Game;

public abstract class AnimalFarm : ProductionBuilding
{
    protected override int FrameWidth => 38;
    protected override int FrameHeight => 48;
    protected override float Scale => 4f;

    protected abstract Item ProducedItem { get; }
    protected abstract Item FeedItem { get; }

    protected override Item InputItem => FeedItem;
    protected override Item OutputItem => ProducedItem;

    protected override string IdleText => "The animals are hungry.";
    protected override string ProcessingVerb => "Feeding";
    protected override string ReadyText => "Ready to collect!";
    protected override string ActionVerb => "Feed";

    protected AnimalFarm(Vector2 position) : base(position) { }
}