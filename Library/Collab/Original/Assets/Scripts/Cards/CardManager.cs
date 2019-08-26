using System.Collections;
using Custom;
using Data;
using Game;
using UnityEngine;

namespace Cards
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private CardData data;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform cardGroup;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private float animationDuration;
        [SerializeField] private Vector3 offset;
        [SerializeField] private GameObject previewPopup;
        [SerializeField] private CardPanel previewCard;
        [SerializeField] private float previewScale = .4f;
        public static CardManager Instance { get; private set; }

        private void Start()
        {
            Instance = this;
        }

        public IEnumerator PlayCard(CardPanel cardPanel)
        {
            var card = cardPanel.transform;
            var position = card.transform.position;
            cardPanel.Root.SetParent(cardGroup);
            if (!cardPanel.IsRevealed) StartCoroutine(TurnCard(cardPanel));
            cardPanel.gameObject.SetActive(false);
            yield return null;
            cardPanel.gameObject.SetActive(true);
            card.localPosition = card.InverseTransformPoint(position);
            yield return Utils.Translate(card, animationDuration, animationCurve, card.localPosition, Vector3.zero);
        }

        public IEnumerator DistributeCard(CardPanel cardPanel,Transform transform)
        {
            var card = cardPanel.transform;
            var position = card.transform.position;
            cardPanel.Root.SetParent(transform);
            if (!cardPanel.IsRevealed) StartCoroutine(TurnCard(cardPanel));
            cardPanel.gameObject.SetActive(false);
            yield return null;
            cardPanel.gameObject.SetActive(true);
            card.localPosition = card.InverseTransformPoint(position);
            yield return Utils.Translate(card, animationDuration, animationCurve, card.localPosition, Vector3.zero);
        }

        public Card TakeCard(Transform parent, bool isRevealed)
        {
            var card = TurnManager.Instance.Take();
            var cardPanel = Instantiate(cardPrefab, parent).GetComponentInChildren<CardPanel>();
            cardPanel.SetCard(card);
            cardPanel.SetRevealed(isRevealed);
            return card;
        }

        public IEnumerator PreviewCard(Transform card)
        {
            yield return Utils.Translate(card, animationDuration, animationCurve,
                card.localPosition, offset);
        }

        public IEnumerator ResetCard(Transform card)
        {
            yield return Utils.Translate(card, animationDuration, animationCurve,
                card.localPosition, Vector3.zero);
        }

        public IEnumerator TurnCard(CardPanel cardPanel)
        {
            var card = cardPanel.transform;
            yield return Utils.Rotate(card, animationDuration, animationCurve,
                card.localEulerAngles, card.localEulerAngles + Vector3.up * 90);
            cardPanel.SetRevealed(!cardPanel.IsRevealed);
            yield return Utils.Rotate(card, animationDuration, animationCurve,
                card.localEulerAngles, card.localEulerAngles + Vector3.up * 90);
        }

        public void ShowPopup(Card card)
        {
            if (card == null) return;
            StopAllCoroutines();
            previewPopup.SetActive(true);
            previewCard.SetCard(card);
            StartCoroutine(Utils.Scale(previewCard.transform, animationDuration, animationCurve,
                Vector3.one * previewScale,
                Vector3.one));
        }

        public void HidePopup()
        {
            StopAllCoroutines();
            previewPopup.SetActive(false);
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}