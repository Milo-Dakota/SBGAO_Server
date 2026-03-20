using System.Collections.Generic;

public class DiscardCardsGA : GameAction, ICardIds
{
    public PlayerSide Player { get; set; }
    public List<int> CardIds { get; set; }
    public DiscardCardsGA(PlayerSide player, List<int> cardIds)
    {
        Player = player;
        CardIds = cardIds;
    }
}