using System.Collections.Generic;

public class VIPCard : CardData
{
    public override int Id => 1;
    public override string Name => "VIPCard";
    public override string Description => "";

    private static readonly List<Effect> _effects = new();
    
    public override List<Effect> Effects => _effects;
}