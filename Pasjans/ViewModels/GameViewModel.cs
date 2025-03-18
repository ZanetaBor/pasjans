using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pasjans.Models;
using Pasjans.View;

namespace Pasjans.ViewModels
{
    public class GameViewModel
    {
        private Deck _deck;
        public string DrawnCard { get; private set; }
        public ICommand DrawCardCommand { get; } //służy do wiązania poleceń z widokiem (np. przyciskiem)
        public ICommand ShuffleCommand { get; }

        //konstruktor GameViewModel
        public GameViewModel()
        {
            _deck = new Deck();
            ShuffleCommand = new RelayCommand(_ => ShuffleDeck());
            DrawCardCommand = new RelayCommand(_ => DrawCard());
        }

        private void ShuffleDeck()
        {
            _deck.Shuffle();
            OnPropertyChanged(nameof(DeckString));
        }
        private void DrawCard()
        {
            if (_deck != null && _deck.ToString().Length > 0)
            {
                try
                {
                    DrawnCard = _deck.DrawCard().ToString();
                    OnPropertyChanged(nameof(DrawnCard));
                    OnPropertyChanged(nameof(DeckString));
                }
                catch (InvalidOperationException)
                {
                    DrawnCard = "Brak kart w talii!";
                    OnPropertyChanged(nameof(DrawnCard));
                }
            }
        }

        public string DeckString => _deck.ToString();

        // Implementacja INotifyPropertyChanged dla MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}