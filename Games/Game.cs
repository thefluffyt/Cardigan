using DSharpPlus.Entities;

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
        public Dictionary<ulong, Sessions.UserInfo> Players { get; set; } = new Dictionary<ulong, Sessions.UserInfo>();
        public Decks.Deck? GameDeck { get; set; }
        public Decks.Deck? DealerDeck { get; set; }
        public DiscordChannel? Channel { get; set; }

        public virtual void Start(DiscordChannel channel)
        {
            Console.WriteLine($"{Name} started in {channel.Id}");
            Channel = channel;
            Bot.ActiveGames.Add(channel, this);
        }

        public virtual void Begin()
        {
            Console.WriteLine($"{Name} commenced in {Channel.Id}");
            InProgress = true;
        }

        public virtual void End()
        {
            InProgress = false;
            Bot.ActiveGames.Remove(Channel);
            Console.WriteLine($"Game '{Name}' has ended!");
        }

        public async virtual Task Join(DSharpPlus.CommandsNext.CommandContext ctx)
        {
            Game game = Bot.ActiveGames[ctx.Channel];
            if (game.Players.Count >= game.MaxPlayers)
            {
                await ctx.Channel.SendMessageAsync($"Sorry {ctx.User.Mention}, the game is already full.");
                return;
            }

            if (game.Players.ContainsKey(ctx.User.Id))
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Mention}, you have already joined the game.");
                return;
            }

            game.Players.Add(ctx.User.Id, new Sessions.UserInfo(ctx));
            await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} has joined the game! {game.MaxPlayers - game.Players.Count} slot{(game.MaxPlayers - game.Players.Count == 1 ? "" : "s")} left. {(game.MaxPlayers - game.Players.Count <= 0 ? " The game is now full no more players can join!" : "")} {(game.Players.Count - game.MinPlayers >= 0 ? "The game has the minimum required players and can now begin!" : "")}");
        }

        public async virtual Task Leave(DSharpPlus.CommandsNext.CommandContext ctx)
        {
            Game game = Bot.ActiveGames[ctx.Channel];
            if (game.Players.ContainsKey(ctx.User.Id))
            {
                game.Players.Remove(ctx.User.Id);
                await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} has left the game. {game.Players.Count} player{(game.Players.Count == 1 ? "" : "s")} remaining.");
            }

            await ctx.Channel.SendMessageAsync($"{ctx.User.Mention}, you are not part of the current game.");
        }

        public virtual void Deal()
        {
            InProgress = true;
            Console.WriteLine($"Dealing cards in game '{Name}'...");
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
}
