using PokerGame;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Xml;
using static System.Console;

using System;
using System.Collections.Generic;
using System.Linq;



int j = 0;



WriteLine("Welcome to Poker!");

do
{
  
 List<Player> players = new List<Player>();

    Write("Enter number of players: ");
    int numPlayers = int.Parse(ReadLine());

   
    for (int i = 0; i < numPlayers; i++)
    {
        Write("Enter name of player " + (i + 1) + ": ");
        string name = ReadLine();
           
        players.Add(new Player(name, "Dealer"));
    }
    
    Dealer dealer = new Dealer("Dealer");
    Game game = new Game(players, dealer);


    game.Start();



    WriteLine("\n____________________\n | If u want to :\n | play again : 1\n | Quit : 2");
    j = int.Parse(ReadLine());

    Clear();
} while (j < 2);

namespace PokerGame
{
    class Game
    {
        private List<Player> players;
        private Dealer dealer;
        private Deck deck;
        private int chipTT;

        public Game(List<Player> players, Dealer dealer)
        {
            this.players = players;
            this.dealer = dealer;
            deck = new Deck();
        }


        public void Start()
        {
            //do
            //{

            foreach (Player player in players)
            {
                player.Haspass = false;
            }

            deck = new Deck();

                // Shuffle the deck
                deck.Shuffle();

                foreach(Player player in players)
                {
                    chipTT += 20;

                    player.RemoveChips(20);
                }

                // Deal two cards to each player
                foreach (Player player in players)
                {
                    if (player.Haspass == false)
                    {
                        player.AddCard(deck.DrawCard());
                        player.AddCard(deck.DrawCard());

                        WriteLine(player.Name + " has: ");
                        foreach (Card card in player.Cards)
                        {
                            WriteLine("  " + card);
                        }

                        player.Continue(player);
                    }
                    if (AllPlayersHavePassed() == false)
                    {
                        Clear();
                        // End the game
                        WriteLine("Tout les joueurs se sont coucher. la partie est fini.");
                        return;
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    dealer.AddCard(deck.DrawCard());


                    foreach (Player player in players)
                    {
                        if (player.Haspass == false)
                        {
                            WriteLine(player.Name + " has: ");
                            foreach (Card card in player.Cards)
                            {
                                WriteLine("  " + card);
                            }

                            WriteLine("Dealer has: ");
                            foreach (Card card in dealer.Cards)
                            {
                                WriteLine("  " + card);
                            }

                            player.Continue(player);
                        }
                        if (AllPlayersHavePassed() == false)
                        {
                            Clear();
                            // End the game
                            WriteLine("Tout les joueurs se sont coucher. la partie est fini.");
                            return;
                        }
                    }
                }

                WriteLine("Résult : ");

                // Print the hands of the players
                foreach (Player player in players)
                {
                    if (player.Haspass == false)
                    {
                        WriteLine(player.Name + " has: ");
                        foreach (Card card in player.Cards)
                        {
                            WriteLine("  " + card);
                        }
                    }
                }

                // Print the hand of the dealer
                WriteLine("Dealer has: ");
                foreach (Card card in dealer.Cards)
                {
                    WriteLine("  " + card);
                }

                // Check the hands of the players and the dealer
                foreach (Player player in players)
                {
                    player.AddDealerHand(dealer.Cards);

                    int result = player.CheckHand();

                    switch (result)
                    {
                        case (10):
                            WriteLine(" " + player.Name + " à eux une Carte Haute \n");
                            break;

                        case (9):
                            WriteLine(" " + player.Name + " à eux une Pair !!!\n");
                            break;

                        case (8):
                            WriteLine(" " + player.Name + " à eux deux Pair !!!\n");

                            break;

                        case (7):
                            WriteLine(" " + player.Name + " à eux Brelan !!!\n");

                            break;

                        case (6):
                            WriteLine(" " + player.Name + " à eux un Suite !!!\n");

                            break;

                        case (5):
                            WriteLine(" " + player.Name + " à eux une Couleur !!!\n");

                            break;

                        case (4):
                            WriteLine(" " + player.Name + " à eux un Full !!!\n");

                            break;

                        case (3):
                            WriteLine(" " + player.Name + " à eux un Carrée !!!\n");

                            break;

                        case (2):
                            WriteLine(" " + player.Name + " à eux une QuinteFlush !!!\n");

                            break;

                        case (1):
                            WriteLine(" " + player.Name + " à eux une QuinteFlush Royale !!!\n");

                            break;
                    }

                }


                foreach (Player player in players)
                {
                    player.ClearHand();
                }
                dealer.ClearHand();

                RemovePlayersOutOfChips();

                CheckChips();

            //} while ();
           
        }


        public void RemovePlayersOutOfChips()
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].IsOutOfChips())
                {
                    players.RemoveAt(i);
                    i--;
                }
            }
        }

        public bool AllPlayersHavePassed()
        {
            int i = 0;
            foreach (Player player in players)
            {
                if (player.Haspass == true)
                {
                    i++;
                }
            }
            if(i == players.Count)
            {
                return false;

            }
            return true;
        }

        public void CheckChips()
        {
            foreach (Player player in players)
            {
                WriteLine($"{player.Name} has {player.Chips} chips.");
            }
        }



    }

    class Dealer
    {
        public string Name { get; private set; }
        public List<Card> Cards { get; private set; }

        public Dealer(string name)
        {
            Name = name;
            Cards = new List<Card>();
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public void ClearHand()
        {
            Cards.Clear();
        }
    }

    class Player : Dealer
    {
        public string Name { get; private set; }

        public int Chips { get; set; }

        public bool Haspass { get; set; }

        public List<Card> Cards { get; private set; }

        public Player(string nameJ,string name) : base(name)
        {
            Name = nameJ;
            Chips = 500;
            Haspass = false;
            Cards = new List<Card>();
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public bool IsOutOfChips()
        {
            return Chips == 0;
        }

        public bool HasPass()
        {
            return Haspass = true;
        }

        public void CheckChipsIndividual(Player player)
        {
            WriteLine($"{player.Name} has {player.Chips} chips.\n\n");
        }

        public void Continue(Player player)
        {
            int Result;


            WriteLine("Que voulez vous faire?\n1 : Check\n2 : Bet More\n3 : pass\n");

            CheckChipsIndividual(player);

            Result = int.Parse(ReadLine());

            switch (Result)
            {
                case (1):
                    Clear();

                    WriteLine(" " + player.Name + " Reste \n");
                    break;
                case (2):
                    Clear();
                    WriteLine(" " + player.Name + " est Partie \n");

                    player.HasPass();

                    

                    break;
                case (3):

                    player.HasPass();

                    Clear();

                    WriteLine(" " + player.Name + " est Partie \n");

                    break;
            }
        }

        public void RemoveChips(int amount)
        {
            Chips -= amount;
        }


        public void AddDealerHand(List<Card> dealerHand)
        {
            // Add the dealer's cards to the player's hand
            Cards.AddRange(dealerHand);
        }

        //#pragma region Combo
            public bool IsStraight(List<Card> cards)
            {
                // Sort the cards by rank
                cards.Sort((a, b) => a.Rank.CompareTo(b.Rank));

                // Check for a straight
                for (int i = 1; i < cards.Count; i++)
                {
                    if (cards[i].Rank != cards[i - 1].Rank + 1)
                    {
                        return false; // Not a straight
                    }
                }

                return true; // Straight
            }

            public bool IsFlush(List<Card> cards)
            {
                // Check for a flush
                for (int i = 1; i < cards.Count; i++)
                {
                    if (cards[i].Suit != cards[0].Suit)
                    {
                        return false; // Not a flush
                    }
                }

                return true; // Flush
            }

            public bool HasFourOfAKind(List<Card> cards)
            {
                // Check for four of a kind
                for (int i = 0; i < cards.Count; i++)
                {
                    int count = 0;
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (cards[i].Rank == cards[j].Rank)
                        {
                            count++;
                        }
                    }
                    if (count == 4)
                    {
                        return true; // Four of a kind
                    }
                }

                return false; // No four of a kind
            }

            public bool HasFullHouse(List<Card> cards)
            {
                // Check for three of a kind
                bool hasThreeOfAKind = false;
                for (int i = 0; i < cards.Count; i++)
                {
                    int count = 0;
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (cards[i].Rank == cards[j].Rank)
                        {
                            count++;
                        }
                    }
                    if (count == 3)
                    {
                        hasThreeOfAKind = true;
                        break;
                    }
                }

                // Check for a pair
                bool hasPair = false;
                for (int i = 0; i < cards.Count; i++)
                {
                    for (int j = i + 1; j < cards.Count; j++)
                    {
                        if (cards[i].Rank == cards[j].Rank)
                        {
                            hasPair = true;
                            break;
                        }
                    }
                    if (hasPair)
                    {
                        break;
                    }
                }

                return hasThreeOfAKind && hasPair; // Full house
            }

            public bool HasThreeOfAKind(List<Card> cards)
            {
                // Check for three of a kind
                for (int i = 0; i < cards.Count; i++)
                {
                    int count = 0;
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (cards[i].Rank == cards[j].Rank)
                        {
                            count++;
                        }
                    }
                    if (count == 3)
                    {
                        return true; // Three of a kind
                    }
                }

                return false; // No three of a kind
            }

            public bool HasTwoPair(List<Card> cards)
            {
                // Check for two pair
                int pairCount = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    for (int j = i + 1; j < cards.Count; j++)
                    {
                        if (cards[i].Rank == cards[j].Rank)
                        {
                            pairCount++;
                            break;
                        }
                    }
                }

                return pairCount == 2; // Two pair
            }

            public bool HasPair(List<Card> cards)
            {
                // Check for a pair
                for (int i = 0; i < cards.Count; i++)
                {
                    for (int j = i + 1; j < cards.Count; j++)
                    {
                        if (cards[i].Rank == cards[j].Rank)
                        {
                            return true; // Pair
                        }
                    }
                }

                return false; // No pair
            }

        //#pragma endregion Combo


        public int CheckHand()
        {
           

            // Check for royal flush
            bool straight = IsStraight(Cards);
            bool flush = IsFlush(Cards);

            if (straight && flush && Cards[0].Rank == Rank.Ace)
            {
                return 1; // Royal flush
            }

            // Check for straight flush
            if (straight && flush)
            {
                return 2; // Straight flush
            }

            // Check for four of a kind
            if (HasFourOfAKind(Cards))
            {
                return 3; // Four of a kind
            }

            // Check for full house
            if (HasFullHouse(Cards))
            {
                return 4; // Full house
            }

            // Check for flush
            if (flush)
            {
                return 5; // Flush
            }

            // Check for straight
            if (straight)
            {
                return 6; // Straight
            }

            // Check for three of a kind
            if (HasThreeOfAKind(Cards))
            {
                return 7; // Three of a kind
            }

            // Check for two pair
            if (HasTwoPair(Cards))
            {
                return 8; // Two pair
            }

            // Check for pair
            if (HasPair(Cards))
            {
                return 9; // Pair
            }

            // Otherwise, return high card
            return 10; // High card
        }

    }

    class Card
    {
        public Suit Suit { get; private set; }
        public Rank Rank { get; private set; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return Rank + " of " + Suit;
        }
    }

    class Deck
    {
        private List<Card> cards;

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

        public void Shuffle()
        {
            Random random = new Random();
            cards = cards.OrderBy(x => random.Next()).ToList();
        }

        public Card DrawCard()
        {
            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }

    enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    enum Rank
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }
}



