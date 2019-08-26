using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Custom;
using Game;
using UI;
using UnityEngine;

namespace Multiplayer
{
    public class LocalManager : TurnManager
    {
        [SerializeField] private int _initialStock;
        [SerializeField] private int _defaultCards;
        [SerializeField] private Vector2Int _cardsPerTurns;
        [SerializeField] private SliderText _playersSlider;
        [SerializeField] private GameObject _computerPrefab;

        private List<CardPlayer> _players;
        private CardPlayer CurrentPlayer { get { return _players[_currentPlayer]; }}
        private int CardsPlayed { get; set; }
        private int Index { get; set; }
        private GameState State { get; set; }
        private int _currentPlayer;

        public void NewGame()
        {
            if (PlayersParent.childCount == 1) Instantiate(MainPlayerPrefab, PlayersParent).transform.SetSiblingIndex(0);
            
            var computersCount = Mathf.RoundToInt(_playersSlider.Value - 1);
            for (var i = computersCount ; i < ExtraPlayersParent.childCount; i++)
                Destroy(ExtraPlayersParent.GetChild(i).gameObject);
            for (var i = ExtraPlayersParent.childCount ; i < computersCount; i++)
                Instantiate(_computerPrefab, ExtraPlayersParent);

            StartCoroutine(LateNewGame());
        }

        private IEnumerator LateNewGame()
        {
            yield return null;
            _players = new List<CardPlayer>(PlayersParent.GetComponentsInChildren<CardPlayer>());
            foreach (var cardPlayer in _players) cardPlayer.SetScore(0);
            if (_players.Count == 0) yield break;
            
            CurrentPlayer.SetFocused(true);
            CardStock.Clear();
            CardStock.AddRange(Cards.Take(_initialStock));
            CardStock.Shuffle();

            Utils.SetChildrenVisible(transform, false);
            Deal(_defaultCards);
        }
        
        private void Deal(int count)
        {
            if (CardStock.Count < count * _players.Count) count = CardStock.Count / _players.Count;
            if (count == 0) StartCoroutine(EndGame());
            else foreach (var player in _players) DealCards(player, CardStock.Take(count));
        }

        public override void PlayCard( CardPanel cardPanel)
        {
            if (!CanPlayCard(cardPanel)) return;
            cardPanel.Details.Player.RemoveCard(cardPanel);
            if (!cardPanel.IsRevealed) StartCoroutine(CardManager.Instance.TurnCard(cardPanel));
            StartCoroutine(PlayCardTurn(cardPanel));
        }
        
        private IEnumerator PlayCardTurn(CardPanel cardPanel)
        {
            if (State == GameState.Playing) yield break;
            SoundManager.Instance.PlayClip(SoundManager.Instance.CardPlay);
            State = GameState.Playing;
            cardPanel.Details.Index = ++Index;
            cardPanel.Details.Turn = Turn;
            yield return GameManager.Instance.CardPlayed(cardPanel);
            State = GameState.Waiting;
            CardsPlayed++;
            
            if (_cardsPerTurns.y > 0 && CardsPlayed >= _cardsPerTurns.y) PassPlayerTurn();
        }

        private bool CanPlayCard( CardPanel cardPanel)
        {
            var player = cardPanel.Details.Player;
            return cardPanel.Root.IsChildOf(player.Hand) && IsPlayerTurn(player) && State != GameState.Playing;
        }

        private void PassPlayerTurn()
        {
            CardsPlayed = 0;
            CurrentPlayer.SetFocused(false);
            _currentPlayer = (_currentPlayer + 1) % _players.Count;
            CurrentPlayer.SetFocused(true);
            if (_currentPlayer == 0) PassGameTurn();
        }

        private void PassGameTurn()
        {
            Turn++;
            if (_defaultCards > 0 && _players.TrueForAll(player => player.IsHandEmpty())) Deal(_defaultCards);
        }

        public override bool IsPlayerTurn(CardPlayer cardPlayer)
        {
            return cardPlayer == CurrentPlayer;
        }

        private IEnumerator EndGame()
        {
            yield return GameManager.Instance.CardsRemained(CardManager.Instance.GetCards());
            var players = _players.OrderByDescending(player => player.Score).ToList();
            GameManager.Instance.GameFinished(players[0]);
            ResultManager.SetResult(players);
        }
    }
}