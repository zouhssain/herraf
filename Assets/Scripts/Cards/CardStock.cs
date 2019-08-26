using System.Collections.Generic;
using System.Linq;
using Custom;
using Data;
using TMPro;
using UnityEngine;

namespace Cards
{
    public class CardStock : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private bool hideOnEmpty;

        private List<Card> _cards;

        public int Count
        {
            get { return _cards.Count; }
        }

        private void Awake()
        {
            _cards = new List<Card>();
        }

        public void AddRange(IEnumerable<Card> cards)
        {
            _cards.AddRange(cards);
            UpdateCount();
        }

        public void Remove(params Card[] cards)
        {
            _cards = _cards.Except(cards).ToList();
            UpdateCount();
        }

        public void Clear()
        {
            _cards.Clear();
            UpdateCount();
        }

        public void Shuffle()
        {
            _cards.Shuffle();
        }

        private void UpdateCount()
        {
            SetStockCount(_cards.Count);
        }

        public Card[] Take(int count)
        {
            var cards = _cards.Take(count).ToArray();
            Remove(cards);
            return cards;
        }

        public void SetStockCount(int count)
        {
            countText.SetText(count.ToString());
            if(hideOnEmpty) Utils.SetChildrenVisible(transform, count > 0);
        }
    }
}