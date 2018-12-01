using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TriplockedEngine.Model
{
    class GameStatus
    {
        public int GameId { get; set; }
        public List<Player> CurrentPlayers { get; set; }
        public List<Monitor> CurrentMonitors { get; set; }
        public int MaxPlayers { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int Status { get; set; }
        public int PlayersResponseCounter { get; set; }

        public GameStatus(int id, int maxPlayers, int maxX, int maxY, int status)
        {
            GameId = id;
            CurrentPlayers = new List<Player>();
            CurrentMonitors = new List<Monitor>();
            MaxPlayers = maxPlayers;
            MaxX = maxX;
            MaxY = maxY;
            Status = status;
            PlayersResponseCounter = 0;
        }
        public string AddPlayer(string id)
        {
            string result;

            if (CurrentPlayers.Count < MaxPlayers)
            {
                Player newPlayer = new Player(id, CurrentPlayers.Count * 3, 1 + CurrentPlayers.Count);
                CurrentPlayers.Add(newPlayer);
                result = "Added";
            }
            else
            {
                result = "Too many players, sorry";
            }
            return result;
        }
        public string RemovePlayer(string id)
        {
            string result;

            if (CurrentPlayers.RemoveAll(p => p.PlayerId.Equals(id)) == 1)
            {
                result = "Player removed";
            }
            else
            {
                result = "Player to remove not found";
            }

            return result;
        }
        public string AddMonitor(string id)
        {
            Monitor newMonitor = new Monitor(id);
            CurrentMonitors.Add(newMonitor);
            return "Monitor added";
        }
        public string RemoveMonitor(string id)
        {
            if (CurrentMonitors.RemoveAll(m => m.MonitorId.Equals(id)) == 1)
            {
                return "Monitor removed";
            }            
            return "Monitor to remove not found";
        }
        public string AddAction(string playerId, List<ActionMessage> actions)
        {
            Player player = CurrentPlayers.FirstOrDefault(p => p.PlayerId.Equals(playerId));

            if (player != null)
            {
                player.ActionList = actions;
                PlayersResponseCounter++;

                if (PlayersResponseCounter == MaxPlayers)
                {
                    MakeMove();
                }
                return "Player action added";
            } else
            {
                return "Player action add failed, player not found";
            }
        }
        private void MakeMove()
        {
            PlayersResponseCounter = 0;

        }
    }
}
