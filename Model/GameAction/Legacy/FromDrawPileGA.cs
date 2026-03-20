public class FromDrawPileGA : GameAction
{
    public int CardId { get; set; }
    public PlayerSide PlayerZone { get; set; }
    public FromDrawPileGA(int cardId, PlayerSide playerZone)
    {
        CardId = cardId;
        PlayerZone = playerZone;
    }
}