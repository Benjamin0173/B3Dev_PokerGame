using PokerGame;
using System.Collections.Generic;
using System.Net.Security;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml;
using static System.Console;

Deck deck = new Deck();
deck.Shuffle();
//deck.DisplayCards();




namespace PokerGame
{

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

            // Populate the deck with 52 cards
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
            // Fisher-Yates shuffle
            Random rng = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card card = cards[k];
                cards[k] = cards[n];
                cards[n] = card;
            }
        }

        public void DisplayCards()
        {
            foreach (Card card in cards)
            {
                Console.WriteLine(card);  // Calls the ToString method of the Card class
            }
        }
    }

    class Hand
    {
        private List<Card> cards;

        public Hand(List<Card> cards)
        {
            this.cards = cards;
        }

        public void SortByRank()
        {
            cards.Sort((a, b) => a.Rank.CompareTo(b.Rank));
        }

        public void PrintResult()
        {
            // Count the occurrences of each rank
            Dictionary<Rank, int> counts = new Dictionary<Rank, int>();
            foreach (Card card in cards)
            {
                if (counts.ContainsKey(card.Rank))
                {
                    counts[card.Rank]++;
                }
                else
                {
                    counts[card.Rank] = 1;
                }
            }

            // Check for the highest-ranking combination of cards
            int maxCount = counts.Values.Max();
            if (maxCount == 4)
            {
                Console.WriteLine("Four of a kind!");
            }
            else if (maxCount == 3)
            {
                Console.WriteLine("Three of a kind!");
            }
            else if (maxCount == 2)
            {
                Console.WriteLine("Pair!");
            }
            else
            {
                Console.WriteLine("No combination.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the number of players:");
            string input = Console.ReadLine();
            int numPlayers = Int32.Parse(input);

            List<Player> players = new List<Player>();
            for (int i = 1; i <= numPlayers; i++)
            {
                players.Add(new Player("Player " + i));
            }
        }
    }

}

