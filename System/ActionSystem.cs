using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ActionSystem 
{
    private List<GameAction> reactions = null!;
    public bool IsPerforming { get; private set; } = false;
    
    private  Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private  Dictionary<Type, List<Action<GameAction>>> postSubs = new();
    private  Dictionary<Type, Func<GameAction, Task>> performers = new();

    private  Dictionary<Delegate, Action<GameAction>> delegateMap = new();

    public async void PerformAsync(GameAction action, Action OnPerformFinished = null!)
    {
        if (IsPerforming) return;
        IsPerforming = true;
        
        await FlowAsync(action);
        
        IsPerforming = false;
        OnPerformFinished?.Invoke();
    }

    public void AddReaction(GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    private async Task FlowAsync(GameAction action)
    {
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        await PerformReactionsAsync();

        reactions = action.PerformReactions;
        await PerformPerformerAsync(action);
        await PerformReactionsAsync();

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        await PerformReactionsAsync();
    }

    private async Task PerformPerformerAsync(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            await performers[type](action);
        }
    }

    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (var sub in subs[type])
            {
                sub(action);
            }
        }
    }

    private async Task PerformReactionsAsync()
    {
        if (reactions == null) return;

        foreach (var reaction in reactions)
        {
            await FlowAsync(reaction);
        }
    }

    public void AttachPerformer<T>(Func<T, Task> performer) where T : GameAction
    {
        Type type = typeof(T);
        Task wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type)) performers[type] = wrappedPerformer;
        else performers.Add(type, wrappedPerformer);
    }

    public void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type)) performers.Remove(type);
    }

    public void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        
        Action<GameAction> wrappedReaction = (GameAction action) => reaction((T)action);
        delegateMap[reaction] = wrappedReaction;

        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        }
        else
        {
            subs.Add(typeof(T), new List<Action<GameAction>>());
            subs[typeof(T)].Add(wrappedReaction);
        }
    }

    public void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        
        if (subs.ContainsKey(typeof(T)))
        {
            if (delegateMap.TryGetValue(reaction, out var wrappedReaction))
            {
                subs[typeof(T)].Remove(wrappedReaction);
                delegateMap.Remove(reaction);
            }
        }
    }
}