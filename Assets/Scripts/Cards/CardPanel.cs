using System;
using Custom;
using Data;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public class CardPanel : MonoBehaviour
    {
        [SerializeField] private Transform back;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image typeImage;
        [SerializeField] private TextMeshProUGUI numberText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private Card card;


        public CardDetails Details { get; private set; }

        public bool IsRevealed
        {
            get { return !back.gameObject.activeSelf; }
        }

        public Transform Root
        {
            get { return transform.parent; }
        }

        public Card Card
        {
            get { return card; }
        }

        private void Awake()
        {
            Details = new CardDetails();
        }

        public void SetCard(Card newCard)
        {
            if (newCard == null) return;
            cardImage.sprite = newCard.sprite;
            numberText.SetText(FormatNumber(newCard.number));
            titleText.SetText(newCard.title);
            subtitleText.SetText(newCard.subtitle);
            card = newCard;
            card.Panel = this;
        }

        private static string FormatNumber(int number)
        {
            return ((number > 6 ? number + 3 : number + 1)).ToString();
        }

        public void SetRevealed(bool isRevealed)
        {
            Utils.SetUniqueChildVisible(back, !isRevealed);
        }

        public void SetBackground(Color color)
        {
            backgroundImage.color = color;
        }

        public void OnClick()
        {
            if (Details.Player) Details.Player.PlayCard(this);
        }

        public void OnLongClick()
        {
            CardManager.Instance.ShowPopup(this);
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", card.type, card.number);
        }
    }

    public class CardDetails
    {
        public CardPlayer Player;
        public int Index;
        public int Turn;
    }
}