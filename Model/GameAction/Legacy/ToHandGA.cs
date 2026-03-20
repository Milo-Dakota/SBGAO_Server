public class ToHandGA : GameAction
{
    public int CardId { get; set; }
    public PlayerSide PlayerZone { get; set; }
    public ToHandGA(int cardId, PlayerSide playerZone)
    {
         CardId = cardId;
         PlayerZone = playerZone;
    }
}