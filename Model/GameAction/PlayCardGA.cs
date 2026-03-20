public class PlayCardGA : GameAction
{
    public PlayerSide Player { get; set; }
    public int CardId { get; set; }
    public PlayCardGA(PlayerSide player, int cardId)
    {
        Player = player;
        CardId = cardId;
    }
}
