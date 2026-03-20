using System.Collections.Generic;

public class FoodWasteFX : Effect
{
    public override List<GameAction> GetGameAction(PlayerSide playerSide, Dictionary<PlayerSide, Dictionary<CardZone, List<Card>>> zones)
    {
        List<GameAction> actions = new List<GameAction>();
        List<int> hand = zones[playerSide][CardZone.Hand].ConvertAll(card => card.Id);
        DiscardCardsGA discardCardsGA = new DiscardCardsGA(playerSide, null);
        GetTargetGA getTargetGA = new GetTargetGA(playerSide, hand, 1, discardCardsGA);
        actions.Add(getTargetGA);
        actions.Add(discardCardsGA);
        return actions;
    }
}
