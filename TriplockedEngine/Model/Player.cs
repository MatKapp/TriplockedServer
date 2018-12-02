using System;
using System.Collections.Generic;
using System.Text;
using Triplocked.TriplockedEngine.Model;

namespace TriplockedEngine.Model
{
    class Player
    {
        public int PlayerNumber { get; set; }
        public string PlayerId { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public AnimationStatus Animation { get; set; }
        public List<int> ActionList { get; set; }
        public List<int> CurrentHand { get; set; }
        public bool ActionRecorded { get; set; }
        private Random Rand;

        public Player(int number, string id, int x, int y,Random rand)
        {
            PlayerNumber = number;
            PlayerId = id;
            HP = 3;
            Attack = 1;
            X = x;
            Y = y;
            Rand = rand;
            ActionRecorded = false;
            DrawCards();
        }
        public void DrawCards()
        {
            
            CurrentHand = new List<int> { 0, 10};
            int i = 3;
            while(i>0)
            {
                int cardId = 10 * Rand.Next(2,9);
                if (! CurrentHand.Contains(cardId))
                {
                    i--;
                    CurrentHand.Add(cardId);
                }
            }
        }
    }
}
