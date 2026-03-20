public class DrawSingleCardGA : GameAction
{
    public PlayerSide Player { get; private set; }
    public DrawSingleCardGA(PlayerSide player)
    {
        Player = player;
    }
}