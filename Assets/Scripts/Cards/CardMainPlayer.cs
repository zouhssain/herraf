using Game;
using UnityEngine;
using UnityEngine.Events;

namespace Cards
{
    public class CardMainPlayer : CardPlayer
    {
        
        public void Quit()
        {
            GameManager.Instance.QuitGame();
        }
    }
}