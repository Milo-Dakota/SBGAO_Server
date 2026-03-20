using System.Collections.Generic;

public class FoodWaste : CardData
{
    public override int Id => 1;
    public override string Name => "FoodWaste";
    public override string Description => "Discard a card from hand";

    private static readonly List<Effect> _effects = new List<Effect> { new FoodWasteFX() };
    
    public override List<Effect> Effects => _effects;
}