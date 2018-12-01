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
        public int MaxPlayers { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int Status { get; set; }
        public int ActionCount { get; set; }

        public GameStatus(int id, int maxPlayers, int maxX, int maxY, int status)
        {
            GameId = id;
            CurrentPlayers = new List<Player>();
            MaxPlayers = maxPlayers;
            MaxX = maxX;
            MaxY = maxY;
            Status = status;
            ActionCount = 0;
        }
        public int AddPlayer(string id)
        {
            if (CurrentPlayers.Count >= MaxPlayers)
            {
                return 1;
            }
            Player newPlayer = new Player(id, CurrentPlayers.Count * 3, 1 + CurrentPlayers.Count);
            return 0;
        }
        public int RemovePlayer(string id)
        {
            CurrentPlayers.RemoveAll(p => p.PlayerId.Equals(id));
            Status = 0;
            return 0;
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
