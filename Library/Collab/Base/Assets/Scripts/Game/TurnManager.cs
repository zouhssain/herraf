using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Data;
using UnityEngine;

namespace Game
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        [SerializeField] private CardData data;
        [SerializeField] private CardStock cardStock;
        [SerializeField] private Transform playersParent;
        [SerializeField] private int defaultCards;
        [SerializeField] private Vector2Int cardsPerTurns;


        private List<CardPlayer> _players;
        private int _currentPlayer;
        private int CardsPlayed { get; set; }
        private int Turn { get; set; }
        private int Index { get; set; }

        private IEnumerable<Card> Cards  { get { return data.cards; } }
        private CardPlayer CurrentPlayer { get { return _players[_currentPlayer]; } }
        public CardPlayer Winner { get { return _players.OrderByDescending(player => player.Score).First(); } }
        public GameState State { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _players = new List<CardPlayer>(playersParent.GetComponentsInChildren<CardPlayer>());
            if (_players.Count > 0) CurrentPlayer.SetFocused(true);
            NewGame();
        }

        private void NewGame()
        {
            cardStock.Clear();
            cardStock.AddRange(Cards);
            cardStock.Shuffle();
            Deal(defaultCards);
        }

        private IEnumerator Play(CardPanel cardPanel)
        {
            if (State == GameState.Playing) yield break;
            SoundManager.Instance.PlayClip(SoundManager.Instance.CardPlay);
            State = GameState.Playing;
            cardPanel.Details.Index = ++Index;
            cardPanel.Details.Turn = Turn;
            yield return GameManager.Instance.CardPlayed(cardPanel);
            State = GameState.Waiting;
            CardsPlayed++;
        }

        private void PassPlayerTurn()
        {
            CardsPlayed = 0;
            CurrentPlayer.SetFocused(false);
            _currentPlayer = (_currentPlayer + 1) % _players.Count;
            CurrentPlayer.SetFocused(true);
            if (_currentPlayer == 0) PassGameTurn();
        }

        public Card[] TakeCard(int count)
        {
            return cardStock.Take(count);
        }

        public void PlayCard(CardPanel cardPanel)
        {
            if (!cardPanel.IsRevealed) StartCoroutine(CardManager.Instance.TurnCard(cardPanel));
            StartCoroutine(PlayTurn(cardPanel));
        }

        private IEnumerator PlayTurn(CardPanel cardPanel)
        {
            yield return Play(cardPanel);
            if (cardsPerTurns.y > 0 && CardsPlayed >= cardsPerTurns.y) PassPlayerTurn();
        }

        private void PassGameTurn()
        {
            Turn++;
            if (defaultCards > 0 && _players.TrueForAll(player => player.IsHandEmpty())) Deal(defaultCards);
        }

        private void Deal(int count)
        {
            if (cardStock.Count < count * _players.Count) count = cardStock.Count / _players.Count;
            if (count < _players.Count) EndGame();
            else foreach (var player in _players) player.Deal(count);
        }

        private void EndGame()
        {
            cardStock.Clear();
            GameManager.Instance.GameFinished(CardManager.Instance.GetCards());
        }

        public bool IsPlayerTurn(CardPlayer cardPlayer)
        {
            return cardPlayer == CurrentPlayer;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }

    public enum GameState
    {
        Waiting,
        Playing,
    }
}