using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Triplocked.TriplockedEngine.Model;
using WebSocketManager.Common;
using TriplockedEngine.Cards;

namespace TriplockedEngine.Model
{
    class GameStatus
    {
        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            SerializationBinder = new JsonBinderWithoutAssembly()
        };

        public int GameId { get; set; }
        public List<Player> CurrentPlayers { get; set; }
        public List<Monitor> CurrentMonitors { get; set; }
    
        public Dictionary<string,Card> CardsList { get; set; }
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
            ActionCount = 0;
            CardsList = new Dictionary<string, Card>()
            {
                {"idle", new Card(0,Direction.Up) },
                {"move_up", new Card(2,Direction.Up) },
                {"move_down", new Card(2,Direction.Down) },
                {"move_left", new Card(2,Direction.Left) },
                {"move_right", new Card(2,Direction.Right) },
            };
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
            string result;

            if (player != null)
            {
                player.ActionList = actions;
                PlayersResponseCounter++;
                result = $"Player action added, {CurrentPlayers.Count}/{MaxPlayers}";

                if (PlayersResponseCounter == MaxPlayers)
                {
                    MakeMove();
                    result = printGameState();
                }
            }
            else
            {
                result = "Player action add failed, player not found";
            }

            return result;
        }
        private void MakeMove()
        {
            for(int i = 0; i < 3; i++)
            {
                ResolveMoves(i);
                //players special
                //players attack
            }
            ActionCount = 0;
            PlayersResponseCounter = 0;

        }

        private string printGameState()
        {
            return JsonConvert.SerializeObject(this, _jsonSerializerSettings);
        }

        private Tuple<int, int> MovementAction(Card card)
        {
            if (card.Lenght == 0)
            {
                return new Tuple<int, int>(0, 0);
            }
            else
            {
                switch (card.Direction)
                {
                    case Direction.Up:
                        return new Tuple<int, int>(0, -1 * card.Lenght);
                    case Direction.Down:
                        return new Tuple<int, int>(0, card.Lenght);
                    case Direction.Left:
                        return new Tuple<int, int>(-1 * card.Lenght, 0);
                    case Direction.Right:
                        return new Tuple<int, int>(card.Lenght, 0);
                    default:
                        return new Tuple<int, int>(0, 0);
                }
            }

        }

        private void ResolveMoves(int cardNumber)
        {
            Dictionary<string, Tuple<int, int>> playersPositions = new Dictionary<string, Tuple<int, int>>();
            Dictionary<string, Tuple<int, int>> playersMovements = new Dictionary<string, Tuple<int, int>>();
            foreach (var player in CurrentPlayers)
            {
                var card = CardsList[player.ActionList[cardNumber].ActionId];
                if (card.Lenght != 0)
                {
                    playersMovements[player.PlayerId] = MovementAction(card);
                }
            }
            bool finished = false;
            while (!finished)
            {
                finished = true;
                foreach (var player in CurrentPlayers)
                {
                    playersPositions[player.PlayerId] = new Tuple<int, int>(player.X, player.Y);
                }
                foreach (var movement in playersMovements)
                {
                    var playerId = movement.Key;
                    playersPositions[playerId] = new Tuple<int, int>(
                        playersPositions[playerId].Item1 + playersMovements[playerId].Item1,
                        playersPositions[playerId].Item2 + playersMovements[playerId].Item2);
                }
                //var duplicateValues = playersPositions.GroupBy(x => x.Value).Where(x => x.Count() > 1);
                //if(duplicateValues.Count() != 0)
                foreach (var position in playersPositions)
                {
                    foreach (var otherPosition in playersPositions)
                    {
                        if (position.Key != otherPosition.Key && position.Value == otherPosition.Value)
                        {
                            finished = false;
                            if (playersMovements.ContainsKey(position.Key))
                            {
                                playersMovements.Remove(position.Key);
                            }
                        }
                    }
                }
            }
            foreach (var player in CurrentPlayers)
            {
                player.X = playersPositions[player.PlayerId].Item1;
                player.Y = playersPositions[player.PlayerId].Item2;
            }
        }
    }
}
