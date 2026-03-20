using System;
using System.Collections.Generic;
using Fleck;
using Newtonsoft.Json;

class Program
{
    static IWebSocketConnection? waitingPlayer = null;
    
    static Dictionary<IWebSocketConnection, Room> playerToRoom = new Dictionary<IWebSocketConnection, Room>();
    
    // 线程锁，防止多个玩家在同一毫秒连接时产生匹配冲突
    static readonly object matchLock = new object();

    static void Main(string[] args)
    {
        var server = new WebSocketServer("ws://0.0.0.0:30005");

        server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                HandlePlayerJoin(socket);
            };

            socket.OnClose = () =>
            {
                HandlePlayerLeave(socket);
            };

            socket.OnMessage = message =>
            {
                try 
                {
                    var msg = JsonConvert.DeserializeObject<GameMessage>(message);
                    
                    if (playerToRoom.TryGetValue(socket, out Room room))
                    {
                        room.HandleMessage(socket, msg);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("消息解析错误: " + ex.Message);
                }
            };
        });

        Console.ReadLine();
    }

    static void OnRoomClosed(Room room)
    {
        playerToRoom.Remove(room.Player1);
        playerToRoom.Remove(room.Player2);
    }

    static void HandlePlayerJoin(IWebSocketConnection socket)
    {
        lock (matchLock)
        {
            if (waitingPlayer == null)
            {
                waitingPlayer = socket;
            }
            else
            {
                var p1 = waitingPlayer;
                var p2 = socket;
                var room = new Room(p1, p2, OnRoomClosed);

                playerToRoom[p1] = room;
                playerToRoom[p2] = room;

                p1.Send(JsonConvert.SerializeObject
                (new GameMessage
                    { 
                        Op = OpCode.S2C_GameStart,
                        Payload = JsonConvert.SerializeObject(new GameStartMsg { Player = PlayerSide.Player1 })
                    }
                ));
                p2.Send(JsonConvert.SerializeObject
                (new GameMessage
                    { 
                        Op = OpCode.S2C_GameStart,
                        Payload = JsonConvert.SerializeObject(new GameStartMsg { Player = PlayerSide.Player2 })
                    }
                ));

                waitingPlayer = null; 
            }
        }
    }

    static void HandlePlayerLeave(IWebSocketConnection socket)
    {
        lock (matchLock)
        {
            if (waitingPlayer == socket)
            {
                waitingPlayer = null;
            }
            else if (playerToRoom.TryGetValue(socket, out Room room))
            {
                playerToRoom.Remove(room.Player1);
                playerToRoom.Remove(room.Player2);
            }
        }
    }
}