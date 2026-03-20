using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class CardSystem
{
    private readonly Dictionary<PlayerSide, Dictionary<CardZone, List<Card>>> zones = new()
    {
        { PlayerSide.Player1, new()
            {
                { CardZone.Hand, new() },
                { CardZone.DrawPile, new() },
                { CardZone.DiscardPile, new() }
            }
        },
        { PlayerSide.Player2, new()
            {
                { CardZone.Hand, new() },
                { CardZone.DrawPile, new() },
                { CardZone.DiscardPile, new() }
            }
        }
    };

    public Dictionary<int, Card> CardIdToCard { get; } = new();

    private readonly ActionSystem _actionSystem;
    private readonly IdSystem _idSystem;
    private readonly Action<GameMessage> _sendNetworkMessage;
    private TaskCompletionSource<List<int>>? _selectionTaskSource;

    public CardSystem(ActionSystem actionSystem, IdSystem idSystem, Action<GameMessage> sendNetworkMessage)
    {
        _actionSystem = actionSystem;
        _idSystem = idSystem;
        _sendNetworkMessage = sendNetworkMessage;

        _actionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        _actionSystem.AttachPerformer<MoveFromGA>(MoveFromPerformer);
        _actionSystem.AttachPerformer<MoveToGA>(MoveToPerformer);
        _actionSystem.AttachPerformer<DrawSingleCardGA>(DrawSingleCardPerformer);
        _actionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        _actionSystem.AttachPerformer<DiscardSingleCardGA>(DiscardSingleCardPerformer);
        _actionSystem.AttachPerformer<DiscardCardsGA>(DiscardCardsPerformer);

        _actionSystem.AttachPerformer<PerformEffectGA>(PerformEffectPerformer);
        _actionSystem.AttachPerformer<GetTargetGA>(GetTargetPerformer);
    }

    public void Setup()
    {
        for (int i = 0; i < 10; i++)
        {
            CreateCard(new VIPCard(), PlayerSide.Player1, CardZone.DrawPile);
            CreateCard(new FoodWaste(), PlayerSide.Player1, CardZone.DrawPile);
            CreateCard(new VIPCard(), PlayerSide.Player2, CardZone.DrawPile);
            CreateCard(new FoodWaste(), PlayerSide.Player2, CardZone.DrawPile);
        }
    }

    private void CreateCard(CardData cardData, PlayerSide player, CardZone zone)
    {
        Card card = new Card(cardData, _idSystem.GetNewCardId());
        CardIdToCard.Add(card.Id, card);
        zones[player][zone].Add(card);

        _sendNetworkMessage?.Invoke(new GameMessage
        {
            Op = OpCode.S2C_CreateCard,
            Payload = JsonConvert.SerializeObject(new CreateCardMsg
            {
                CardId = card.Id,
                DataId = cardData.Id,
                Zone = zone
            })
        });
    }

    private Task MoveFromPerformer(MoveFromGA moveFromGA)
    {
        if (moveFromGA.CardId == 0) return Task.CompletedTask;
        List<Card> cards = zones[moveFromGA.PlayerSide][moveFromGA.CardZone];
        Card card = CardIdToCard[moveFromGA.CardId];
        cards.Remove(card);

        _sendNetworkMessage?.Invoke(new GameMessage
        {
            Op = OpCode.S2C_MoveCardFrom,
            Payload = JsonConvert.SerializeObject(new MoveCardToMsg
            {
                CardId = moveFromGA.CardId,
                Player = moveFromGA.PlayerSide,
                To = moveFromGA.CardZone
            })
        });

        return Task.CompletedTask;
    }

    private Task MoveToPerformer(MoveToGA moveToGA)
    {
        if (moveToGA.CardId == 0) return Task.CompletedTask;
        List<Card> cards = zones[moveToGA.PlayerSide][moveToGA.CardZone];
        Card card = CardIdToCard[moveToGA.CardId];
        cards.Add(card);

        _sendNetworkMessage?.Invoke(new GameMessage
        {
            Op = OpCode.S2C_MoveCardTo,
            Payload = JsonConvert.SerializeObject(new MoveCardToMsg
            {
                CardId = moveToGA.CardId,
                Player = moveToGA.PlayerSide,
                To = moveToGA.CardZone
            })
        });

        return Task.CompletedTask;
    }

    private Task DrawSingleCardPerformer(DrawSingleCardGA drawSingleCardGA)
    {
        Card card = zones[drawSingleCardGA.Player][CardZone.DrawPile].RandomSelect();
        MoveFromGA moveFromGA = new(card.Id, drawSingleCardGA.Player, CardZone.DrawPile);
        MoveToGA moveToGA = new(card.Id, drawSingleCardGA.Player, CardZone.Hand);
        _actionSystem.AddReaction(moveFromGA);
        _actionSystem.AddReaction(moveToGA);
        return Task.CompletedTask; 
    }

    private Task DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        for (int i = 0; i < drawCardsGA.drawAmount; i++)
        {
            _actionSystem.AddReaction(new DrawSingleCardGA(drawCardsGA.Player));
        }
        return Task.CompletedTask;
    }

    private Task DiscardSingleCardPerformer(DiscardSingleCardGA discardSingleCardGA)
    {
        MoveFromGA moveFromGA = new(discardSingleCardGA.CardId, discardSingleCardGA.Player, CardZone.Hand);
        MoveToGA moveToGA = new(discardSingleCardGA.CardId, discardSingleCardGA.Player, CardZone.DiscardPile);
        _actionSystem.AddReaction(moveFromGA);
        _actionSystem.AddReaction(moveToGA);
        return Task.CompletedTask;
    }

    private Task DiscardCardsPerformer(DiscardCardsGA discardCardsGA)
    {
        if (discardCardsGA.CardIds == null) return Task.CompletedTask;
        foreach (var cardId in discardCardsGA.CardIds)
        {
            _actionSystem.AddReaction(new DiscardSingleCardGA(discardCardsGA.Player, cardId));
        }
        return Task.CompletedTask;
    }

    private Task PlayCardPerformer(PlayCardGA playCardGA)
    {
        if (playCardGA.CardId == null) return Task.CompletedTask;

        MoveFromGA moveFromGA = new(playCardGA.CardId, playCardGA.Player, CardZone.Hand);
        _actionSystem.AddReaction(moveFromGA);
        
        foreach (var effect in CardIdToCard[playCardGA.CardId].Effects)
        {
            PerformEffectGA performEffectGA = new(playCardGA.Player, effect);
            _actionSystem.AddReaction(performEffectGA);
        }

        MoveToGA moveToGA = new(playCardGA.CardId, playCardGA.Player, CardZone.DiscardPile);
        _actionSystem.AddReaction(moveToGA);
        return Task.CompletedTask;
    }

    private Task PerformEffectPerformer(PerformEffectGA performEffectGA)
    {
        List<GameAction> effectActions = performEffectGA.Effect.GetGameAction(performEffectGA.Player, zones);
        
        foreach (var effectAction in effectActions)
        {
            _actionSystem.AddReaction(effectAction);
        }
        return Task.CompletedTask;
    }

    private async Task GetTargetPerformer(GetTargetGA getTargetGA)
    {
        int amountToSelect = Math.Min(getTargetGA.Amount, getTargetGA.Sourse.Count);

        _selectionTaskSource = new TaskCompletionSource<List<int>>();

        _sendNetworkMessage?.Invoke(new GameMessage
        {
            Op = OpCode.S2C_SelectRequest,
            Payload = JsonConvert.SerializeObject(new SelectRequestMsg
            {
                RequestId = 1,
                Player = getTargetGA.Player,
                CandidateIds = getTargetGA.Sourse,
                Count = amountToSelect
            })
        });

        List<int> resultCards = await _selectionTaskSource.Task;

        getTargetGA.Selected = new List<int>(resultCards);
        getTargetGA.CardIdsInterface.CardIds = getTargetGA.Selected;

        _selectionTaskSource = null;
    }

    public void OnReceiveClientSelection(List<int> selectedCards)
    {
        if (_selectionTaskSource != null)
        {
            _selectionTaskSource.TrySetResult(selectedCards);
        }
    }
}