using DSharpPlus.Entities;

namespace Cardagin.Games
{
    public abstract class Game
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxPlayers { get; set; }
        public int MinPlayers { get; set; }
        public bool CanHotJoin { get; set; }
        public bool InProgress { get; set; } = false;
        public Dictionary<ulong, Sessions.UserInfo> Players { get; set; } = new Dictionary<ulong, Sessions.UserInfo>();
        public Decks.Deck GameDeck { get; set; }
        public Decks.Deck DealerDeck { get; set; }
        public DiscordChannel Channel { get; set; }

        public virtual void Start(DiscordChannel channel)
        {
            Console.WriteLine($"Game '{Name}' has started!");
            Channel = channel;
            Bot.ActiveGames.Add(channel, this);
        }

        public virtual void Begin()
        {
            Console.WriteLine($"Game '{Name}' is beginning!");
            InProgress = true;
        }

        public virtual void End()
        {
            InProgress = false;
            Bot.ActiveGames.Remove(Channel);
            Console.WriteLine($"Game '{Name}' has ended!");
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
