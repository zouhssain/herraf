using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using Cards;
using Custom;
using Data;
using Services;
using UnityEngine;
using UnityEngine.Events;

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
        
        [SerializeField] private GameObject multiplayerPanel;
        [SerializeField] private GameObject loggedOutPanel;
        [SerializeField] private GameObject localPanel;
        [SerializeField] private UnityEvent _onQuit;
        
        
        private Dictionary<CardPlayer, int> _countDictionary;
        private CardPlayer _lastWinner;
        private bool _isWaiting;
        private bool _isHerraf;
        private bool IsCompleted { get; set; }

        private void Awake()
        {
            multiplayerPanel.SetActive(Values.Multiplayer);
            localPanel.SetActive(!Values.Multiplayer);
            Destroy(localPanel.activeSelf ? multiplayerPanel : localPanel );
            if (multiplayerPanel.activeSelf)
            {
                loggedOutPanel.SetActive(!ServiceManager.IsLoggedIn);
            }
        }

        private void Start()
        {
            Instance = this;
            _countDictionary = new Dictionary<CardPlayer, int>();
        }

        public IEnumerator AsyncMove(CardPanel newPanel, CardPanel oldPanel, Action<bool> callback)
        {
            StartNewMove();
            yield return SpecialMove(newPanel, oldPanel);
            callback(IsCompleted);
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
                    oldPanel.Details.Index == newPanel.Details.Index - 1 &&
                    oldPanel.Details.Player is CardMainPlayer)
                {
                    yield return SpecialMove(newPanel, oldPanel);
                    if (IsCompleted)
                    {
                        if (!_countDictionary.ContainsKey(oldPanel.Details.Player)) _countDictionary.Add(oldPanel.Details.Player, 0);
                        _countDictionary[oldPanel.Details.Player]++;
                    } 
                }
                else SetCompleted(false);

                if (!IsCompleted)
                {
                    _lastWinner = newPanel.Details.Player;
                    yield return CardManager.Instance.WinCards(_lastWinner, list);
                    _lastWinner.ReportScore(list.Length);
                    yield break;
                }

                ColorManager.Instance.SetCardDefault(list);
            }

            yield return CardManager.Instance.AddCard(newPanel);
        }

        public IEnumerator CardsRemained(CardPanel[] remainedCards)
        {
            if (!_lastWinner) yield break;
            if (remainedCards.Length > 1)
            {
                CardManager.Instance.HoldPlace(remainedCards[0]);
                yield return CardManager.Instance.TranslateCard(remainedCards[0], remainedCards[1]);
            }
            yield return CardManager.Instance.WinCards(_lastWinner, remainedCards);
            _lastWinner.ReportScore(remainedCards.Length);
        }

        public void GameFinished(CardPlayer winner)
        {
            Debug.LogFormat("Winner : {0}, Score : {1}", winner.Name, winner.Score);
        }

        public void StartNewMove()
        {
            _isWaiting = true;
            _isHerraf = false;
        }

        private IEnumerator SpecialMove(CardPanel newPanel, CardPanel oldPanel)
        {
            if (_countDictionary.ContainsKey(oldPanel.Details.Player) && _countDictionary[oldPanel.Details.Player] > maxMoves)
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
            

            specialPanel.SetActive(false);
        }
        
        public void SetHerraf()
        {
            _isHerraf = true;
        }

        public void SetCompleted(bool isCompleted)
        {
            _isWaiting = false;
            IsCompleted = isCompleted;
        }

        public void Replay()
        {
            TurnManager.Instance.Replay();
        }
        
        
        public void QuitGame()
        {
            _onQuit.Invoke();
        }

        private void OnDestroy()
        {
            Instance = null;
		}

    }
}