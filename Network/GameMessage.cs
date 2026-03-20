using System;
using System.Collections.Generic;

public class GameMessage {
    public OpCode Op { get; set; }
    public string Payload { get; set; } = null!;
}

public enum OpCode
{
    S2C_GameStart = 100,
    S2C_CreateCard = 101,
    S2C_MoveCardFrom = 102,
    S2C_MoveCardTo = 103,
    S2C_SelectRequest = 104,
    C2S_PlayCard = 201,
    C2S_SelectResult = 202
}

public class GameStartMsg {
    public PlayerSide Player { get; set; } 
}

public class CreateCardMsg {
    public int DataId { get; set; }
    public int CardId { get; set; }
    public CardZone Zone { get; set; }
}

public class MoveCardFromMsg {
    public int CardId { get; set; }
    public PlayerSide Player { get; set; }
    public CardZone From { get; set; }
}

public class MoveCardToMsg {
    public int CardId { get; set; }
    public PlayerSide Player { get; set; }
    public CardZone To { get; set; }
}

public class SelectRequestMsg {
    public int RequestId { get; set; }
    public PlayerSide Player { get; set; }
    public List<int> CandidateIds { get; set; } = null!;
    public int Count { get; set; }
}

public class PlayCardMsg {
    public int CardId { get; set; }
}

public class SelectResultMsg {
    public int RequestId { get; set; } 
    public List<int> SelectedIds { get; set; } = null!;
}   