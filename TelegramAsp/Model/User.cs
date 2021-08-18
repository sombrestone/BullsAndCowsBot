using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullsAndCowsGame
{
    public class UserBase
    {
        public UserBase() { }
        public UserBase(long id,string username) 
        {
            Id = id;
            UserName=username;
        }
        public long Id { get; set; }
        public string UserName { get; set; }
    }

    public class User: UserBase
    {
        public User() { }
        public UserBase GetBase()
        {
            return new UserBase(Id, UserName);
        }
        public string[] Invites { get; set; }
        public string Game { get; set; }
        public int Wins { get; set; }
    }

    public class Player : UserBase
    {
        public Player() { }
        public Player(long id, string username, string number = null) : base(id, username)
        {
            Number = number;
        }
        public UserBase GetBase()
        {
            return new UserBase(Id, UserName);
        }
        public string Number { get; set; }
    }
}
