using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasjans.Models
{
    public enum Suit
    {
        Hearts,    // Kier
        Diamonds,  // Karo
        Clubs,     // Trefl
        Spades     // Pik
    }

    public enum Rank
    {
        Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
        Jack, Queen, King, Ace
    }

    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }
        public string ImageSource => $"pack://application:,,,/Assets/{Suit.ToString().ToLower()}_{(int)Rank}.jpg";
        // Nazwa pliku obrazka
        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString() => $"{Rank} of {Suit}"; //przesłonięcie metody ToString() - wyswietlanie mojej nazwy karty zamiast ogolnej nazwy
    }

}
