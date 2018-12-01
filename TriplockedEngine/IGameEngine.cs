using System;
using System.Collections.Generic;
using System.Text;

namespace TriplockedEngine
{
    interface IGameEngine
    {
        string AddPlayer(string webSocketId, int roomId = 0);
        string RemovePlayer(string webSocketId);
        string Action(string webSocketId, string action);
    }
}
