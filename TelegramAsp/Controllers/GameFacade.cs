using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramAsp.Model;

namespace BullsAndCowsGame
{
    public class GameFacade
    {
        static GameService GameSevice = new GameService();
        static UserService userService = new UserService();

        public static CommandArgs CommandArgsConstructor(Update update, TelegramBotClient client)
        {
            string[] args = update.Message.Text.ToLower().Split(' ');
            CommandArgs commandArgs = new CommandArgs
            {
                Sender = new UserBase(update.Message.From.Id, update.Message.From.Username),
                Command = args[0],
                Argument = (args.Length > 1) ? args[1] : null,
                BotClient = client,
            };
            var command = commands.Select(x=>x).Where(x=>x.Text.Equals(args[0])).FirstOrDefault();
            if (command == null) { 
                if (!GameService.CheckTurn(commandArgs.Command))throw new Exception("unnownCommand"); 
                else
                {
                    commandArgs.Argument = commandArgs.Command;
                    commandArgs.Command = "/turn";
                    command= commands.Select(x => x).Where(x => x.Text.Equals(commandArgs.Command)).FirstOrDefault();
                }
            }
            if (command.IsPair)
            {
                if (args.Length <= 1) throw new Exception("nullArguments");
                var user = userService.Get(commandArgs.Argument).Result;
                if (user == null) throw new Exception("userNotFound");
                commandArgs.Addressee = user.GetBase();
                if (commandArgs.Addressee.Id == commandArgs.Sender.Id) throw new Exception("selfInvite");
            }
            return commandArgs;
        }

        public static void OnMessage(CommandArgs args)
        {
             var command = commands.Select(x => x).Where(x => x.Text.Equals(args.Command.ToLower())).FirstOrDefault();
             command.Execute(args);
        }

        async void send(string code, CommandArgs args, bool isSender=true)
        {
            await args.BotClient.SendTextMessageAsync((isSender) ? args.Sender.Id : args.Addressee.Id,
                TelegramAsp.Controllers.MessageHandler.GetValue(code));
        }

        static List<BCBotCommand> commands = new List<BCBotCommand>
        {
            new BCBotCommand(newGame,"/game",true),
            new BCBotCommand(accept,"/accept",true),
            new BCBotCommand(leave,"/leave"),
            new BCBotCommand(help,"/help"),
            new BCBotCommand(start,"/start"),
            new BCBotCommand(turn,"/turn"),
            new BCBotCommand(status,"/status"),
        };
        
        static async void start(CommandArgs args)
        {
            var user = userService.Get(args.Sender.Id).Result;
            if (user != null) { help(args); return; }
            userService.NewUser(args.Sender.Id, args.Sender.UserName);
            help(args);
        }

        static async void help(CommandArgs args)
        {
            string res = $"Привет, {args.Sender.UserName}!\n" +
                $"Я бот для игры в \"Быки и коровы\".\n" +
                $"Для начала игры используй : \"/game (ник друга)\"\n" +
                $"Для просмотра твоего статуса и приглашений используй команду \"/status\"\n" +
                $"Для того что бы принять приглашение используй команду \"/accept (ник друга)\"\n" +
                $"Чтобы покинуть игру используй команду \"/leave\"\n";
                await args.BotClient.SendTextMessageAsync(args.Sender.Id, res);
        }


        static async void newGame(CommandArgs args)
        {
            bool success = await userService.AddInvite(args.Sender.UserName, args.Addressee.UserName);
            if (success)
            {
                await args.BotClient.SendTextMessageAsync(args.Addressee.Id, $"Игрок {args.Sender.UserName} приглашает вас в игру.\n" +
                    $"Для принятия приглашения воспользуйтесть командой \"/accept (ник друга)\".");
                await args.BotClient.SendTextMessageAsync(args.Sender.Id, $"Игрок {args.Addressee.UserName} получил приглашение, дождитесь ответа.");
            }
            else
            {
                if (userService.Get(args.Addressee.Id) == null) await args.BotClient.SendTextMessageAsync(args.Sender.Id, $"Пользователь " +
                        $"{args.Addressee.UserName} не найден.\nДля того чтобы пользователь мог получить приглашение " +
                       $"он должен написать мне \"/start\"");
                else await args.BotClient.SendTextMessageAsync(args.Sender.Id, $"Возможно вы уже приглашали данного пользователя");
            }
        }

        static async void accept(CommandArgs args)
        {
            bool success = await userService.AcceptInvite(args.Sender.UserName,args.Addressee.UserName);
            if (success)
            {
                await args.BotClient.SendTextMessageAsync(args.Addressee.Id, $"Игрок {args.Sender.UserName} принял ваше приглашение.\n " +
                    $"Загадайте четырехзначное число с неповторяющимися цифрами.\n");
                await args.BotClient.SendTextMessageAsync(args.Sender.Id, "Загадайте четырехзначное число с неповторяющимися цифрами.");
                GameService game = new GameService(args.Addressee,args.Sender);
                game.Create();
            }
            else await args.BotClient.SendTextMessageAsync(args.Sender.Id, $"Пользователь {args.Addressee.UserName} вас не приглашал.") ;
        }

        static async void turn(CommandArgs args)
        {
            if (userService.IsUserFree(args.Sender.UserName).Result) await args.BotClient.SendTextMessageAsync(args.Sender.Id, $"Вы не в игре\\");
            GameService game = new GameService();
            var result =game.Turn(args.Argument, args.Sender, out string message,out bool isEnd);
            await args.BotClient.SendTextMessageAsync(args.Sender.Id, result);
            if (message!="") await args.BotClient.SendTextMessageAsync(game.AnoutherPlayer(args.Sender).Id, message);
            if (isEnd) userService.EndGame(args.Sender.UserName, game.AnoutherPlayer(args.Sender).UserName);
        }

        static async void status(CommandArgs args)
        {
            await args.BotClient.SendTextMessageAsync(args.Sender.Id, userService.Status(args.Sender.UserName));
        }

        static async void leave(CommandArgs args)
        {
            if (userService.IsUserFree(args.Sender.UserName).Result) await args.BotClient.SendTextMessageAsync(args.Sender.Id, "Вы не в игре!");
            GameService game = new GameService();
            game.game = game.GetGame(args.Sender);
            UserBase player = game.AnoutherPlayer(args.Sender);
            userService.EndGame(player.UserName, args.Sender.UserName);
            GameService.Leave(args.Sender);
            await args.BotClient.SendTextMessageAsync(args.Sender.Id, "Вы покинули игру.");
            await args.BotClient.SendTextMessageAsync(player.Id, "Ваш соперник покинул игру! Вы победили!");
        }
    }
}
