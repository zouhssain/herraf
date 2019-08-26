using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AnswerPanel : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;

        public void SetAnswer(string answerText, bool isValid, Action<AnswerPanel, bool> onAnswerClick)
        {
            text.SetText(answerText);
            button.onClick.AddListener(() => onAnswerClick(this, isValid));
        }

        public void SetColor(Color color)
        {
            button.targetGraphic.color = color;
        }
    }
}