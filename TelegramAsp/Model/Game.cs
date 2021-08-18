using BullsAndCowsGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullsAndCowsGame
{
    public class Game
    {
        public Game() { }
        public Player CurrentPlayer { get; set; }
        public Player NextTurnPlayer { get; set; }
        public List<Turn> Turns { get; set; }
    }
}
