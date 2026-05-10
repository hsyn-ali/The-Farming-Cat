using System;
using System.Numerics;

namespace Game;

public static class BuildingFactory
{
    public static InteractableBuilding Create(string type, Vector2 position)
    {
        switch (type)
        {
            case "ChickenHouse":   return new ChickenHouse(position);
            case "CowHouse":       return new CowHouse(position);
            case "SheepHouse":     return new SheepHouse(position);
            case "ChickenFactory": return new ChickenFactory(position);
            case "CowFactory":     return new CowFactory(position);
            case "SheepFactory":   return new SheepFactory(position);
            case "Shop":           return new Shop(position);
            case "Chest":          return new Chest(position);
            case "NoticeBoard":    return new NoticeBoard(position);
            default:
                throw new ArgumentException($"Unknown building type: {type}");
        }
    }
}