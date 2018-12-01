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
    
        public Dictionary<int, Card> CardsList { get; set; }
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
            CardsList = new Dictionary<int, Card>()
            {// id       mvm_len   mvm_dir      dmg        dmg_kernel  
                {0, new Card(0,Direction.Up     ,0  ,new bool[1,1]{ {false} } )},
                {1, new Card(1,Direction.Up     ,0  ,new bool[1,1]{ {false} } )},
                {2, new Card(1,Direction.Down   ,0  ,new bool[1,1]{ {false} } )},
                {3, new Card(1,Direction.Left   ,0  ,new bool[1,1]{ {false} } )},
                {4, new Card(1,Direction.Right  ,0  ,new bool[1,1]{ {false} } )},
                {5, new Card(0,Direction.Right  ,1  ,new bool[3,3]{ {true,true,true}, { true, true, true }, { true, true, true } } )},
                {11, new Card(0,Direction.Right  ,1  ,new bool[3,3]{ { true, true, true}, { false, false, false }, { false, false, false } } )},
                {12, new Card(0,Direction.Right  ,1  ,new bool[3,3]{ { false, false, true}, { false, false, true }, { false, false, true } } )},
                {13, new Card(0,Direction.Right  ,1  ,new bool[3,3]{ { false, false, false }, { false, false, false }, { true, true, true } } )},
                {14, new Card(0,Direction.Right  ,1  ,new bool[3,3]{ { true, false, false }, { true, false, false }, { true, false, false } } )},
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
        public string AddAction(string playerId, List<int> actions)
        {
            Player player = CurrentPlayers.FirstOrDefault(p => p.PlayerId.Equals(playerId));
            string result;

            if (player != null)
            {
                if (!player.ActionRecorded)
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
                    result = "Player action add failed, action already added";
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
            foreach (var player in CurrentPlayers)
            {
                player.DrawCards();
                player.ActionRecorded = false;
            }

            for (int i = 0; i < 3; i++)
            {
                ResolveMoves(i);
                ResolveAttack(i);
                //players special
                //players attack
            }

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
                var card = CardsList[player.ActionList[cardNumber]];
                if (card.Lenght != 0)
                {
                    playersMovements[player.PlayerId] = MovementAction(card);
                    player.Animation = AnimationStatus.Move;
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
                    var movX = playersPositions[playerId].Item1 + playersMovements[playerId].Item1;
                    if (movX >= MaxX) movX = MaxX - 1;
                    if (movX < 0) movX = 0;

                    var movY = playersPositions[playerId].Item2 + playersMovements[playerId].Item2;
                    if (movY >= MaxY) movY = MaxY - 1;
                    if (movY < 0) movY = 0;

                    playersPositions[playerId] = new Tuple<int, int>(movX,movY);
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
            //zobaczyc czy to dziala
            foreach (var player in CurrentPlayers)
            {
                player.X = playersPositions[player.PlayerId].Item1;
                player.Y = playersPositions[player.PlayerId].Item2;
                if(player.Animation == AnimationStatus.Move && ! playersMovements.ContainsKey(player.PlayerId))
                {
                    player.Animation = AnimationStatus.Colide;
                }
            }
        }
        private void ResolveAttack(int cardNumber)
        {
            //Dictionary<string, Tuple<int, List<Tuple<int,int>>>> playersAttacks = new Dictionary<string, Tuple<int, List<Tuple<int,int>>>>(); //dmg, kernel
            Dictionary<string, Tuple<int, int>> playersPositions = new Dictionary<string, Tuple<int, int>>();
            foreach (var player in CurrentPlayers)
            {
                playersPositions[player.PlayerId] = new Tuple<int, int>(player.X, player.Y);
                List<Tuple<int, int>> affectedPositions = new List<Tuple<int, int>>();
                var card = CardsList[player.ActionList[cardNumber]];
                if (card.Dmg != 0)
                {
                    int sizePerSide = card.DmgKernel.GetLength(0) / 2;
                    for (int y = 0; y < card.DmgKernel.GetLength(0); y++)
                    {
                        for (int x = 0; x < card.DmgKernel.GetLength(0) ; x++)
                        {
                            if (card.DmgKernel[y, x])
                            {
                                affectedPositions.Add(new Tuple<int, int>(player.X + x - sizePerSide, player.Y + y - sizePerSide));
                            }
                        }
                    }
                    //playersAttacks[player.PlayerId] = new Tuple<int, bool[,]>(card.Dmg, card.DmgKernel);
                    //player.Animation = AnimationStatus.Attack;
                }
                foreach (var position in playersPositions)
                {
                    if (affectedPositions.Contains(position.Value))
                    {
                        CurrentPlayers.First(d => d.PlayerId == position.Key).HP-=card.Dmg;
                        //dodać taking dmg by player
                    }
                }
            }
        }
    }
}