//Deck deck = new Deck();
//Dealer dealer = new Dealer("Dealer");
//Player player = new Player("Ben");


//List<Player> playerTT = new List<Player>();

//deck.Shuffle();

//Game game = new Game(player, dealer, deck);

//game.DealCards();

////deck.DisplayCards();

//WriteLine("----------------");
//player.DisplayCards();
//player.CheckHand();
//WriteLine("----------------");
//dealer.DisplayCards();

//namespace PokerGame
//{

//    enum Suit
//    {
//        Clubs,
//        Diamonds,
//        Hearts,
//        Spades
//    }

//    enum Rank
//    {
//        Two,
//        Three,
//        Four,
//        Five,
//        Six,
//        Seven,
//        Eight,
//        Nine,
//        Ten,
//        Jack,
//        Queen,
//        King,
//        Ace
//    }

//    class Card
//    {
//        public Suit Suit { get; private set; }
//        public Rank Rank { get; private set; }

//        public Card(Suit suit, Rank rank)
//        {
//            Suit = suit;
//            Rank = rank;
//        }

//        public override string ToString()
//        {
//            return Rank + " of " + Suit;
//        }

//    }

//    class Deck
//    {
//        private List<Card> cards;

//        public Deck()
//        {
//            cards = new List<Card>();

