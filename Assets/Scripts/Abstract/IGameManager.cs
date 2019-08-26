using System;
using System.Collections;
using Cards;

namespace Abstract
{
    public interface IGameManager
    {
        IEnumerator AsyncMove(CardPanel newPanel, CardPanel oldPanel, Action<bool> callback);
        IEnumerator CardPlayed(CardPanel newPanel);
        IEnumerator CardsRemained(CardPanel[] remainedCards);
        void GameFinished(CardPlayer winner);
        void QuitGame();
    }
}