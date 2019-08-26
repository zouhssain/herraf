using System.Collections.Generic;

namespace Cards
{
    public class CardExtraPlayer : CardPlayer
    {
        protected void PlayRandomCard()
        {
            var cardPanel = cardPanelsParent.GetComponentInChildren<CardPanel>();
            if (cardPanel) PlayCard(cardPanel);
        }

        protected IEnumerable<CardPanel> GetCardPanels()
        {
            return cardPanelsParent.GetComponentsInChildren<CardPanel>();
        }
    }
}