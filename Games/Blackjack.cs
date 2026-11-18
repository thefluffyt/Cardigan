using DSharpPlus.Commands;
using DSharpPlus;
using System.Text;
using DSharpPlus.Entities;
using System.Collections.Immutable;

namespace Cardagin.Games
{
    public class Blackjack : Game
    {
        public Blackjack()
        {
            Name = "Blackjack";
            Description = "A classic casino card game where players try to get as close to 21 as possible without going over.";
            MinPlayers = 1;
            MaxPlayers = 7;
            CanHotJoin = true;
            GameDeck = new Decks.StandardDeck();
        }

        public override void Start(CommandContext ctx)
        {
            base.Start(ctx);
        }

        public async override Task Deal(CommandContext ctx)
        {
            ArgumentNullException.ThrowIfNull(Channel);
            ArgumentNullException.ThrowIfNull(GameDeck);
            ArgumentNullException.ThrowIfNull(DealerDeck);

            await base.Deal(ctx);

            foreach (KeyValuePair<ulong, Sessions.UserInfo> player in Players)
            {
                Sessions.UserInfo userInfo = player.Value;

                userInfo.Decks[Channel].AddCard(GameDeck.DrawCard());
                userInfo.Decks[Channel].AddCard(GameDeck.DrawCard());
            }

            DealerDeck.AddCard(GameDeck.DrawCard());
            DealerDeck.AddCard(GameDeck.DrawCard());

            await DealMessage(ctx);
        }

        public async override Task End(CommandContext ctx)
        {
            await base.End(ctx);
        }

        private async Task DealMessage(CommandContext ctx)
        {
            ArgumentNullException.ThrowIfNull(Channel);
            ArgumentNullException.ThrowIfNull(DealerDeck);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Dealing cards...");
            foreach (KeyValuePair<ulong, Sessions.UserInfo> player in Players)
            {
                Sessions.UserInfo userInfo = player.Value;
                sb.AppendLine($"{userInfo.User.Username}'s hand: ");
                foreach (Decks.Card card in userInfo.Decks[Channel].Cards)
                {
                    sb.AppendLine($" - {card.GetShortCardName()}");
                }
            }

            sb.AppendLine($"\nDealer's visible card: {DealerDeck.Cards[0].GetShortCardName()}");

            sb.AppendLine($"<@{Players.Keys.ToImmutableArray()[0]}> it is your turn; hit or stand.");

            await ctx.RespondAsync(sb.ToString()).ConfigureAwait(false);
        }
    }
}
