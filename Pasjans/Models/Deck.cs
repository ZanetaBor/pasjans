using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasjans.Models
{
    public class Deck
    {
        private List<Card> cards;

        // konstruktor klasy Deck
        public Deck()
        {
            cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    cards.Add(new Card(suit, rank));
                }
            }
        }
        public List<Card> GetCards()
        {
            return new List<Card>(cards);  
        }


        //tasowanie talii kard
        public void Shuffle()
        {
            Random rng = new Random();
            cards = cards.OrderBy(_ => rng.Next()).ToList();
        }

        //metoda pobierania karty z listy
        public Card DrawCard()
        {
            if (cards.Count == 0)
                throw new InvalidOperationException("Deck is empty");

            Card drawnCard = cards[^1]; // Pobiera ostatnią kartę
            cards.RemoveAt(cards.Count - 1);
            return drawnCard;
        }

        public override string ToString()
        {
            return string.Join(", ", cards);
        }
    }

}
