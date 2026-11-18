using DSharpPlus.Entities;
using DSharpPlus.Commands;
using Cardagin.Decks;
using Microsoft.Extensions.Logging;

namespace Cardagin.Games
{
    public abstract class Game
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int MaxPlayers { get; set; }
        public int MinPlayers { get; set; }
        public bool CanHotJoin { get; set; }
        public bool InProgress { get; set; } = false;
        public uint CurrentPlayer = 0;
        public Dictionary<ulong, Sessions.UserInfo> Players { get; set; } = new Dictionary<ulong, Sessions.UserInfo>();
        public Decks.Deck GameDeck { get; set; } = new EmptyDeck();
        public Decks.Deck DealerDeck { get; set; } = new EmptyDeck();
        public DiscordChannel? Channel { get; set; }

        public virtual void Start(CommandContext ctx)
        {
            DiscordChannel channel = ctx.Channel;
            Bot.Logger.LogDebug($"{Name} started in {channel.Id}");
            Channel = channel;
            Bot.ActiveGames.Add(channel, this);
        }

        public async virtual Task End(CommandContext ctx)
        {
            ArgumentNullException.ThrowIfNull(Channel);

            InProgress = false;
            Bot.ActiveGames.Remove(Channel);
            Bot.Logger.LogDebug($"{Name} ended in {Channel.Id}");

            await ctx.RespondAsync($"The current game of {Name} has been ended.");
        }

        public async virtual Task Join(CommandContext ctx, bool autoJoin = false)
        {
            Game game = Bot.ActiveGames[ctx.Channel];
            if (game.Players.Count >= game.MaxPlayers)
            {
                await ctx.RespondAsync($"Sorry {ctx.User.Mention}, the game is already full.");
                return;
            }

            if (game.Players.ContainsKey(ctx.User.Id))
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, you have already joined the game.");
                return;
            }

            game.Players.Add(ctx.User.Id, new Sessions.UserInfo(ctx));
            Bot.Logger.LogInformation($"{ctx.User.Id} joined {Name} in {ctx.Channel}");
            if (autoJoin)
            {
                await ctx.FollowupAsync($"{ctx.User.Mention} has joined the game! {game.MaxPlayers - game.Players.Count} slot{(game.MaxPlayers - game.Players.Count == 1 ? "" : "s")} left. {(game.MaxPlayers - game.Players.Count <= 0 ? " The game is now full no more players can join!" : "")} {(game.Players.Count - game.MinPlayers >= 0 ? "The game has the minimum required players and can now begin!" : "")}");
            } else {
                await ctx.RespondAsync($"{ctx.User.Mention} has joined the game! {game.MaxPlayers - game.Players.Count} slot{(game.MaxPlayers - game.Players.Count == 1 ? "" : "s")} left. {(game.MaxPlayers - game.Players.Count <= 0 ? " The game is now full no more players can join!" : "")} {(game.Players.Count - game.MinPlayers >= 0 ? "The game has the minimum required players and can now begin!" : "")}");
            }
        }

        public async virtual Task Leave(CommandContext ctx)
        {
            Game game = Bot.ActiveGames[ctx.Channel];
            Bot.Logger.LogDebug($"{ctx.User.Id} left {Name} in {ctx.Channel}");
            if (game.Players.ContainsKey(ctx.User.Id))
            {
                game.Players.Remove(ctx.User.Id);
                Bot.Logger.LogInformation($"{ctx.User.Id} left {Name} in {ctx.Channel}");
                await ctx.RespondAsync($"{ctx.User.Mention} has left the game. {game.Players.Count} player{(game.Players.Count == 1 ? "" : "s")} remaining.");
                return;
            }

            await ctx.RespondAsync($"{ctx.User.Mention}, you are not part of the current game.");
        }

        public async virtual Task Deal(CommandContext ctx)
        {
            InProgress = true;
            if (Bot.ActiveGames[ctx.Channel].Players.Count < Bot.ActiveGames[ctx.Channel].MinPlayers)
            {
                Bot.Logger.LogWarning($"Too few players for {Name} in {ctx.Channel.Id} to deal");
                await ctx.RespondAsync("Cannot deal cards as there are not enough players in the game.");
                return;
            }
            else if (Bot.ActiveGames[ctx.Channel].Players.Count > Bot.ActiveGames[ctx.Channel].MaxPlayers)
            {
                Bot.Logger.LogWarning($"Too many players for {Name} in {ctx.Channel.Id} to deal");
                await ctx.RespondAsync("Cannot deal cards as there are too many players in the game.");
                return;
            }

            Bot.Logger.LogInformation($"Initial cards dealt for {Name} in {ctx.Channel.Id}");
            GameDeck.Shuffle();
        }

        public int HandTotal(CommandContext ctx, ulong userID)
        {
            int total = 0;

            Deck hand = Players[userID].Decks[ctx.Channel];




            return total;
        }
    }

    public class EmptyGame : Game
    {
        public EmptyGame()
        {
            Name = "Empty Game";
            Description = "This is a placeholder game with no functionality.";
        }
    }

    public enum GameType
    { 
        EMPTY = -1,
        BLACKJACK,
        POKER
    }
}
