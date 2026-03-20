using System.Collections.Generic;

public abstract class Effect
{
    public abstract List<GameAction> GetGameAction(PlayerSide playerSide, Dictionary<PlayerSide, Dictionary<CardZone, List<Card>>> zones);
}
