public class DrawCardsGA : GameAction
{
    public PlayerSide Player { get; set; }
    public int drawAmount;
    public DrawCardsGA(PlayerSide player, int amount)
    {
        Player = player;
        drawAmount = amount;
    }
}
