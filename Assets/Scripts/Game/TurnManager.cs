using System.Collections.Generic;
using Cards;
using Custom;
using Data;
using UnityEngine;

namespace Game
{
    public abstract class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        [SerializeField] private CardData _data;
        [SerializeField] protected ResultManager ResultManager;
        [SerializeField] protected Transform PlayersParent;
        [SerializeField] protected Transform ExtraPlayersParent;
        [SerializeField] protected GameObject MainPlayerPrefab;
        [SerializeField] protected CardStock CardStock;

        protected int Turn { get; set; }
        protected IEnumerable<Card> Cards { get { return _data.cards; }}

        protected void Start() { Instance = this; }
        public void Replay()
        {
            Utils.SetChildrenVisible(transform);
        }
        public abstract void PlayCard(CardPanel cardPanel);
        public abstract bool IsPlayerTurn(CardPlayer cardPlayer);
        
        protected static void DealCards(CardPlayer player, Card[] cards)
        {
            player.Deal(CardManager.Instance.TakeCard(player.Hand, cards, player.CardRevealed));
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