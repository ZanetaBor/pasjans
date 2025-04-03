using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Pasjans.Models;

namespace Pasjans.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private Deck _deck;
        public List<Card> Deck { get; private set; } // talia kart
        public List<Card> DrawPile { get; private set; } // Stos dobierania (zakryte karty)
        public List<Card> DiscardPile { get; private set; } // Odkryte karty
        public List<List<Card>> TableauPiles { get; private set; } // 10 stosów gry
        public List<List<Card>> FoundationPiles { get; private set; } // Miejsca na Asy

        public string DrawnCard { get; private set; }
        public ICommand DrawCardCommand { get; } //służy do wiązania poleceń z widokiem (np. przyciskiem)
        public ICommand ShuffleCommand { get; }

        // konstruktor GameViewModel
        public GameViewModel()
        {
            _deck = new Deck();
            _deck.Shuffle();
            Deck = new List<Card>(_deck.GetCards());
            DrawPile = new List<Card>(Deck);
            DiscardPile = new List<Card>();

            // Miejsca na Asy (cztery puste pola)
            FoundationPiles = new List<List<Card>> { new(), new(), new(), new() };

            // Inicjalizacja 10 stosów
            TableauPiles = new List<List<Card>>();
            for (int i = 0; i < 10; i++)
            {
                TableauPiles.Add(new List<Card>());
            }

            InitializeGame();

            ShuffleCommand = new RelayCommand(_ => ShuffleDeck());
            DrawCardCommand = new RelayCommand(_ => DrawCard());

            OnPropertyChanged(nameof(DrawPile));
            OnPropertyChanged(nameof(TableauPiles));
        }

        private void ShuffleDeck()
        {
            _deck.Shuffle();
            Deck = new List<Card>(_deck.GetCards());
            DrawPile = new List<Card>(Deck); // Resetujemy stos dobierania
            DiscardPile.Clear();

            OnPropertyChanged(nameof(Deck));
            OnPropertyChanged(nameof(DrawPile));
            OnPropertyChanged(nameof(DiscardPile));
        }

        private void InitializeGame()
        {
            DrawPile.Clear();
            DiscardPile.Clear();

            foreach (var pile in FoundationPiles)
            {
                pile.Clear();
            }
            foreach (var pile in TableauPiles)
            {
                pile.Clear();
            }

            // Rozdawanie kart do Tableau
            for (int i = 0; i < 10; i++)
            {
                int hiddenCount = i < 4 ? 5 : 4;
                for (int j = 0; j < hiddenCount; j++)
                {
                    if (DrawPile.Count > 0)
                    {
                        TableauPiles[i].Add(DrawPile.Last());
                        DrawPile.RemoveAt(DrawPile.Count - 1);
                    }
                }
                if (DrawPile.Count > 0)
                {
                    var lastCard = DrawPile.Last();
                    lastCard.IsFaceUp = true; // Ostatnia karta w kolumnie jest odkryta
                    TableauPiles[i].Add(lastCard);
                    DrawPile.RemoveAt(DrawPile.Count - 1);
                }
            }

            OnPropertyChanged(nameof(DrawPile));
            OnPropertyChanged(nameof(DiscardPile));
            OnPropertyChanged(nameof(FoundationPiles));
            OnPropertyChanged(nameof(TableauPiles));
        }

        private void DrawCard()
        {
            if (DrawPile.Count > 0)
            {
                var drawn = DrawPile.Last();
                DrawPile.RemoveAt(DrawPile.Count - 1);
                drawn.IsFaceUp = true;
                DiscardPile.Add(drawn);
                DrawnCard = drawn.ToString();

                OnPropertyChanged(nameof(DrawPile));
                OnPropertyChanged(nameof(DiscardPile));
                OnPropertyChanged(nameof(DrawnCard));
            }
            else
            {
                DrawnCard = "Brak kart w talii!";
                OnPropertyChanged(nameof(DrawnCard));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
