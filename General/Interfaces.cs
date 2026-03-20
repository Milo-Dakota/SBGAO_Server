using System.Collections.Generic;

public interface ICards
{
    List<Card> Cards {get; set;}
}

public interface ICard
{
    Card Card { get; set; }
}

public interface ICardIds
{
    List<int> CardIds { get; set; }
}