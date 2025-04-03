using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Pasjans.Models;
using Pasjans.View;

namespace Pasjans.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private Deck _deck;
<<<<<<< Updated upstream
        public List<Card> Deck { get; private set; }
=======
        public List<Card> Deck { get; private set; } // talia kart
        public List<Card> DrawPile { get; private set; } // Stos dobierania (zakryte karty)
        public List<Card> DiscardPile { get; private set; } // Odkryte karty
        public Card TopDrawPileCard => DrawPile.Any() ? DrawPile.Last() : null;
        public Card TopDiscardPileCard => DiscardPile.Any() ? DiscardPile.Last() : null;
        public List<List<Card>> TableauPiles { get; private set; } // 10 stosów gry
        public List<List<Card>> FoundationPiles { get; private set; } // Miejsca na Asy

>>>>>>> Stashed changes
        public string DrawnCard { get; private set; }
        public ICommand DrawCardCommand { get; } //służy do wiązania poleceń z widokiem (np. przyciskiem)
        public ICommand ShuffleCommand { get; }

        //konstruktor GameViewModel
        public GameViewModel()
        {
            _deck = new Deck();
<<<<<<< Updated upstream
            Deck = new List<Card>(_deck.GetCards());
=======
            _deck.Shuffle();
            DrawPile = new List<Card>(_deck.GetCards());
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

>>>>>>> Stashed changes
            ShuffleCommand = new RelayCommand(_ => ShuffleDeck());
            DrawCardCommand = new RelayCommand(_ => DrawCard());
        }

        private void ShuffleDeck()
        {
            _deck.Shuffle();
            Deck = new List<Card>(_deck.GetCards());
            OnPropertyChanged(nameof(Deck));
        }
<<<<<<< Updated upstream
=======

        private void InitializeGame()
        {
            DrawPile = new List<Card>(_deck.GetCards());

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

>>>>>>> Stashed changes
        private void DrawCard()
        {
            if (_deck != null && _deck.GetCards().Count > 0)
            {
<<<<<<< Updated upstream
                try
                {
                    Card drawn = _deck.DrawCard();
                    DrawnCard = drawn.ToString();
                    Deck = new List<Card>(_deck.GetCards());
                    OnPropertyChanged(nameof(DrawnCard));
                    OnPropertyChanged(nameof(Deck));
                }
                catch (InvalidOperationException)
                {
                    DrawnCard = "Brak kart w talii!";
                    OnPropertyChanged(nameof(DrawnCard));
                }
=======
                var drawn = DrawPile.Last();
                DrawPile.RemoveAt(DrawPile.Count - 1);
                drawn.IsFaceUp = true;
                DiscardPile.Add(drawn);
                DrawnCard = drawn.ToString();

                OnPropertyChanged(nameof(DrawPile));
                OnPropertyChanged(nameof(DiscardPile));
                OnPropertyChanged(nameof(DrawnCard));
                OnPropertyChanged(nameof(TopDrawPileCard));
                OnPropertyChanged(nameof(TopDiscardPileCard));
            }
            else
            {
                DrawnCard = "Brak kart w talii!";
                OnPropertyChanged(nameof(DrawnCard));
>>>>>>> Stashed changes
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