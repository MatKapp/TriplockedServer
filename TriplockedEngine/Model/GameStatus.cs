using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Triplocked.TriplockedEngine.Model;

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
        public int PlayersResponseCounter { get; set; }

        public GameStatus(int id, int maxPlayers, int maxX, int maxY, int status)
        {
            GameId = id;
            CurrentPlayers = new List<Player>();
            MaxPlayers = maxPlayers;
            MaxX = maxX;
            MaxY = maxY;
            Status = status;
            PlayersResponseCounter = 0;
        }
        public string AddPlayer(string id)
        {
            string result;

            if (CurrentPlayers.Count >= MaxPlayers)
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
            Player playerToRemove = CurrentPlayers.FirstOrDefault(player => player.PlayerId.Equals(id));
            string result;

            if (playerToRemove != null)
            {
                CurrentPlayers.Remove(playerToRemove);
                result = "Player removed";
            }
            else
            {
                result = "Player to remove not found";
            }

            return result;
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
