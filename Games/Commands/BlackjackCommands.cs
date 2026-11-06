using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cardagin.Games.Commands
{
    public class BlackjackCommands : BaseCommandModule
    {
        [Command("stall")]
        public async Task Stall(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("You have chosen to stall your turn.");
            // Placeholder implementation for stalling in blackjack
        }

    }
}
