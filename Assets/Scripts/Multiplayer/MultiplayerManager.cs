using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Data;
using Game;
using PlayerIOClient;
using Services;
using UnityEngine;

namespace Multiplayer
{
    public class MultiplayerManager : TurnManager
    {
        [SerializeField] private GameObject _extraPlayerPrefab;
        [SerializeField] private RoomManager _roomManager;
        private Dictionary<string, CardPlayer> _players;
        private string _currentPlayer;
        private int _stock;

        public void GotMessage(Message message)
        {
            switch (message.Type)
            {
                case "Setup":
                    var playerIds = new string[message.Count / 2];
                    var playerNames = new string[playerIds.Length];
                    for (var i = 0; i < playerIds.Length; i++)
                    {
                        playerIds[i] = message.GetString((uint) i);
                        playerNames[i] = message.GetString((uint) (playerNames.Length + i));
                    }
                    Setup(playerIds, playerNames);
                    break;
                case "Deal":
                    var cardIds = new int[message.Count - 1];
                    for (var i = 0; i < cardIds.Length; i++) cardIds[i] = message.GetInt((uint) (i + 1));
                    DealCards(message.GetString(0), cardIds);
                    break;
                case "PlayCard":
                    PlayCard(message.GetString(1), message.GetInt(0));
                    break;
                case "WinCard":
                    var wonCardsIds = new int[message.Count - 3];
                    for (var i = 0; i < wonCardsIds.Length; i++) wonCardsIds[i] = message.GetInt((uint) (i + 3));
                    WinCard(message.GetString(0), message.GetString(1), message.GetInt(2), wonCardsIds);
                    break;
                case "PassTurn":
                    SetPlayerTurn(message.GetInt(0), message.GetString(1));
                    break;
                case "SpecialMove":
                    SpecialMove(message);
                    break;
                case "Score":
                    UpdateScore(message);
                    break;
                case "Finish":
                    EndGame(message);
                    break;
            }
        }

        private void UpdateScore(Message message)
        {
            var count = message.Count / 2;
            for (uint i = 0; i < count; i++)
            {
                var playerId = message.GetString(2 * i);
                if (PlayerExists(playerId)) _players[playerId].SetScore(message.GetInt(2 * i + 1));
            }
        }

        public void Setup(string[] playerIds, string[] playerNames)
        {
            _stock = Cards.Count();
            _players = new Dictionary<string, CardPlayer>(playerIds.Length);
            foreach (var cardPlayer in PlayersParent.GetComponentsInChildren<CardPlayer>()) 
                Destroy(cardPlayer.gameObject);

            for (var i = 0; i < playerIds.Length; i++)
            {
                var playerId = playerIds[i];
                var prefab = ServiceManager.User.Id.Equals(playerId) ? MainPlayerPrefab : _extraPlayerPrefab;
                var parent = ServiceManager.User.Id.Equals(playerId) ? PlayersParent : ExtraPlayersParent;
                var cardPlayer = Instantiate(prefab, parent).GetComponent<CardPlayer>();
                cardPlayer.Name = playerNames[i];
                cardPlayer.Id = playerId;
                _players.Add(playerId, cardPlayer);
            }
        }

        public override void PlayCard(CardPanel cardPanel)
        {
            Debug.Log("Play Card : " + cardPanel.Card);
            if (cardPanel.Details.Player.Id.Equals(_currentPlayer))
                _roomManager.SendMessage(Message.Create("PlayCard", GetCard(cardPanel.Card)));
        }

        public override bool IsPlayerTurn(CardPlayer cardPlayer)
        {
            return cardPlayer.Id.Equals(_currentPlayer);
        }

        private void EndGame(Message message)
        {
            UpdateScore(message);
            ResultManager.SetResult(_players.Select(pair => pair.Value).OrderByDescending(player => player.Score).ToList());
        }

        private void PlayCard(string playerId, int cardId)
        {
            if (!PlayerExists(playerId)) return;

            var card = GetCard(cardId);
            Debug.Log(card);
            var player = _players[playerId];
            var cardPanel = player.Hand.GetComponentsInChildren<CardPanel>()
                .FirstOrDefault(panel => panel.Card == card);

            if (!cardPanel) return;
            cardPanel.Details.Player.RemoveCard(cardPanel);
            if (!cardPanel.IsRevealed) StartCoroutine(CardManager.Instance.TurnCard(cardPanel));
            StartCoroutine(CardManager.Instance.AddCard(cardPanel));
        }

