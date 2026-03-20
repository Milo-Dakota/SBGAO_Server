public class PerformEffectGA : GameAction
{
    public PlayerSide Player { get; set; }
    public Effect Effect { get; set; }
    public PerformEffectGA(PlayerSide player, Effect effect)
    {
        Player = player;
        Effect = effect;
    }
}
