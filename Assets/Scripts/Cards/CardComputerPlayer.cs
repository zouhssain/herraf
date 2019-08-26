using System.Collections;
using System.Linq;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cards
{
    public class CardComputerPlayer : CardExtraPlayer
    {
        public override void SetFocused(bool isFocused)
        {
            base.SetFocused(isFocused);
            if (isFocused) StartCoroutine(Play());
        }

        public void SetName(string computerName)
        {
            Name = computerName;
        }

        private IEnumerator Play()
        {
            yield return new WaitForSeconds(Random.Range(.2f, .5f));
            var playedCards = CardManager.Instance.GetCards();
            var matchCard = GetCardPanels()
                .FirstOrDefault(card => playedCards.Any(panel => panel.Card.number == card.Card.number));
            if (matchCard) PlayCard(matchCard);
            else PlayRandomCard();
        }
        
        
    }
}