//            // Populate the deck with 52 cards
//            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
//            {
//                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
//                {
//                    cards.Add(new Card(suit, rank));
//                }
//            }
//        }

//        public void Shuffle()
//        {
//            // Fisher-Yates shuffle
//            Random rng = new Random();
//            int n = cards.Count;
//            while (n > 1)
//            {
//                n--;
//                int k = rng.Next(n + 1);
//                Card card = cards[k];
//                cards[k] = cards[n];
//                cards[n] = card;
//            }
//        }

//        public void DisplayCards()
//        {
//            foreach (Card card in cards)
//            {
//                WriteLine(card);  // Calls the ToString method of the Card class
//            }
//        }

//        public Card DrawCard()
//        {
//            if (cards.Count == 0)
//            {
//                throw new InvalidOperationException("The deck is empty.");
//            }

//            Card topCard = cards[cards.Count - 1];
//            cards.RemoveAt(cards.Count - 1);
//            return topCard;
//        }
//    }

//    class Hand
//    {
//        private List<Card> cards;

//        public Hand(List<Card> cards)
//        {
//            this.cards = cards;
//        }

//        public void SortByRank()
//        {
//            cards.Sort((a, b) => a.Rank.CompareTo(b.Rank));
//        }

//        public void PrintResult()
//        {
//            // Count the occurrences of each rank
//            Dictionary<Rank, int> counts = new Dictionary<Rank, int>();
//            foreach (Card card in cards)
//            {
//                if (counts.ContainsKey(card.Rank))
//                {
//                    counts[card.Rank]++;
//                }
//                else
//                {
//                    counts[card.Rank] = 1;
//                }
//            }

