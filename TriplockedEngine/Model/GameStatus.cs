using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TriplockedEngine.Cards;

namespace TriplockedEngine.Model
{
    class GameStatus
    {
        public int GameId { get; set; }
        public List<Player> CurrentPlayers { get; set; }
    
        public Dictionary<string,Card> CardsList { get; set; }
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
            CardsList = new Dictionary<string, Card>()
            {
                {"idle", new Card(0,Direction.Up) },
                {"move_up", new Card(2,Direction.Up) },
                {"move_down", new Card(2,Direction.Down) },
                {"move_left", new Card(2,Direction.Left) },
                {"move_right", new Card(2,Direction.Right) },
            };
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
            for(int i = 0; i < 3; i++)
            {
                ResolveMoves(i);
                //players special
                //players attack
            }
            ActionCount = 0;

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
