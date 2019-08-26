using System.Linq;
using Data;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public abstract class CardPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject indicator;
        [SerializeField] private Image avatar;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] protected CardStock cardStock;
        [SerializeField] protected Transform cardPanelsParent;
        [SerializeField] protected bool cardRevealed;
        
        public int Score { get; private set; }
        public string Id { get; set; }
        public string Name
        {
            get{ return nameText.text;}
            set { nameText.text = value; }
        }
        public Vector3 Pivot { get{ return cardStock.transform.position;} }
        public Sprite Avatar { get{ return avatar.sprite;} }
        public bool CardRevealed { get{ return cardRevealed;} }
        public Transform Hand { get{ return cardPanelsParent;} }


        public void RemoveCard(CardPanel cardPanel)
        {
            cardStock.Remove(cardPanel.Card);
        }

        public void Deal(params CardPanel[] cardPanels)
        {
            foreach (var cardPanel in cardPanels) cardPanel.Details.Player = this;
            Debug.Log(cardPanels[0].Details.Player.Name + "  " + string.Join(" ", cardPanels.Select(panel => panel.Card.number.ToString()).ToArray()));
            cardStock.AddRange(cardPanels.Select(panel => panel.Card));
        }

        public virtual void SetFocused(bool isFocused)
        {
            indicator.SetActive(isFocused);
        }

        public void PlayCard(CardPanel cardPanel)
        {            
            TurnManager.Instance.PlayCard(cardPanel);
        }

        public void ReportScore(int score)
        {
            SetScore(Score += score);
        }
        
        public void SetScore(int score)
        {
            Score = score;
            scoreText.SetText(score.ToString());
        }

        public bool IsHandEmpty()
        {
            return cardStock.Count == 0;
        }

    }
}