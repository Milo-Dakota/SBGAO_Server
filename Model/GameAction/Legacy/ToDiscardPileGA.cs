public class ToDiscardPileGA : GameAction
{
    public int CardId { get; set; }
    public PlayerSide PlayerZone { get; set; }
    public ToDiscardPileGA(int cardId, PlayerSide playerZone)
    {
        CardId = cardId;
        PlayerZone = playerZone;
    }
}
