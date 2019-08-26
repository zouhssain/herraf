using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Custom;
using Data;
using UnityEngine;

namespace Game
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private RectTransform canvas;
        [SerializeField] private CardStock cardStock;
        [SerializeField] private Transform cardGroup;
        [SerializeField] private Transform tempGroup;
        [SerializeField] private Transform binGroup;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private float animationDuration;
        [SerializeField] private float dealGapDuration;
        [SerializeField] private float previewScale = .4f;
        [SerializeField] private Vector3 previewOffset;
        [SerializeField] private GameObject previewPopup;
        [SerializeField] private CardPanel previewCard;
        [SerializeField] private GameObject previewButton;
        private CardPanel _previewCardPanel;
        public static CardManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public IEnumerator AddCard(CardPanel cardPanel)
        {
            Transform placeholder = null;
            foreach (Transform child in cardGroup)
            {
                if (child.childCount != 0) continue;
                placeholder = child;
                break;
            }
            yield return DealCard(cardPanel, cardGroup, placeholder);
        }

        private IEnumerator DealCard(CardPanel cardPanel, Transform parent, Transform placeholder = null)
        {
            yield return null;
            var card = cardPanel.transform;
            var position = card.transform.position;
            cardPanel.Root.SetParent(parent, false);
            if (placeholder)
            {
                cardPanel.Root.SetSiblingIndex(placeholder.GetSiblingIndex());
                Destroy(placeholder.gameObject);
            }

            cardPanel.Root.localPosition = Vector3.zero;
            cardPanel.gameObject.SetActive(false);
            yield return null;
            cardPanel.gameObject.SetActive(true);
            card.localPosition = card.InverseTransformPoint(position);
            yield return TranslateLocal(card, Vector3.zero);
        }

        public IEnumerator TranslateCard(CardPanel newPanel, CardPanel oldPanel)
        {
            newPanel.Root.SetParent(tempGroup, true);
            var offset = Vector3.Scale(previewOffset, canvas.localScale);
            yield return TranslateCard(newPanel, oldPanel.Root.position + offset);
            SoundManager.Instance.PlayClip(SoundManager.Instance.CardFriction);
        }

        private IEnumerator TranslateCard(CardPanel cardPanel, Vector3 worldPosition)
        {
            yield return TranslateWorld(cardPanel.Root, worldPosition);
        }

        private IEnumerator TranslateWorld(Transform card, Vector3 worldPosition)
        {
            yield return TranslateLocal(card, card.parent.InverseTransformPoint(worldPosition));
        }

        private IEnumerator TranslateLocal(Transform card, Vector3 localPosition)
        {
            yield return Utils.Translate(card, animationDuration, animationCurve,
                card.localPosition, localPosition);
        }

        public CardPanel[] TakeCard(Transform parent, Card[] cards, bool isRevealed = false)
        {
            if (cards == null || cards.Length < 1) return null;
            foreach (var card in cards)
            {
                var cardPanel = Instantiate(cardPrefab, canvas).GetComponentInChildren<CardPanel>();
                cardPanel.SetCard(card);
                cardPanel.SetRevealed(isRevealed);
                cardPanel.Root.position = cardStock.transform.position;
                cardPanel.gameObject.SetActive(false);
            }

            var cardPanels = cards.Select(card => card.Panel).ToArray();
            StartCoroutine(DealCards(parent, cardPanels));
            return cardPanels;
        }

        private IEnumerator DealCards(Transform parent, IEnumerable<CardPanel> cardPanels)
        {
            foreach (var cardPanel in cardPanels)
            {
                yield return new WaitForSeconds(dealGapDuration);
                SoundManager.Instance.PlayClip(SoundManager.Instance.CardPlay);
                cardPanel.gameObject.SetActive(true);
                StartCoroutine(DealCard(cardPanel, parent));
            }
        }

        public IEnumerator WinCards(CardPlayer player, params CardPanel[] cardPanels)
        {
            if (cardPanels == null || cardPanels.Length == 0) yield break;
            for (var i = cardPanels.Length - 1; i > 0; i--) HoldPlace(cardPanels[i]);

            yield return null;
            var offset = Vector3.Scale(previewOffset, canvas.localScale);
            if (cardPanels.Length > 1) cardPanels[0].Root.SetParent(cardPanels[1].Root);
            
            for (var i = 1; i < cardPanels.Length - 1; i++)
            {
                yield return TranslateWorld(cardPanels[i].Root, cardPanels[i + 1].Root.position + offset);
                cardPanels[i].Root.SetParent(cardPanels[i+1].Root);
                SoundManager.Instance.PlayClip(SoundManager.Instance.CardFriction);
            }

            StartCoroutine(DeleteCards(player.Pivot, cardPanels));
        }
        
