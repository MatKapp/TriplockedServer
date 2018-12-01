using System;
using System.Collections.Generic;
using System.Text;
using Triplocked.TriplockedEngine.Model;

namespace TriplockedEngine.Model
{
    class Player
    {
        public string PlayerId { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public AnimationStatus Animation { get; set; }
        public List<int> ActionList { get; set; }
        public List<int> CurrentHand { get; set; }
        public bool ActionRecorded { get; set; }

        public Player(string id, int x, int y)
        {
            PlayerId = id;
            HP = 3;
            Attack = 1;
            X = x;
            Y = y;
            DrawCards();
        }
        public void DrawCards()
        {
            CurrentHand = new List<int> { 1, 2, 3, 4, 5 };
        }
    }
}
