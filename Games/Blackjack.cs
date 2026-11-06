using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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

        public override void Start(DiscordChannel channel)
        {
            base.Start(channel);
        }

        public override void Begin()
        {
            base.Begin();
        }

        public override void Deal()
        {
            base.Deal();
            foreach (KeyValuePair<ulong, Sessions.UserInfo> player in Players)
            {
                Sessions.UserInfo userInfo = player.Value;
                userInfo.Decks[Channel].AddCard(GameDeck.DrawCard());
                userInfo.Decks[Channel].AddCard(GameDeck.DrawCard());
            }

            DealerDeck.AddCard(GameDeck.DrawCard());
            DealerDeck.AddCard(GameDeck.DrawCard());

            DealMessage();
        }

        public override void End()
        {
            base.End();
        }

        private async Task DealMessage()
        {
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

            await Channel.SendMessageAsync(sb.ToString()).ConfigureAwait(false);
        }
    }
}
