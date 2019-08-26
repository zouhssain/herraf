using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using Cards;
using Custom;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour, IGameManager
    {
        public static IGameManager Instance { get; private set; }
        [SerializeField] private QuizManager quizManager;
        [SerializeField] private GameObject specialPanel;
        [SerializeField] private CardPanel newCardPanel;
        [SerializeField] private CardPanel oldCardPanel;
        [SerializeField] private Transform actionsPanel;
        [SerializeField] private int maxMoves;

        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private float animationDuration;
        [SerializeField] private float startScale = .4f;
        
        private Dictionary<CardPlayer, int> _countDictionary;
        private CardPlayer _lastWinner;
        private bool _isWaiting;
        private bool _isHerraf;
        private bool _isCompleted;

        private void Start()
        {
            Instance = this;
            _countDictionary = new Dictionary<CardPlayer, int>();
        }

        public IEnumerator CardPlayed(CardPanel newPanel)
        {
            var oldPanel = CardManager.Instance.GetCards()
                .Where(panel => panel.Card.number == newPanel.Card.number)
                .OrderBy(panel => panel.Details.Index)
                .FirstOrDefault();
            if (oldPanel)
            {
                var number = newPanel.Card.number;
                var groups = CardManager.Instance.GetCards().Where(panel => panel.Card.number >= number)
                    .OrderBy(panel => panel.Card.number)
                    .GroupBy(panel => panel.Card.number)
                    .TakeWhile(group => group.First().Card.number == number++);
                var cards = groups.Select(group => group.First()).ToList();
                cards.Insert(0, newPanel);
                var list = cards.ToArray();

                ColorManager.Instance.SetCardFocused(list);
                ColorManager.Instance.SetCardPlayed(newPanel);

                yield return CardManager.Instance.TranslateCard(newPanel, oldPanel);

                StartNewMove();
                if (oldPanel.Details.Turn == newPanel.Details.Turn &&
                    oldPanel.Details.Index == newPanel.Details.Index - 1)
                    yield return SpecialMove(newPanel, oldPanel);
                else SetCompleted(false);

                if (!_isCompleted)
                {
                    _lastWinner = newPanel.Details.Player;
                    yield return CardManager.Instance.WinCards(_lastWinner, list);
                    yield break;
                }

                ColorManager.Instance.SetCardDefault(list);
            }

            yield return CardManager.Instance.AddCard(newPanel);
        }
        
        public void CardsWon(CardPlayer player, params CardPanel[] cardPanels)
        {
            player.ReportScore(cardPanels.Length);
        }

        public void GameFinished(CardPlayer winner, CardPanel[] remainedCards)
        {
            StartCoroutine(GameFinished(_lastWinner, winner, remainedCards));

        }

        private static IEnumerator GameFinished(CardPlayer lastWinner, CardPlayer winner, CardPanel[] remainedCards)
        {
            if(remainedCards.Length > 1) yield return CardManager.Instance.TranslateCard(remainedCards[0], remainedCards[1]);
            yield return CardManager.Instance.WinCards(lastWinner, remainedCards);
            Debug.LogFormat("Winner : {0}, Score : {1}", winner.Name, winner.Score);
        }

        private IEnumerator SpecialMove(CardPanel newPanel, CardPanel oldPanel)
        {
            if (_countDictionary.ContainsKey(oldPanel.Details.Player) && _countDictionary[oldPanel.Details.Player] < maxMoves)
            {
                Debug.LogWarning("Max moves reached");
                SetCompleted(false);
                yield break;
            }
            
            specialPanel.SetActive(true);
            
            oldCardPanel.gameObject.SetActive(false);
            newCardPanel.gameObject.SetActive(false);
            actionsPanel.gameObject.SetActive(false);

            oldCardPanel.SetCard(oldPanel.Card);
            newCardPanel.SetCard(newPanel.Card);
            StartCoroutine(Utils.Scale(oldCardPanel.transform, animationDuration, animationCurve,
                Vector3.one * startScale, Vector3.one));
            yield return null;
            oldCardPanel.gameObject.SetActive(true);

            yield return new WaitForSeconds(.2f);
            StartCoroutine(Utils.Scale(newCardPanel.transform, animationDuration, animationCurve,
                Vector3.one * startScale, Vector3.one));
            yield return null;
            newCardPanel.gameObject.SetActive(true);

            yield return new WaitForSeconds(.2f);
            StartCoroutine(Utils.Scale(actionsPanel.transform, animationDuration, animationCurve,
                Vector3.one * startScale, Vector3.one));
            yield return null;
            actionsPanel.gameObject.SetActive(true);

            yield return new WaitWhile(() => _isWaiting && !_isHerraf);
            if(_isHerraf) yield return quizManager.StartQuiz(newPanel.Card, SetCompleted);
            if (_isCompleted)
            {
                if (!_countDictionary.ContainsKey(oldPanel.Details.Player)) _countDictionary.Add(oldPanel.Details.Player, 0);
                    _countDictionary[oldPanel.Details.Player]++;
            } 

            specialPanel.SetActive(false);
        }

        private void StartNewMove()
        {
            _isWaiting = true;
            _isHerraf = false;
        }
        
        public void SetHerraf()
        {
            _isHerraf = true;
        }

        public void SetCompleted(bool isCompleted)
        {
            _isWaiting = false;
            _isCompleted = isCompleted;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}