//        public IEnumerator WinCards(CardPlayer player, CardPanel cardPanel, params CardPanel[] cardPanels)
//        {
//            if (cardPanel == null || cardPanels == null) yield break;
//            for (var i = cardPanels.Length - 1; i > 0; i--) HoldPlace(cardPanels[i]);
//
//            yield return null;
//            var offset = Vector3.Scale(previewOffset, canvas.localScale);
//            if (cardPanels.Length > 0) cardPanel.Root.SetParent(cardPanels[0].Root);
//            
//            for (var i = 0; i < cardPanels.Length - 1; i++)
//            {
//                yield return TranslateWorld(cardPanels[i].Root, cardPanels[i + 1].Root.position + offset);
//                cardPanels[i].Root.SetParent(cardPanels[i+1].Root);
//                SoundManager.Instance.PlayClip(SoundManager.Instance.CardFriction);
//            }
//
//            StartCoroutine(DeleteCards(player.Pivot, cardPanels));
//            GameManager.Instance.CardsWon(player, cardPanels);
//        }

        public void HoldPlace(CardPanel cardPanel)
        {
            var placeHolder = new GameObject(cardPanel.ToString(), typeof(RectTransform));
            placeHolder.transform.SetParent(cardPanel.Root.parent);
            placeHolder.transform.SetSiblingIndex(cardPanel.Root.GetSiblingIndex());
            cardPanel.Root.SetParent(tempGroup);
        }

        private IEnumerator DeleteCards(Vector3 endPosition, params CardPanel[] cardPanels)
        {
            if (cardPanels == null || cardPanels.Length == 0) yield break;
            for (var i = 0; i < cardPanels.Length - 1; i++) 
                StartCoroutine(TranslateLocal(cardPanels[i].Root, Vector3.zero));
            SoundManager.Instance.PlayClip(SoundManager.Instance.CardStock);
            yield return TranslateCard(cardPanels[cardPanels.Length - 1], endPosition);
            foreach (var cardPanel in cardPanels) cardPanel.Root.SetParent(binGroup);
            yield return null;
        }

        public IEnumerator TurnCard(CardPanel cardPanel)
        {
            var card = cardPanel.transform;
            var angles = card.eulerAngles;
            yield return Utils.Rotate(card, animationDuration / 2, animationCurve, angles, angles + Vector3.up * 90);
            cardPanel.SetRevealed(!cardPanel.IsRevealed);
            yield return Utils.Rotate(card, animationDuration / 2, animationCurve, card.localEulerAngles, Vector3.zero);
        }

        public void ShowPopup(CardPanel cardPanel)
        {
            if (cardPanel == null) return;
            previewButton.SetActive(cardPanel.Root.parent != cardGroup);
            if (previewButton.activeSelf)
                previewButton.SetActive(TurnManager.Instance.IsPlayerTurn(cardPanel.Details.Player));
            previewPopup.SetActive(true);
            previewCard.SetCard(cardPanel.Card);
            _previewCardPanel = cardPanel;
            StartCoroutine(Utils.Scale(previewCard.transform, animationDuration, animationCurve,
                Vector3.one * previewScale, Vector3.one));
            if (previewButton.activeSelf)
                StartCoroutine(Utils.Scale(previewButton.transform, animationDuration,
                    animationCurve, Vector3.one * previewScale, Vector3.one));
        }

        private void HidePopup()
        {
            previewPopup.SetActive(false);
            _previewCardPanel = null;
        }

        private void PlayPopupCard()
        {
            _previewCardPanel.OnClick();
            HidePopup();
        }

        public CardPanel[] GetCards()
        {
            return cardGroup.GetComponentsInChildren<CardPanel>();
        }
        
        public void Display(Transform child)
        {
            Utils.SetUniqueChildVisible(child);
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}