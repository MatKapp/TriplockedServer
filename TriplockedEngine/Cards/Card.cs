using System;
using System.Collections.Generic;
using System.Text;
using TriplockedEngine.Model;

namespace TriplockedEngine.Cards
{
    class Card
    {
        public string Name { get; set; }
        public int Lenght { get; set; }
        public Direction Direction { get; set; }
        public int Dmg { get; set; }
        public bool[,] DmgKernel { get; set; }
        public int Special { get; set; }
        public int SpecialArg { get; set; }
        public Card(string name, int len, Direction dir, int dmg, bool[,] ker , int spec = 0, int specArg = 0)
        {
            Name = name;
            Lenght = len;
            Direction = dir;
            Dmg = dmg;
            DmgKernel = ker;
            Special = spec;
            SpecialArg = specArg;
        }
    }
}
