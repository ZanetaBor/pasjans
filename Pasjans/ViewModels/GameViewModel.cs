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
        public List<Card> Deck { get; private set; }
        public string DrawnCard { get; private set; }
        public ICommand DrawCardCommand { get; } //służy do wiązania poleceń z widokiem (np. przyciskiem)
        public ICommand ShuffleCommand { get; }

        //konstruktor GameViewModel
        public GameViewModel()
        {
            _deck = new Deck();
            Deck = new List<Card>(_deck.GetCards());
            ShuffleCommand = new RelayCommand(_ => ShuffleDeck());
            DrawCardCommand = new RelayCommand(_ => DrawCard());
        }

        private void ShuffleDeck()
        {
            _deck.Shuffle();
            Deck = new List<Card>(_deck.GetCards());
            OnPropertyChanged(nameof(Deck));
        }
        private void DrawCard()
        {
            if (_deck != null && _deck.GetCards().Count > 0)
            {
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