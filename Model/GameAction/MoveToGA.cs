public class MoveToGA : GameAction
{
    public int CardId { get; set; }
    public PlayerSide PlayerSide { get; set; }
    public CardZone CardZone { get; set; }
    public MoveToGA(int cardId, PlayerSide playerSide, CardZone cardZone)
    {
        CardId = cardId;
        PlayerSide = playerSide;
        CardZone = cardZone;
    }
}