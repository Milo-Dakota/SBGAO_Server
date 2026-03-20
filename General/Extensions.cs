using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class PlayerSideExtensions
{
    public static PlayerSide Opponent(this PlayerSide side)
    {
        if (side == PlayerSide.Player1) return PlayerSide.Player2;
        if (side == PlayerSide.Player2) return PlayerSide.Player1;
        return PlayerSide.None;
    }
}

public static class ListExtensions
{
    public static T? Draw<T>(this List<T> list)
    {
        if (list.Count == 0) return default;
        int index = new Random().Next(0, list.Count);
        T item = list[index];
        list.Remove(item);
        return item;
    }
    public static T? RandomSelect<T>(this List<T> list)
    {
        if (list.Count == 0) return default;
        int index = new Random().Next(0, list.Count);
        T item = list[index];
        return item;
    }
}



public static class TaskExtensions
{
    public static async void Forget(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}