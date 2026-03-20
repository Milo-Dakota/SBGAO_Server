public class FromHandGA : GameAction
{
    public int CardId { get; set; }
    public PlayerSide PlayerZone { get; set; }
    public FromHandGA(int cardId, PlayerSide playerZone)
    {
        CardId = cardId;
        PlayerZone = playerZone;
    }
}
