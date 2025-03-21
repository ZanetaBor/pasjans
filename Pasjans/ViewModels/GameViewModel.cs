using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pasjans.Models;

namespace Pasjans.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private Deck _deck;
        public List<Card> Deck { get; private set; }
        public List<Card> DrawPile { get; private set; }  // Stos dobierania (zakryte karty)
        public List<Card> DiscardPile { get; private set; } // Odkryte karty
        public List<List<Card>> TableauPiles { get; private set; } // 10 stosów gry
        public List<Card> FoundationPiles { get; private set; } // Miejsca na Asy
        
        public string DrawnCard { get; private set; }
        public ICommand DrawCardCommand { get; } //służy do wiązania poleceń z widokiem (np. przyciskiem)
        public ICommand ShuffleCommand { get; }

        //konstruktor GameViewModel
        public GameViewModel()
        {
            _deck = new Deck();
            _deck.Shuffle();

            Deck = new List<Card>(_deck.GetCards());
            DrawPile = new List<Card>(Deck);
            DiscardPile = new List<Card>();

            // Miejsca na Asy (cztery puste pola)
            FoundationPiles = new List<Card> { null, null, null, null };

            // Inicjalizacja 10 stosów
            TableauPiles = new List<List<Card>>();
            for (int i = 0; i < 10; i++)
            {
                TableauPiles.Add(new List<Card>());
            }

            // Rozdanie kart do Tableau
            for (int col = 0; col < 10; col++)
            {
                for (int row = 0; row < (col < 4 ? 6 : 5); row++)
                {
                    var card = DrawPile.Last();
                    DrawPile.RemoveAt(DrawPile.Count - 1);
                    TableauPiles[col].Add(card);
                }
            }

            ShuffleDeck();
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
            OnPropertyChanged(nameof(Deck));
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
                    TableauPiles[i].Add(_deck.DrawCard());
                }
                TableauPiles[i].Add(_deck.DrawCard()); // Ostatnia karta odkryta
            }

            // Reszta talii na stosie dobierania
            DrawPile.AddRange(_deck.GetCards());

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

        // Implementacja INotifyPropertyChanged dla MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}