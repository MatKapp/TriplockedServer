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
        public int ActionCount { get; set; }

        public GameStatus(int id, int maxPlayers, int maxX, int maxY, int status)
        {
            GameId = id;
            CurrentPlayers = new List<Player>();
            CurrentMonitors = new List<Monitor>();
            MaxPlayers = maxPlayers;
            MaxX = maxX;
            MaxY = maxY;
            Status = status;
            ActionCount = 0;
        }
        public int AddPlayer(string id)
        {
            if (CurrentPlayers.Count < MaxPlayers)
            {
                Player newPlayer = new Player(id, CurrentPlayers.Count * 3, 1 + CurrentPlayers.Count);
                return 0;
            } else
            {
                return 1;
            }            
        }
        public int RemovePlayer(string id)
        {
            if (CurrentPlayers.RemoveAll(p => p.PlayerId.Equals(id)) == 1)
            {
                if (CurrentPlayers.Count < 2)
                {
                    Status = 0;
                }                
                return 0;
            }
            return 1;
        }
        public int AddMonitor(string id)
        {
            Monitor newMonitor = new Monitor(id);
            CurrentMonitors.Add(newMonitor);
            return 0;
        }
        public int RemoveMonitor(string id)
        {
            if (CurrentMonitors.RemoveAll(m => m.MonitorId.Equals(id)) == 1)
            {
                return 0;
            }            
            return 1;
        }
        public int AddAction(string PlayerId, List<ActionMessage> Actions)
        {
            Player player = CurrentPlayers.Where(p => p.PlayerId.Equals(PlayerId)).FirstOrDefault();
            if (player != null)
            {
                player.ActionList = Actions;
                ActionCount++;
                if (ActionCount == MaxPlayers)
                {
                    MakeMove();
                }
                return 0;
            } else
            {
                return 1;
            }
        }
        private void MakeMove()
        {
            ActionCount = 0;

        }
    }
}
