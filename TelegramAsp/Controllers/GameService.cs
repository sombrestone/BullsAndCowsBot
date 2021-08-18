using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BullsAndCowsGame
{
    public class GameService
    {
        IRepository<Game> repository;

        public Game game;

        public int NumOfTurn
        {
            get{return (int)Math.Ceiling((decimal)game.Turns.Count / 2);}
        }

        public GameService(UserBase player1, UserBase player2):this()
        {
            game = new Game
            {
                CurrentPlayer = new Player(player1.Id, player1.UserName),
                NextTurnPlayer = new Player(player2.Id, player2.UserName),
                Turns = new List<Turn>()
            };
        }

        public GameService(Game game):this()
        {
            this.game = game;
        }

        public GameService() 
        {
            repository = new GameJsonRep();
        }

        public static bool CheckTurn(string turn)
        {
            return (turn.Length==4)
                &&(int.TryParse(turn,out int n))
                &&(turn.Length - turn.ToCharArray().Distinct().Count()==0);
        }

        private void nextTurn()
        {
            Player temp = game.CurrentPlayer;
            game.CurrentPlayer = game.NextTurnPlayer;
            game.NextTurnPlayer = temp;
        }

        private int cows(string turn)
        {
            int result = 0;
            for(int i = 0; i < turn.Length; i++)
                for(int j = 0; j < game.NextTurnPlayer.Number.Length; j++)
                    if ((turn[i] == game.NextTurnPlayer.Number[j]) && (i != j)) result++;
            return result;
        }

        private int bulls(string turn)
        {
            int result = 0;
            for (int i = 0; i < turn.Length; i++)
                if (turn[i] == game.NextTurnPlayer.Number[i]) result++;
            return result;
        }

        Turn turn(string turn, UserBase player)
        {
            if (!CheckTurn(turn)) return new Turn { Status = TurnStatus.FormatEror };
            if (player.Id!= game.CurrentPlayer.Id) return new Turn { Status = TurnStatus.NotYourTurn };
            if (turn == game.NextTurnPlayer.Number) return endGameTurnBuilder();
            var result = turnBuider(turn);
            game.Turns.Add(result);
            nextTurn();
            return result;
        }

        Turn endGameTurnBuilder()
        {
            return new Turn
            {
                Status = TurnStatus.GameEnd,
                Player = game.CurrentPlayer,
                TurnNum = NumOfTurn
            };
        }

        Turn turnBuider(string turn)
        {
            return new Turn
            {
                Status = TurnStatus.Success,
                Player = game.CurrentPlayer,
                Number = turn,
                Bulls = bulls(turn),
                Cows = cows(turn),
                TurnNum = NumOfTurn
            };
        }

        public void Create()
        {
            repository.Create(this.game);
        }

        public void Update()
        {
            repository.Update(this.game);
        }

        public Game GetGame(UserBase player)
        {
            return repository.Get(player.Id);
        }

        public void SetNumber(UserBase player, string number)
        {
            if (game.CurrentPlayer.Id == player.Id) game.CurrentPlayer.Number = number;
            else game.NextTurnPlayer.Number = number;
        }

        public UserBase AnoutherPlayer(UserBase player)
        {
            return (player.Id == game.CurrentPlayer.Id) ? game.NextTurnPlayer : game.CurrentPlayer;
        }

        bool isSetNumber(UserBase player)
        {
            return (player.Id == game.CurrentPlayer.Id) ?
                (game.CurrentPlayer.Number != null)
                : (game.NextTurnPlayer.Number != null);
        }
        
        public string Turn(string turn, UserBase player, out string message,out bool isEnd)
        {
            message = "";
            isEnd = false;
            game=this.GetGame(player);
            if (!isSetNumber(player))
            {
                SetNumber(player, turn);
                Update();
                message = $"Игрок {player.UserName} загадал число.";
                return $"Вы загадали число { turn}.";
            }
            if (!isSetNumber(AnoutherPlayer(player)))
            {
                return "Другой игрок еще не определил число, подождите.";
            }
            Turn turnResult = this.turn(turn, player);
            if (turnResult.Status == TurnStatus.Success) {
                var result = $"Число быков: {turnResult.Bulls}.\nЧисло коров: {turnResult.Cows}";
                message = $"Игрок {player.UserName} совершил ход со следующим результатом:\n.{result}";
                Update();
                return result;
            }
            if (turnResult.Status == TurnStatus.GameEnd) 
            {
                repository.Delete(player.Id);
                message = $"Игрок {player.UserName} угадал ваше число и одержал победу.";
                isEnd = true;
                return $"Вы угадали число! Поздравляю.";
            }
            else return $"Сейчас не ваш ход!";
        }

        public static void Leave(UserBase player)
        {
            (new GameJsonRep()).Delete(player.Id);
        }
    }
}
