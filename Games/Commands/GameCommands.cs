using DSharpPlus.Commands;

namespace Cardagin.Games.Commands
{
    public class GameCommands
    {

        [Command("start")]
        public async Task StartGame(CommandContext ctx, string gameName, bool autoJoin = true)
        {
            if (!ctx.Channel.IsThread) {
                await ctx.RespondAsync("Please only start games in threads.");
                return;
            }

            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                await ctx.RespondAsync("A game is already active in this channel. Please finish the current game before starting a new one.");
                return;
            }

            Game game = new EmptyGame();
            bool isRecognized = true;

            switch (Bot.gameAliases[gameName])
            {
                case GameType.BLACKJACK:
                    game = new Blackjack();
                    break;
                default:
                    await ctx.RespondAsync($"Game '{gameName}' not recognized.");
                    isRecognized = false;
                    return;
            }

            if (isRecognized)
            {
                game.Start(ctx);
                await ctx.RespondAsync($"Starting a new game of {game.Name}! For the game to begin there must be at least {game.MinPlayers} and at most {game.MaxPlayers}");
                if (autoJoin) await game.Join(ctx, true);
            }

            return;
            // Placeholder implementation for starting a game
        }

        [Command("end")]
        public async Task EndGame(CommandContext ctx)
        {
            await Bot.ActiveGames[ctx.Channel].End(ctx);
        }

        [Command("join")]
        public async Task JoinGame(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                await Bot.ActiveGames[ctx.Channel].Join(ctx);
            } else
            {
                await ctx.RespondAsync("There is no active game in this channel to join.");
            }
        }

        [Command("leave")]
        public async Task LeaveGame(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                await Bot.ActiveGames[ctx.Channel].Leave(ctx);
            } else
            {
                await ctx.RespondAsync("There is no active game in this channel.");
            }
        }

        [Command("deal")]
        public async Task DealCards(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                await Bot.ActiveGames[ctx.Channel].Deal(ctx);
            } else {
                await ctx.RespondAsync("There is no active game in this channel to deal cards for.");
            }
        }
    }
}