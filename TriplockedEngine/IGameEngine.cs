using System;
using System.Collections.Generic;
using System.Text;

namespace TriplockedEngine
{
    interface IGameEngine
    {
        string AddPlayer(string webSocketId, int roomId = 0);
        string RemovePlayer(string webSocketId);
        string AddMonitor(string webSockedId, int roomId = 0);
        string RemoveMonitro(string webSockedId);
        string Action(string webSocketId, string action);
        int GetRoomSize(int roomId = 0);
        int GetNumberOfPlayers(int roomId = 0);
    }
}