//            // Check for the highest-ranking combination of cards
//            int maxCount = counts.Values.Max();
//            if (maxCount == 4)
//            {
//                WriteLine("Four of a kind!");
//            }
//            else if (maxCount == 3)
//            {
//                WriteLine("Three of a kind!");
//            }
//            else if (maxCount == 2)
//            {
//                WriteLine("Pair!");
//            }
//            else
//            {
//                WriteLine("No combination.");
//            }
//        }
//    }

//    class Program
//    {
//        static void Main(string[] args)
//        {
//            WriteLine("Enter the number of players:");
//            string input = ReadLine();
//            int numPlayers = Int32.Parse(input);

//            List<Player> players = new List<Player>();
//            for (int i = 1; i <= numPlayers; i++)
//            {
//                players.Add(new Player("Player " + i));
//            }
//        }
//    }

//    class Player
//    {
//        public string Name { get; private set; }
//        private List<Card> cards;

//        public Player(string name)
//        {
//            Name = name;
//            cards = new List<Card>();
//        }

//        public void AddCard(Card card)
//        {
//            cards.Add(card);
//        }

//        public void DisplayCards()
//        {
//            WriteLine(Name+ " a ces carte : ");
//            foreach (Card card in cards)
//            {
//                WriteLine(card);  // Calls the ToString method of the Card class
//            }
//        }

//        public void CheckHand()
//        {
//            Dictionary<Rank, int> rankCounts = new Dictionary<Rank, int>();

//            foreach (Card card in cards)
//            {
//                if (rankCounts.ContainsKey(card.Rank))
//                {
//                    rankCounts[card.Rank]++;
//                }
//                else
//                {
//                    rankCounts[card.Rank] = 1;
//                }
//            }

//            foreach (KeyValuePair<Rank, int> entry in rankCounts)
//            {
//                if (entry.Value == 2)
//                {
//                    WriteLine("Pair of " + entry.Key + "s");
//                }
//            }
//        }
//    }

//    class Dealer
//    {
//        public string Name /*= "Dealer";*/{ get; private set; }
//        private List<Card> cards;

//        public Dealer(string name)
//        {
//            Name = name;
//            cards = new List<Card>();
//        }

//        public void AddCard(Card card)
//        {
//            cards.Add(card);
//        }

//        public void DisplayCards()
//        {
//            WriteLine("(Dealer) " + Name + " a ces carte : ");

//            foreach (Card card in cards)
//            {
//                WriteLine(card);  // Calls the ToString method of the Card class
//            }
//        }
//    }

//    class Game
//    {
//        private Player players;
//        private Dealer dealer;
//        private Deck deck;

//        public Game(Player players, Dealer dealer,Deck deck)
//        {
//            this.players = players;
//            this.dealer = dealer;
//            this.deck = deck;
//        }

//        public void DealCards()
//        {
//            //foreach (Player player in players)
//            //{
//                players.AddCard(deck.DrawCard());
//                players.AddCard(deck.DrawCard());
//            //}
//            dealer.AddCard(deck.DrawCard());
//            dealer.AddCard(deck.DrawCard());
//            dealer.AddCard(deck.DrawCard());
//            dealer.AddCard(deck.DrawCard());
//            dealer.AddCard(deck.DrawCard());
//        }
//    }
//}

