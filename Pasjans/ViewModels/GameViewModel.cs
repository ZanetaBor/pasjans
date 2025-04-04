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
        private bool _isGameWon;

        public List<Card> DrawPile { get; private set; } // Stos dobierania (zakryte karty)
        public List<Card> DiscardPile { get; private set; } // Odkryte karty

        public Card TopDrawPileCard => DrawPile.Any() ? DrawPile.Last() : null;
        public Card TopDiscardPileCard => DiscardPile.Any() ? DiscardPile.Last() : null;

        public List<List<Card>> TableauPiles { get; private set; } // 10 stosów gry
        public List<List<Card>> FoundationPiles { get; private set; } // Miejsca na Asy

        public string GameStatus { get; private set; }
        public bool IsGameWon
        {
            get => _isGameWon;
            private set
            {
                _isGameWon = value;
                OnPropertyChanged(nameof(IsGameWon));
            }
        }

        // Komendy
        public ICommand DrawCardCommand { get; }
        public ICommand RestartGameCommand { get; }
        public ICommand MoveCardCommand { get; }
        public ICommand MoveToFoundationCommand { get; }
        public ICommand AutoMoveCommand { get; }

        // Konstruktor GameViewModel
        public GameViewModel()
        {
            // Inicjalizacja stosów
            DrawPile = new List<Card>();
            DiscardPile = new List<Card>();

            // Miejsca na Asy (cztery puste pola)
            FoundationPiles = new List<List<Card>> { new(), new(), new(), new() };

            // Inicjalizacja 10 stosów
            TableauPiles = new List<List<Card>>();
            for (int i = 0; i < 10; i++)
            {
                TableauPiles.Add(new List<Card>());
            }

            // Inicjalizacja komend
            DrawCardCommand = new RelayCommand(_ => DrawCard(), _ => CanDrawCard());
            RestartGameCommand = new RelayCommand(_ => InitializeGame());
            MoveCardCommand = new RelayCommand(param => MoveCard(param), param => CanMoveCard(param));
            MoveToFoundationCommand = new RelayCommand(param => MoveToFoundation(param), param => CanMoveToFoundation(param));
            AutoMoveCommand = new RelayCommand(_ => AutoMove(), _ => CanAutoMove());

            // Rozpocznij nową grę
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Resetowanie statusu gry
            IsGameWon = false;
            GameStatus = "Nowa gra rozpoczęta!";

            // Czyszczenie wszystkich stosów
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

            // Tworzenie i tasowanie nowej talii
            _deck = new Deck();
            _deck.Shuffle();
            DrawPile = new List<Card>(_deck.GetCards());

            // Rozdawanie kart do Tableau
            for (int i = 0; i < 10; i++)
            {
                int hiddenCount = i < 4 ? 5 : 4;
                for (int j = 0; j < hiddenCount; j++)
                {
                    if (DrawPile.Count > 0)
                    {
                        Card card = DrawPile.Last();
                        card.IsFaceUp = false;
                        TableauPiles[i].Add(card);
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

            // Powiadomienie o zmianie właściwości
            NotifyAllPropertyChanges();
        }

        private bool CanDrawCard()
        {
            return DrawPile.Any() && !IsGameWon;
        }

        private void DrawCard()
        {
            if (DrawPile.Count > 0)
            {
                var drawn = DrawPile.Last();
                DrawPile.RemoveAt(DrawPile.Count - 1);
                drawn.IsFaceUp = true;
                DiscardPile.Add(drawn);
                GameStatus = $"Dobrano kartę: {drawn}";

                CheckGameWon();
                NotifyDiscardAndDrawPileChanges();
            }
            else
            {
                GameStatus = "Brak kart w talii!";
                OnPropertyChanged(nameof(GameStatus));
            }
        }

        // Metoda obsługująca przenoszenie karty z jednego stosu do innego
        private void MoveCard(object parameter)
        {
            if (parameter is Tuple<Card, int, int> moveInfo)
            {
                Card card = moveInfo.Item1;
                int sourceIndex = moveInfo.Item2;
                int targetIndex = moveInfo.Item3;

                // Implementacja przenoszenia karty
                // Przykład: przenosimy kartę z jednego stosu tableau do innego
                List<Card> sourceList;
                List<Card> targetList = TableauPiles[targetIndex];
                int cardIndex;

                // Ustalenie źródłowego stosu
                if (sourceIndex == -1) // Jeśli karta pochodzi ze stosu odkrytych
                {
                    sourceList = DiscardPile;
                    cardIndex = sourceList.IndexOf(card);
                }
                else
                {
                    sourceList = TableauPiles[sourceIndex];
                    cardIndex = sourceList.IndexOf(card);
                }

                // Jeśli to jest karta z kolumny tableau, pobieramy też wszystkie karty na niej
                List<Card> cardsToMove = new List<Card>();
                if (sourceIndex >= 0 && cardIndex >= 0)
                {
                    cardsToMove.AddRange(sourceList.GetRange(cardIndex, sourceList.Count - cardIndex));
                    sourceList.RemoveRange(cardIndex, sourceList.Count - cardIndex);

                    // Upewnij się, że ostatnia karta w źródłowym stosie jest odkryta
                    if (sourceList.Any())
                    {
                        sourceList.Last().IsFaceUp = true;
                    }
                }
                else if (sourceIndex == -1 && cardIndex >= 0)
                {
                    cardsToMove.Add(card);
                    sourceList.RemoveAt(cardIndex);
                }

                // Dodaj karty do docelowego stosu
                targetList.AddRange(cardsToMove);

                GameStatus = $"Przeniesiono kartę {card} do stosu {targetIndex + 1}";

                CheckGameWon();
                NotifyAllPropertyChanges();
            }
        }

        private bool CanMoveCard(object parameter)
        {
            if (parameter is Tuple<Card, int, int> moveInfo && !IsGameWon)
            {
                Card card = moveInfo.Item1;
                int sourceIndex = moveInfo.Item2;
                int targetIndex = moveInfo.Item3;

                // Sprawdzenie, czy ruch jest dozwolony według zasad gry
                if (targetIndex >= 0 && targetIndex < 10) // Upewnij się, że docelowy indeks jest prawidłowy
                {
                    List<Card> targetPile = TableauPiles[targetIndex];

                    // Jeśli stos docelowy jest pusty, tylko król może być na nim położony
                    if (!targetPile.Any())
                    {
                        return card.Rank == Rank.King;
                    }
                    else
                    {
                        // Sprawdź, czy karta może być położona na wierzchu stosu docelowego
                        Card topCard = targetPile.Last();

                        // Karta musi być o jeden stopień niższa i przeciwnego koloru
                        bool isOppositeColor = IsOppositeColor(card.Suit, topCard.Suit);
                        bool isOneLower = (int)card.Rank == (int)topCard.Rank - 1;

                        return isOppositeColor && isOneLower;
                    }
                }
            }
            return false;
        }

        // Metoda obsługująca przenoszenie karty do stosu foundacji
        private void MoveToFoundation(object parameter)
        {
            if (parameter is Tuple<Card, int> moveInfo)
            {
                Card card = moveInfo.Item1;
                int sourceIndex = moveInfo.Item2;

                // Znajdź odpowiedni stos foundacji dla danego koloru
                int foundationIndex = (int)card.Suit;
                List<Card> foundationPile = FoundationPiles[foundationIndex];

                // Usuń kartę ze źródłowego stosu
                if (sourceIndex == -2) // Karta pochodzi ze stosu odrzuconych
                {
                    DiscardPile.Remove(card);
                    foundationPile.Add(card);
                }
                else if (sourceIndex >= 0 && sourceIndex < 10) // Karta pochodzi z tableau
                {
                    List<Card> sourcePile = TableauPiles[sourceIndex];
                    sourcePile.Remove(card);

                    // Upewnij się, że ostatnia karta w źródłowym stosie jest odkryta
                    if (sourcePile.Any())
                    {
                        sourcePile.Last().IsFaceUp = true;
                    }

                    foundationPile.Add(card);
                }

                GameStatus = $"Przeniesiono kartę {card} do foundacji";

                CheckGameWon();
                NotifyAllPropertyChanges();
            }
        }

        private bool CanMoveToFoundation(object parameter)
        {
            if (parameter is Tuple<Card, int> moveInfo && !IsGameWon)
            {
                Card card = moveInfo.Item1;

                // Ustal, do którego stosu foundacji powinna trafić karta
                int foundationIndex = (int)card.Suit;
                List<Card> foundationPile = FoundationPiles[foundationIndex];

                // Jeśli stos jest pusty, tylko As może być na nim położony
                if (!foundationPile.Any())
                {
                    return card.Rank == Rank.Ace;
                }
                else
                {
                    // W przeciwnym razie karta musi być o jeden stopień wyższa i tego samego koloru
                    Card topCard = foundationPile.Last();
                    return (int)card.Rank == (int)topCard.Rank + 1 && card.Suit == topCard.Suit;
                }
            }
            return false;
        }

        // Automatyczne przenoszenie kart, które mogą być przeniesione do foundacji
        private void AutoMove()
        {
            bool cardMoved;
            do
            {
                cardMoved = false;

                // Sprawdź karty na wierzchu stosów tableau
                for (int i = 0; i < TableauPiles.Count; i++)
                {
                    var pile = TableauPiles[i];
                    if (pile.Any())
                    {
                        Card topCard = pile.Last();
                        if (topCard.IsFaceUp && CanMoveToFoundation(new Tuple<Card, int>(topCard, i)))
                        {
                            MoveToFoundation(new Tuple<Card, int>(topCard, i));
                            cardMoved = true;
                            break;
                        }
                    }
                }

                // Sprawdź kartę na wierzchu stosu odrzuconych
                if (!cardMoved && DiscardPile.Any())
                {
                    Card topCard = DiscardPile.Last();
                    if (CanMoveToFoundation(new Tuple<Card, int>(topCard, -2)))
                    {
                        MoveToFoundation(new Tuple<Card, int>(topCard, -2));
                        cardMoved = true;
                    }
                }
            } while (cardMoved);

            GameStatus = "Automatyczne ruchy zakończone";
            OnPropertyChanged(nameof(GameStatus));
        }

        private bool CanAutoMove()
        {
            // Sprawdź, czy jakakolwiek karta może być przeniesiona do foundacji
            for (int i = 0; i < TableauPiles.Count; i++)
            {
                var pile = TableauPiles[i];
                if (pile.Any() && pile.Last().IsFaceUp && CanMoveToFoundation(new Tuple<Card, int>(pile.Last(), i)))
                {
                    return true;
                }
            }

            if (DiscardPile.Any() && CanMoveToFoundation(new Tuple<Card, int>(DiscardPile.Last(), -2)))
            {
                return true;
            }

            return false;
        }

        // Sprawdź, czy gra została wygrana (wszystkie karty w foundacji)
        private void CheckGameWon()
        {
            int totalFoundationCards = FoundationPiles.Sum(pile => pile.Count);
            if (totalFoundationCards == 52) // 52 karty w talii
            {
                IsGameWon = true;
                GameStatus = "Gratulacje! Wygrałeś!";
                OnPropertyChanged(nameof(GameStatus));
            }
        }

        // Pomocnicza metoda do sprawdzania, czy kolory kart są przeciwne
        private bool IsOppositeColor(Suit suit1, Suit suit2)
        {
            bool suit1IsRed = suit1 == Suit.Hearts || suit1 == Suit.Diamonds;
            bool suit2IsRed = suit2 == Suit.Hearts || suit2 == Suit.Diamonds;
            return suit1IsRed != suit2IsRed;
        }

        // Powiadomienie o zmianie wszystkich głównych właściwości
        private void NotifyAllPropertyChanges()
        {
            OnPropertyChanged(nameof(DrawPile));
            OnPropertyChanged(nameof(DiscardPile));
            OnPropertyChanged(nameof(TopDrawPileCard));
            OnPropertyChanged(nameof(TopDiscardPileCard));
            OnPropertyChanged(nameof(TableauPiles));
            OnPropertyChanged(nameof(FoundationPiles));
            OnPropertyChanged(nameof(GameStatus));
        }

        // Powiadomienie o zmianie stosu kart do dobierania i odrzuconych
        private void NotifyDiscardAndDrawPileChanges()
        {
            OnPropertyChanged(nameof(DrawPile));
            OnPropertyChanged(nameof(DiscardPile));
            OnPropertyChanged(nameof(TopDrawPileCard));
            OnPropertyChanged(nameof(TopDiscardPileCard));
            OnPropertyChanged(nameof(GameStatus));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}