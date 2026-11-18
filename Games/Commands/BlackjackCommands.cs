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

        [Command("hit")]
        public async Task Hit(CommandContext ctx)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{ctx.User.Username} you have hit here is your new hand:");
            //Display hand
            //Display whether they have bust

            await ctx.RespondAsync("");
        }
    }
}
