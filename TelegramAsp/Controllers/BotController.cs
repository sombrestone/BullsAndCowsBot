using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.Json;
using System.Text.Json.Serialization;
using BullsAndCowsGame;
using System.IO;
using Microsoft.Extensions.Logging;
using TelegramAsp.Model;

namespace TelegramAsp.Controllers
{
    [ApiController]
    [Route("api/bot")]
    public class BotController : ControllerBase
    {
        TelegramBotClient client = new TelegramBotClient("800222533:AAEj5y5O2gQTB0MctfCtSnEA5ZKQhkUjHXo");

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            try
            {
                var args = GameFacade.CommandArgsConstructor(update, client);
                GameFacade.OnMessage(args);
            }
            catch (Exception e)
            {
                string errorMessage = MessageHandler.Error(e);
                await client.SendTextMessageAsync(update.Message.From.Id, errorMessage);
            }
            return Ok();
        }
        
    }
}