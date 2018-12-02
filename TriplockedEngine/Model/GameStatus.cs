using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Triplocked.TriplockedEngine.Model;
using WebSocketManager.Common;
using TriplockedEngine.Cards;
using System.Linq;

namespace TriplockedEngine.Model
{
    class GameStatus
    {
        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
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
        public int MaxHP { get; set; }
        public int PlayersResponseCounter { get; set; }
        public int[,] Grid;
        public Random Rand;

        public GameStatus(int id, int maxPlayers, int maxX, int maxY, int status)
        {
            GameId = id;
            CurrentPlayers = new List<Player>();
            CurrentMonitors = new List<Monitor>();
            MaxPlayers = maxPlayers;
            MaxX = maxX;
            MaxY = maxY;
            Status = status;
            MaxHP = 3;
            Rand = new Random();
            Grid = new int[MaxX, MaxY];
            CardsList = new Dictionary<int, Card>()
            {// id     name  mvm_len   mvm_dir      dmg        dmg_kernel  special   special_arg 
                {-1, new Card("Move 0",0,Direction.Up     ,0  ,new bool[1,1]{ {false} } )},
                {-2, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ {true,true,true}, { true, false, true }, { true, true, true } } )},

                {0,  new Card("Move 1",1,Direction.Up     ,0  ,new bool[1,1]{ {false} } )},
                {1,  new Card("Move 1",1,Direction.Down   ,0  ,new bool[1,1]{ {false} } )},
                {2,  new Card("Move 1",1,Direction.Left   ,0  ,new bool[1,1]{ {false} } )},
                {3,  new Card("Move 1",1,Direction.Right  ,0  ,new bool[1,1]{ {false} } )},

