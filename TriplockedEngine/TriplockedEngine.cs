using TriplockedEngine.Model;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Triplocked.TriplockedEngine.Model;

namespace TriplockedEngine
{
    public class TriplockedEngine : IGameEngine
    {
        private GameStatus gameStatus;

        public string AddPlayer(string webSocketId, int roomId = 0)
        {
            return gameStatus.AddPlayer(webSocketId);
        }

        public string RemovePlayer(string webSocketId)
        {
            return gameStatus.RemovePlayer(webSocketId);
        }

        public string Action(string webSocketId, string actionMessage)
        {
            var action = JsonConvert.DeserializeObject<List<ActionMessage>>(actionMessage);
            return gameStatus.AddAction(webSocketId, action);
        }

        public string AddMonitor(string webSockedId, int roomId = 0)
        {
            throw new NotImplementedException();
        }

        public string RemoveMonitro(string webSockedId)
        {
            throw new NotImplementedException();
        }

        public int GetRoomSize(int roomId = 0)
        {
            return gameStatus.MaxPlayers;
        }

        public int GetNumberOfPlayers(int roomId = 0)
        {
            return gameStatus.CurrentPlayers.Count;
        }

        public TriplockedEngine()
        {
            gameStatus = new GameStatus(1,2,4,7,1);
        }
    }
}
