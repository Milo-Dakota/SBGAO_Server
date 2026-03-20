using System.Collections.Generic;

public abstract class CardData
{
    public abstract int Id { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract List<Effect> Effects { get; }
}