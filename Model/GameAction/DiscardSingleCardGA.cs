public class DiscardSingleCardGA : GameAction
{
    public PlayerSide Player { get; set; }
    public int CardId { get; set; }
    public DiscardSingleCardGA(PlayerSide player, int cardId)
    {
        Player = player;
        CardId = cardId;
    }
}
