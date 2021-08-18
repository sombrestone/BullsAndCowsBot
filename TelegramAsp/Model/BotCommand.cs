using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramAsp.Model
{
    public class BCBotCommand
    {
        private readonly Action<CommandArgs> command;
        public string Text{get;}
        public bool IsPair { get; }
        public BCBotCommand(Action<CommandArgs> command, string text,bool isPair=false)
        {
            this.command = command;
            this.Text = text;
            this.IsPair = isPair;
        }

        public void Execute(CommandArgs param)
        {
            command(param);
        }
    }
}
