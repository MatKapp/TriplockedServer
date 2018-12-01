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
        public int Dmg { get; set; }
        public bool[,] DmgKernel { get; set; }
        public Card(int len, Direction dir, int dmg, bool[,] ker)
        {
            Lenght = len;
            Direction = dir;
            Dmg = dmg;
            DmgKernel = ker;
        }
    }
}
