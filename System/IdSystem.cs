using System;
using System.Data;

public class IdSystem
{
    private int _cardIdCount;
    private int _requestIdCount;

    public IdSystem()
    {
        _cardIdCount = 0;
        _requestIdCount = 0;
    }

    public int GetNewCardId()
    {
        _cardIdCount++;
        return _cardIdCount;
    }

    public int GetNewRequestId()
    {
        _requestIdCount++;
        return _requestIdCount;
    }
}