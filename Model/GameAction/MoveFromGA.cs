public class MoveFromGA : GameAction
{
    public int CardId { get; set; }
    public PlayerSide PlayerSide { get; set; }
    public CardZone CardZone { get; set; }
    public MoveFromGA(int cardId, PlayerSide playerSide, CardZone cardZone)
    {
        CardId = cardId;
        PlayerSide = playerSide;
        CardZone = cardZone;
    }
}