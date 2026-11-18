using System;
using System.Collections.Generic;
using System.Text;

namespace Cardagin.Decks
{
    public class StandardCard : Card
    {
        public Rank rank { get; private set; }
        public Suit suit { get; private set; }
        private Dictionary<Suit, string> suitSymbols = new Dictionary<Suit, string>
        {
            { Suit.Club, "♣" },
            { Suit.Diamond, "♦" },
            { Suit.Heart, "♥" },
            { Suit.Spade, "♠" }
        };

        public StandardCard()
        {
            rank = Rank.Joker;
            suit = Suit.Club;
        }

        public StandardCard(Rank rank, Suit suit)
        {
            this.rank = rank;
            this.suit = suit;
        }

        public override string GetCardName()
        {
            return $"{rank} of {suit}s";
        }

        public override string GetShortCardName()
        {
            return $"{rank} {suitSymbols[suit]}";
        }

        public override int GetCardValue(Game game)
        { 
            switch (rank)
            {
                case Rank.Two: return 2;
                case Rank.Three: return 3;
                case Rank.Four: return 4;
                case Rank.Five: return 5;
                case Rank.Six: return 6;
                case Rank.Seven: return 7;
                case Rank.Eight: return 8;
                case Rank.Nine: return 9;
                case Rank.Ten: return 10;
                case Rank.Jack: return 10;
                case Rank.
            }
        }

        public enum Rank
        {
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace,
            Joker
        }

        public enum Suit
        {
            Club,
            Diamond,
            Heart,
            Spade
        }
    }
}
