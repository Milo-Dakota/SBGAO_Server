using System.Collections.Generic;

public class GetTargetGA : GameAction
{
    public PlayerSide Player { get; set; }
    public List<int> Sourse { get; set; }
    public int Amount { get; set; }
    public List<int> Selected { get; set; }
    public ICardIds CardIdsInterface { get; set; }

    public GetTargetGA(PlayerSide player, List<int> sourse, int amount, ICardIds cardIdsInterface)
    {
        Player = player;
        Sourse = sourse;
        CardIdsInterface = cardIdsInterface;
        Amount = amount;
    }
}
