using System;
using System.Collections;
using System.Linq;
using Custom;
using Data;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class QuizManager : MonoBehaviour
    {
        private const char C = '●';

        [SerializeField] private CardData data;
        [SerializeField] private GameObject answerPrefab;
        [SerializeField] private Transform answersParent;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI indicatorText;
        [SerializeField] private Slider timeSlider;
        [SerializeField] private float answerDuration;
        [SerializeField] private float maxDuration;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color validColor;
        [SerializeField] private Color invalidColor;
        [SerializeField] private Color accentColor;
        private bool _isWaiting;
        private bool _isValidAnswer;

        public IEnumerator StartQuiz(Card card, Action<bool> onFinish)
        {
            titleText.SetText(card.title);
            subtitleText.SetText(card.subtitle);
            Utils.SetUniqueChildVisible(transform);

            var subtitle = card.subtitle.ToLowerInvariant();
            var duration = answerDuration * card.acronyms.Length < maxDuration ? answerDuration : maxDuration / card.acronyms.Length;
            
            for (var i = 0; i < card.acronyms.Length; i++)
            {
                indicatorText.SetText(string.Format("<color #{0}>{1}</color>{2}",
                    ColorUtility.ToHtmlStringRGB(validColor),
                    new string(C, i),
                    new string(C, card.acronyms.Length - i)));

                var key = card.acronyms[i].key.ToLowerInvariant();
                var keyUpper = key.ToUpperInvariant();
                var index = subtitle.IndexOf(key, StringComparison.Ordinal);
                if (index >= 0)
                {
                    subtitle = subtitle.Remove(index, key.Length).Insert(index, keyUpper);
                    subtitleText.SetText(subtitle
                        .Remove(index, key.Length)
                        .Insert(index, string.Format("<color #{0}>{1}</color>", 
                            ColorUtility.ToHtmlStringRGB(accentColor), key)
                    ));
                }
                
                yield return ValidateAcronym(card.acronyms[i], duration);
                if (_isValidAnswer) continue;
                onFinish.Invoke(false);
                Utils.SetUniqueChildVisible(transform, false);
                yield break;
            }

            Utils.SetUniqueChildVisible(transform, false);
            onFinish(true);
        }

        private IEnumerator ValidateAcronym(Acronym acronym, float duration)
        {
            var answers = data.cards.SelectMany(card => card.acronyms)
                .Where(a => a != acronym && a.key.Equals(acronym.key))
                .Select(a => a.value)
                .Take(3).ToList();
            answers.Add(acronym.value);
            answers = answers.OrderBy(a => UnityEngine.Random.value).ToList();
            
            for (var i = answers.Count; i < answersParent.childCount; i++)
                Destroy(answersParent.GetChild(i).gameObject);
            for (var i = answersParent.childCount; i < answers.Count; i++) Instantiate(answerPrefab, answersParent);
            yield return null;

            var answerPanels = answersParent.GetComponentsInChildren<AnswerPanel>();
            for (var i = 0; i < answerPanels.Length; i++)
            {
                answerPanels[i].SetColor(defaultColor);
                answerPanels[i].SetAnswer(answers[i], acronym.value.Equals(answers[i]), OnAnswerClick);
            }

            _isWaiting = true;
            _isValidAnswer = false;
            timeSlider.value = 1;
            var time = 0f;
            while (_isWaiting && time < duration)
            {
                time += Time.deltaTime;
                timeSlider.value = 1 - time / duration;
                yield return null;
            }

            _isWaiting = false;
            yield return new WaitForSeconds(.5f);
            timeSlider.value = 1;
        }

        private void OnAnswerClick(AnswerPanel answerPanel, bool isValid)
        {
            if (!_isWaiting) return;
            _isWaiting = false;
            _isValidAnswer = isValid;
            answerPanel.SetColor(isValid ? validColor : invalidColor);
        }
    }
}