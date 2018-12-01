using System;
using System.Collections.Generic;
using System.Text;
using TriplockedEngine.Model;

namespace TriplockedEngine.Cards
{
    class Card
    {
        public int Lenght { get; set; }
        public Direction Direction { get; set; }
        public Card(int len, Direction dir)
        {
            Lenght = len;
            Direction = dir;
        }
    }
}
