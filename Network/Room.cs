using Fleck;
using Newtonsoft.Json;

public class Room
{
    public IWebSocketConnection Player1 { get; set; }
    public IWebSocketConnection Player2 { get; set; }

    private Dictionary<IWebSocketConnection, PlayerSide> SocketToSide = new();

    public ActionSystem ActionSystem { get; private set; }
    public IdSystem IdSystem { get; private set; }
    public CardSystem CardSystem { get; private set; }

    private readonly Action<Room> _onRoomClosed;

    public Room(IWebSocketConnection p1, IWebSocketConnection p2, Action<Room> onRoomClosed)
    {
        Player1 = p1;
        Player2 = p2;
        SocketToSide[p1] = PlayerSide.Player1;
        SocketToSide[p2] = PlayerSide.Player2;
        _onRoomClosed = onRoomClosed;
        ActionSystem = new ActionSystem();
        IdSystem = new IdSystem();
        CardSystem = new CardSystem(ActionSystem, IdSystem, SendMessage);
        CardSystem.Setup();
    }

    public void SendMessage(GameMessage msg)
    {
        Player1.Send(JsonConvert.SerializeObject(msg));
        Player2.Send(JsonConvert.SerializeObject(msg));
    }

    public void HandleMessage(IWebSocketConnection socket, GameMessage msg)
    {
        switch (msg.Op)
        {
            case OpCode.C2S_SelectResult:
                {
                    var result = JsonConvert.DeserializeObject<SelectResultMsg>(msg.Payload);
                    if(result == null)
                    {
                        Console.WriteLine("SelectResultMsg is null");
                        _onRoomClosed(this);
                        break;
                    }
                    CardSystem.OnReceiveClientSelection(result.SelectedIds);
                    break;        
                }

            case OpCode.C2S_PlayCard:
                {
                    var result = JsonConvert.DeserializeObject<PlayCardMsg>(msg.Payload);
                    if(result == null)
                    {
                        Console.WriteLine("PlayCardMsg is null");
                        _onRoomClosed(this);
                        break;
                    }
                    ActionSystem.PerformAsync(new PlayCardGA(SocketToSide[socket], result.CardId));
                    break;                    
                }
   
        }
    }
}