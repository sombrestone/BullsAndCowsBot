using BullsAndCowsGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramAsp.Model
{
    public class CommandArgs
    {
        public CommandArgs() { }
        public UserBase Sender { get; set; }
        public UserBase Addressee { get; set; }
        public TelegramBotClient BotClient { get; set; }
        public string Command { get; set; }
        public string Argument { get; set; }
    }
}
