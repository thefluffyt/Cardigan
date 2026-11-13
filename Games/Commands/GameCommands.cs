using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Cardagin.Games.Commands
{
    public class GameCommands : BaseCommandModule
    {
        [Command("start")]
        public async Task StartGame(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                await ctx.Channel.SendMessageAsync("A game is already active in this channel. Please finish the current game before starting a new one.");
                return;
            }

            await ctx.Channel.SendMessageAsync("Please specify a game to start. Usage: `!start <game_name>`");
        }

        [Command("start")]
        public async Task StartGame(CommandContext ctx, string gameName)
        {

            if (!ctx.Channel.IsThread) {
                await ctx.Channel.SendMessageAsync("Please only start games in threads.");
                return;
            }

            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                await ctx.Channel.SendMessageAsync("A game is already active in this channel. Please finish the current game before starting a new one.");
                return;
            }

            Game game = new EmptyGame();
            bool isRecognized = true;

            switch (gameName.ToLower())
            {
                case "blackjack":
                    game = new Blackjack();
                    break;
                case "21":
                    game = new Blackjack();
                    break;
                case "bj":
                    game = new Blackjack();
                    break;
                default:
                    await ctx.Channel.SendMessageAsync($"Game '{gameName}' not recognized.");
                    isRecognized = false;
                    return;
            }

            if (isRecognized)
            {
                game.Start(ctx.Channel);
                await ctx.Channel.SendMessageAsync($"Starting a new game of {game.Name}! For the game to begin there must be at least {game.MinPlayers} and at most {game.MaxPlayers}");
                await JoinGame(ctx);
            }

            // Placeholder implementation for starting a game
        }

        [Command("end")]
        public async Task EndGame(CommandContext ctx)
        {
            Bot.ActiveGames[ctx.Channel].End();
            await ctx.Channel.SendMessageAsync("The current game has been ended.");
        }

        [Command("join")]
        public async Task JoinGame(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                await Bot.ActiveGames[ctx.Channel].Join(ctx);
            }

            await ctx.Channel.SendMessageAsync("There is no active game in this channel to join.");
        }

        [Command("leave")]
        public async Task LeaveGame(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                await Bot.ActiveGames[ctx.Channel].Leave(ctx);
            }

            await ctx.Channel.SendMessageAsync("There is no active game in this channel.");
        }

        [Command("deal")]
        public async Task DealCards(CommandContext ctx)
        {
            if (Bot.ActiveGames.ContainsKey(ctx.Channel))
            {
                if (Bot.ActiveGames[ctx.Channel].Players.Count < Bot.ActiveGames[ctx.Channel].MinPlayers)
                {
                    await ctx.Channel.SendMessageAsync("Cannot deal cards as there are not enough players in the game.");
                    return;
                } else if (Bot.ActiveGames[ctx.Channel].Players.Count > Bot.ActiveGames[ctx.Channel].MaxPlayers)  {
                    await ctx.Channel.SendMessageAsync("Cannot deal cards as there are too many players in the game.");
                    return;
                }

                    Bot.ActiveGames[ctx.Channel].Deal();
            } else {
                await ctx.Channel.SendMessageAsync("There is no active game in this channel to deal cards for.");
            }
        }
    }
}