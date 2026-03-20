using System.Collections.Generic;

public class Card
{
    public string Name => data.Name;
    public string Description => data.Description;
    public List<Effect> Effects => data.Effects;

    public int Id { get; set; }

    private readonly CardData data;

    public Card(CardData cardData, int id)
    {
        data = cardData;
        Id = id;
    }
}