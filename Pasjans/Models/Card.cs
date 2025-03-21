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
        public bool IsFaceUp { get; set; } //revers/obverse

        // Nazwa pliku obrazka
        public string ImageSource => IsFaceUp
            ? $"pack://application:,,,/Assets/{Suit.ToString().ToLower()}_{(int)Rank}.jpg"
            : $"pack://application:,,,/Assets/revers.jpg";

        
        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
            IsFaceUp = false;
        }

        public override string ToString() => $"{Rank} of {Suit}"; //przesłonięcie metody ToString() - wyswietlanie mojej nazwy karty zamiast ogolnej nazwy
    }

}