        private void WinCard(string playerId, string otherPlayerId, int cardId, params int[] wonCardsIds)
        {
            if (!PlayerExists(playerId)) return;

            CardPanel cardPanel = null;
            var winner = _players[playerId];
            
            if (cardId >= 0)
            {
                var card = GetCard(cardId);
                cardPanel = winner.Hand.GetComponentsInChildren<CardPanel>()
                    .FirstOrDefault(panel => panel.Card == card);
            }
           
            var wonCards = wonCardsIds.Select(GetCard).ToList();
            var wonPanels = CardManager.Instance.GetCards().Where(panel => wonCards.Contains(panel.Card))
                .OrderBy(panel => wonCards.IndexOf(panel.Card)).ToArray();
            StartCoroutine(WinCard(winner, otherPlayerId, cardPanel, wonPanels));
        }

        private IEnumerator WinCard(CardPlayer winner, string playerId, CardPanel cardPanel, CardPanel[] wonPanels)
        {
            if (cardPanel == null)
            {
                if (wonPanels.Length > 1)
                {
                    CardManager.Instance.HoldPlace(cardPanel = wonPanels[0]);
                    yield return CardManager.Instance.TranslateCard(cardPanel, wonPanels[1]);
                }
            }
            else
            {
                ColorManager.Instance.SetCardFocused(wonPanels);
                ColorManager.Instance.SetCardPlayed(cardPanel);
                cardPanel.Details.Player.RemoveCard(cardPanel);
                if (!cardPanel.IsRevealed) StartCoroutine(CardManager.Instance.TurnCard(cardPanel));
                if (wonPanels.Length > 0) yield return CardManager.Instance.TranslateCard(cardPanel, wonPanels[0]);
            }
            

            if (PlayerExists(playerId)) Debug.Log(playerId + " - - - - - -");
            var list = wonPanels.ToList();
            list.Insert(0, cardPanel);
            yield return CardManager.Instance.WinCards(winner, list.ToArray());
        }

        private bool PlayerExists(string playerId)
        {
            if (!string.IsNullOrEmpty(playerId) && _players.ContainsKey(playerId)) return true;
            Debug.LogWarning("Player not found : " + playerId);
            return false;
        }

        private void SpecialMove(Message message )
        {
//            var playerId = message.GetString(0);
//            var cardId = message.GetInt(1);
            var otherPlayerId = message.GetString(2);
//            var matchId = message.GetInt(1);
            if (PlayerExists(otherPlayerId))
            {
                if (_players[otherPlayerId] is CardMainPlayer)
                {
                    StartCoroutine(GameManager.Instance.AsyncMove(null, null, isCompleted =>
                    {
                        _roomManager.SendMessage(Message.Create("SpecialMove", isCompleted));
                    }));
                }
            }
            else _roomManager.SendMessage(Message.Create("SpecialMove", false));
        }


        public void SetPlayerTurn(int turn, string playerId)
        {
            Turn = turn;
            SetPlayerFocused(_currentPlayer, false);
            _currentPlayer = playerId;
            SetPlayerFocused(_currentPlayer, true);
        }

        public void SetPlayerFocused(string playerId, bool isFocused)
        {
            if (!PlayerExists(playerId)) return;
            _players[playerId].SetFocused(isFocused);
        }

        public void DealCards(string playerId, params int[] cardIds)
        {
            if (!PlayerExists(playerId)) return;
            var cards = cardIds.Select(GetCard).Where(card => card != null).ToArray();
            DealCards(_players[playerId], cards);
            CardStock.SetStockCount(_stock -= cards.Length);
        }

        private Card GetCard(int cardId)
        {
            var number = cardId % 10;
            var type = cardId / 10;
            return Cards.FirstOrDefault(card => card.number == number && (int) card.type == type);
        }


        private static int GetCard(Card card)
        {
            return card.number + (int) card.type * 10;
        }
    }
}