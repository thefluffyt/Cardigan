using DSharpPlus.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cardagin.Games.Commands
{
    [Command("bj")]
    public class BlackjackCommands
    {
        [Command("stall")]
        public async Task Stall(CommandContext ctx)
        {
            await ctx.RespondAsync("You have chosen to stall your turn.");
            // Placeholder implementation for stalling in blackjack
        }

    }
}
