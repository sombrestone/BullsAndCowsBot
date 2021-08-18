using System;
using System.Collections.Generic;
using System.Text;

namespace BullsAndCowsGame
{
    public class Turn
    {
        public Turn() { }
        public TurnStatus Status { get; set; }
        public string Number { get; set; }
        public Player Player { get; set; }
        public int Bulls { get; set; }
        public int Cows { get; set; }
        public int TurnNum { get; set; }
    }

    public enum TurnStatus
    {
        Success, 
        FormatEror,
        NotYourTurn,
        GameEnd
    }
}
