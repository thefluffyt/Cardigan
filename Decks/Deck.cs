using System;
using System.Collections.Generic;
using System.Text;

namespace Cardagin.Decks
{
    public abstract class Deck
    {
        public List<Card> Cards { get; set; } = new List<Card>();

        public void AddDeck(Deck deckToAdd, int n = 1)
        {
            for (int i = 0; i < n; i++)
            {
                Cards.AddRange(deckToAdd.Cards);
            }
        }

        public void AddCard(Card cardToAdd)
        {
            Cards.Add(cardToAdd);
        }

        public void Shuffle()
        {
            int n = Cards.Count;
            while (n > 1)
            {
                n--;
                int k = new Random().Next(n + 1);
                Card value = Cards[k];
                Cards[k] = Cards[n];
                Cards[n] = value;
            }
        }

        public Card DrawCard(int n = 0)
        {
            Card card = Cards[0];
            Cards.RemoveAt(n);
            return card;
        }
    }

    public class EmptyDeck : Deck
    {
        public EmptyDeck()
        {

        }
    }
}