                {10, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { true, true, true}, { false, false, false }, { false, false, false } } )},
                {11, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { false, false, false }, { false, false, false }, { true, true, true } } )},
                {12, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { true, false, false}, { true, false, false }, { true, false, false } } )},
                {13, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { false, false, true}, { false, false, true}, { false, false, true } } )},

                {20, new Card("Attack 2",0,Direction.Right  ,2  ,new bool[3,3]{ { false, true, false }, { false, false, false }, { false, false, false } } )},
                {21, new Card("Attack 2",0,Direction.Right  ,2  ,new bool[3,3]{ { false, false, false }, { false, false, false }, { false, true, false } } )},
                {22, new Card("Attack 2",0,Direction.Right  ,2  ,new bool[3,3]{ { false, false, false }, { true, false, false }, { false, false, false } } )},
                {23, new Card("Attack 2",0,Direction.Right  ,2  ,new bool[3,3]{ { false, false, false }, { false, false, true}, { false, false, false } } )},
                
                {30, new Card("Move 1, Attack 1",1,Direction.Up  ,1  ,new bool[3,3]{ { true, true, true}, { false, false, false }, { false, false, false } } )},
                {31, new Card("Move 1, Attack 1",1,Direction.Down  ,1  ,new bool[3,3]{ { false, false, false }, { false, false, false }, { true, true, true } } )},
                {32, new Card("Move 1, Attack 1",1,Direction.Left  ,1  ,new bool[3,3]{ { true, false, false}, { true, false, false }, { true, false, false } } )},
                {33, new Card("Move 1, Attack 1",1,Direction.Right  ,1  ,new bool[3,3]{ { false, false, true}, { false, false, true}, { false, false, true } } )},
                
                {40,  new Card("Heal 1",0,Direction.Up     ,0  ,new bool[1,1]{ {false} } ,1,1)},
                {41,  new Card("Heal 1",0,Direction.Up     ,0  ,new bool[1,1]{ {false} } ,1,1)},
                {42,  new Card("Heal 1",0,Direction.Up     ,0  ,new bool[1,1]{ {false} } ,1,1)},
                {43,  new Card("Heal 1",0,Direction.Up     ,0  ,new bool[1,1]{ {false} } ,1,1)},

                {50,  new Card("Attack 1",0,Direction.Up     ,1  ,new bool[5,5]{ { false, false, true, false, false }, { false, false, true, false, false }, { false, false, false, false, false }, { false, false, true, false, false }, { false, false, true, false, false } } ,0,0)},
                {51,  new Card("Attack 1",0,Direction.Up     ,1  ,new bool[5,5]{ { false, false, true, false, false }, { false, false, true, false, false }, { false, false, false, false, false }, { false, false, true, false, false }, { false, false, true, false, false } } ,0,0)},
                {52,  new Card("Attack 1",0,Direction.Up     ,1  ,new bool[5,5]{ { false, false, false, false, false }, { false, false, false, false, false }, { true, true, false, true, true }, { false, false, false, false, false }, { false, false, false, false, false } } ,0,0)},
                {53,  new Card("Attack 1",0,Direction.Up     ,1  ,new bool[5,5]{ { false, false, false, false, false }, { false, false, false, false, false }, { true, true, false, true, true }, { false, false, false, false, false }, { false, false, false, false, false } } ,0,0)},

                {60, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { true, false, true}, { false, false, false}, { true, false, true } } )},
                {61, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { true, false, true}, { false, false, false}, { true, false, true } } )},
                {62, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { true, false, true}, { false, false, false}, { true, false, true } } )},
                {63, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { true, false, true}, { false, false, false}, { true, false, true } } )},
                
                {70, new Card("Move 3",3,Direction.Up  ,0  ,new bool[1,1]{ { false} } )},
                {71, new Card("Move 3",3,Direction.Down  ,0  ,new bool[1,1]{ { false} } )},
                {72, new Card("Move 3",3,Direction.Left  ,0  ,new bool[1,1]{ { false} } )},
                {73, new Card("Move 3",3,Direction.Right  ,0  ,new bool[1,1]{ { false} } )},

                {80, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { false, true, false}, { true, false, true}, { false, true, false } } )},
                {81, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { false, true, false}, { true, false, true}, { false, true, false } } )},
                {82, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { false, true, false}, { true, false, true}, { false, true, false } } )},
                {83, new Card("Attack 1",0,Direction.Right  ,1  ,new bool[3,3]{ { false, true, false}, { true, false, true}, { false, true, false } } )},

            };
            PlayersResponseCounter = 0;
        }
        public string AddPlayer(string id)
        {
            string result;

            if (CurrentPlayers.Count < MaxPlayers && !CurrentPlayers.Exists(player => player.PlayerId.Equals(id)))
            {
                Player newPlayer = new Player(CurrentPlayers.Count, id, (CurrentPlayers.Count % 2) * 4, 4 - (CurrentPlayers.Count % 2)*4,Rand);
                CurrentPlayers.Add(newPlayer);
                Player p;
                switch (CurrentPlayers.Count)
                {
                    case 3:
                        MaxX = 5;
                        MaxY = 4;
                        Grid = new int[MaxX, MaxY];
                        p = CurrentPlayers.First(d => d.PlayerNumber == 0);
                        p.X = 0;
                        p.Y = 0;
                        p = CurrentPlayers.First(d => d.PlayerNumber == 1);
                        p.X = 2;
                        p.Y = 3;
                        p = CurrentPlayers.First(d => d.PlayerNumber == 2);
                        p.X = 4;
                        p.Y = 0;
                        break;
                    case 4:
                        MaxX = 5;
                        MaxY = 5;
                        Grid = new int[MaxX, MaxY];
                        p = CurrentPlayers.First(d => d.PlayerNumber == 0);
                        p.X = 0;
                        p.Y = 0;
                        p = CurrentPlayers.First(d => d.PlayerNumber == 1);
                        p.X = 0;
                        p.Y = 4;
                        p = CurrentPlayers.First(d => d.PlayerNumber == 2);
                        p.X = 4;
                        p.Y = 0;
                        p = CurrentPlayers.First(d => d.PlayerNumber == 3);
                        p.X = 4;
                        p.Y = 4;
                        break;
                }
                result = "User Added";
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

            if (!String.IsNullOrEmpty(id) && CurrentPlayers.RemoveAll(p => p.PlayerId.Equals(id)) == 1)
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
                    player.ActionRecorded = true;
                    result = $"Player action added, {CurrentPlayers.Count}/{MaxPlayers}";

                    if (PlayersResponseCounter == CurrentPlayers.Count)
                    {
                        result = MakeMove();
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
        private string MakeMove()
        {
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append("{\"data\": [");

            foreach (var player in CurrentPlayers)
            {
                player.DrawCards();
                player.ActionRecorded = false;
                player.Animation = AnimationStatus.Idle;
            }

            for (int i = 0; i < 3; i++)
            {
                ResolveMoves(i);
                GenerateGrid();
                ResolveAttack(i);
                ResolveSpecial(i);
                
                resultBuilder.Append(printGameState());

                //if (i != 2)
                resultBuilder.Append(',');
                

            }
            foreach (var player in CurrentPlayers)
            {
                player.Animation = AnimationStatus.Idle;
            }
            resultBuilder.Append(printGameState());

            resultBuilder.Append("]}");
            PlayersResponseCounter = 0;

            return resultBuilder.ToString();
        }

        private void GenerateGrid()
        {
            for (int i = 0; i < MaxX; i++)
            {
                for (int j = 0; j < MaxY; j++)
                {
                    Grid[i, j] = -1;
                }
            }
            foreach (var player in CurrentPlayers)
            {
                if (player.Animation != AnimationStatus.Death)
                {
                    Grid[player.X, player.Y] = player.PlayerNumber;
                }
            }
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
          
            foreach (var player in CurrentPlayers)      //TODO check if does it work
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
                    player.Animation = AnimationStatus.Attack;
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
                    foreach (var pos in affectedPositions)
                    {
                        if(Grid[pos.Item1,pos.Item2] == -1)
                        {
                            Grid[pos.Item1, pos.Item2] = -2;
                        }
                    }
                    if (affectedPositions.Contains(position.Value))
                    {
                        Player currentPlayer = CurrentPlayers.First(d => d.PlayerId == position.Key);
                        currentPlayer.HP -= card.Dmg;
                        if (currentPlayer.HP <= 0)
                        {
                            currentPlayer.Animation = AnimationStatus.Death;
                            Status = 0; //game ended (specjalna wiadomość?)

                        }
                        switch (currentPlayer.Animation)
                        {
                            case AnimationStatus.Idle:
                                currentPlayer.Animation = AnimationStatus.IdleHurt;
                                break;
                            case AnimationStatus.Move:
                                currentPlayer.Animation = AnimationStatus.MoveHurt;
                                break;
                            case AnimationStatus.Attack:
                                currentPlayer.Animation = AnimationStatus.AttackHurt;
                                break;
                            case AnimationStatus.Colide:
                                currentPlayer.Animation = AnimationStatus.ColideHurt;
                                break;
                        }
                    }
                }
            }
        }
        private void ResolveSpecial(int cardNumber)
        {
            foreach (var player in CurrentPlayers)
            {
                var card = CardsList[player.ActionList[cardNumber]];
                if (card.Special != 0)
                {
                    switch (player.Animation)
                    {
                        case AnimationStatus.Idle:
                            player.Animation = AnimationStatus.Special;
                            break;
                        case AnimationStatus.Move:
                            player.Animation = AnimationStatus.Special;
                            break;
                        case AnimationStatus.IdleHurt:
                            player.Animation = AnimationStatus.SpecialHurt;
                            break;
                        case AnimationStatus.MoveHurt:
                            player.Animation = AnimationStatus.SpecialHurt;
                            break;
                        case AnimationStatus.Colide:
                            player.Animation = AnimationStatus.Special;
                            break;
                        case AnimationStatus.ColideHurt:
                            player.Animation = AnimationStatus.SpecialHurt;
                            break;

                    }
                    switch (card.Special)
                    {
                        case 1: //heal
                            player.HP = Math.Min(player.HP + card.SpecialArg, MaxHP);
                            break;
                    }
                }
            }
        }
    }
}
