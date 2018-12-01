using TriplockedEngine.Model;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TriplockedEngine
{
    public class TriplockedEngine : IGameEngine
    {
        private GameStatus gameStatus;

        public string AddPlayer(string webSocketId, int roomId = 0)
        {
            int result = gameStatus.AddPlayer(webSocketId);
            if (result == 0)
            {
                return "Added";
            }
            else
            {
                return "Error";
            }
        }

        public string RemovePlayer(string webSocketId)
        {
            int result = gameStatus.RemovePlayer(webSocketId);
            return "Removed";
        }

        public string Action(string webSocketId, string actionMessage)
        {
            var action = JsonConvert.DeserializeObject<List<ActionMessage>>(actionMessage);
            int result = gameStatus.AddAction(webSocketId, action);
            if (result == 0)
            {
                return "OK";
            } else
            {
                return "Error";
            }
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
