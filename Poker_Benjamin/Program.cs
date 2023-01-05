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
    int? numPlayers = int.Parse(ReadLine().Trim());

   if(numPlayers == 0 || numPlayers == null){}
    else
    {
        for (int i = 0; i < numPlayers; i++)
        {
            Write("Enter name of player " + (i + 1) + ": ");
            string name = ReadLine().Trim();

            if (name == "")
            {
                name = "Player" + i++;
            }

            players.Add(new Player(name, "Dealer"));
        }


        Dealer dealer = new Dealer("Dealer");
        Game game = new Game(players, dealer);


        game.Start();
    }
    WriteLine("\n____________________\n | If u want to :\n | play again : 1\n | Quit : 2");
    j = int.Parse(ReadLine());

    Clear();
} while (j < 2 || j == 0);

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
            int Morebet = 0;
            int PlayerRestant;

            do
            {

                foreach (Player player in players)
                {
                    player.Haspass = false;
                }

                deck = new Deck();

                // Shuffle the deck
                deck.Shuffle();

                foreach(Player player in players)
                {
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

                        Morebet += player.Continue(player);
                    }
                    if (AllPlayersHavePassed() == false)
                    {
                        Clear();
                        // End the game
                        WriteLine("Tout les joueurs se sont coucher. la partie est fini.");
                        return;
                    }
                }

                //Boucle For retourne 5 carte
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

                            WriteLine("\nDealer has: ");
                            foreach (Card card in dealer.Cards)
                            {
                                WriteLine("  " + card);
                            }

                            Morebet += player.Continue(player);
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
                        WriteLine("" + player.Name + " has: ");
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
                    //comparer Result entre Player pour Résult

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

                List<Player> winners = DetermineWinners();

                ShowWinners(winners, Morebet);

                foreach (Player player in players)
                {
                    player.ClearHand();
                }

                dealer.ClearHand();

                RemovePlayersOutOfChips();
            
                CheckChips();

                PlayerRestant = IsGameOver();

                Morebet = 0;

            } while (players.Count > PlayerRestant);
           
        }

        public int IsGameOver()
        {
            int i = 0;
            foreach (Player player in players)
            {
                if (player.Chips <= 0)
                {
                    i++;
                }
            }
            return i;
        }

        public List<Player> DetermineWinners()
        {
            List<Player> winners = new List<Player>();
            int maxHandRank = 0;
            foreach (Player player in players)
            {
                if (player.CheckHand() > maxHandRank)
                {
                    maxHandRank = player.CheckHand();
                    winners.Clear();
                    winners.Add(player);
                }
                else if (player.CheckHand() == maxHandRank)
                {
                    winners.Add(player);
                }
            }
            return winners;
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
                WriteLine($"{player.Name} has {player.Chips} chips.\n\n");
            }
        }

        public void ShowWinners(List<Player> winners, int Morebet)
        {
            chipTT = 0;

            foreach (Player player in players)
            {
                chipTT += 20;

            }

            chipTT += (Morebet * 20);

            if (winners.Count == 1)
            {
                WriteLine($"The winner is {winners[0].Name}!");
                winners[0].AddChips(chipTT);
            }
            else
            {
                WriteLine("The winners are:");
                foreach (Player player in winners)
                {
                    WriteLine(player.Name);
                }
                int splitPot = chipTT / winners.Count;
                foreach (Player player in winners)
                {
                    player.AddChips(splitPot);
                }
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

        public int Continue(Player player)
        {
            int? Result;

            do
            {
                WriteLine("\nQue voulez vous faire?\n1 : Check\n2 : Bet More (20)\n3 : pass\n");

                CheckChipsIndividual(player);

                Result = Convert.ToInt32(ReadLine());
            } while (Result == 0 || Result > 3);

            switch (Result)
            {
                case (1):
                    Clear();

                    WriteLine(" " + player.Name + " Reste \n");
                    break;
                case (2):
                    Clear();
                    WriteLine(" " + player.Name + " est Réencherie \n");

                    RemoveChips(20);

                    return 1;

                case (3):

                    player.HasPass();

                    Clear();

                    WriteLine(" " + player.Name + " est Partie \n");

                    break;
            }
            return 0;
        }

        public void RemoveChips(int amount)
        {
            Chips -= amount;
        }

        public void AddChips(int amount)
        {
            Chips += amount;
        }

        public void ClearHand()
        {
            this.Cards.Clear();
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
