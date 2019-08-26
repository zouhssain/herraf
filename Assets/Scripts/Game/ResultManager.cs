using System.Collections.Generic;
using Cards;
using Custom;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class ResultManager : MonoBehaviour
    {
        [SerializeField] private Image resultImage;
        [SerializeField] private Image resultBackImage;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Color winColor;
        [SerializeField] private Color winBackColor;
        [SerializeField] private Color loseColor;
        [SerializeField] private Color loseBackColor;
        [SerializeField] private string winText;
        [SerializeField] private string loseText;
        [SerializeField] private Transform players;
        [SerializeField] private UnityEvent _onResult;


        public void SetResult(List<CardPlayer> cardPlayers)
        {
            SetWin(cardPlayers[0] is CardMainPlayer);
            var playerPanels = players.GetComponentsInChildren<PlayerPanel>();
            for (var i = 0; i < playerPanels.Length; i++)
            {
                playerPanels[i].gameObject.SetActive(true);
                if (cardPlayers.Count > i) playerPanels[i].SetPlayer(cardPlayers[i]);
                else playerPanels[i].gameObject.SetActive(false);
            }
            _onResult.Invoke();
        }

        private void SetWin(bool isWin)
        {
            SoundManager.Instance.PlayClip(isWin
                ? SoundManager.Instance.Win
                : SoundManager.Instance.Lose);
            resultImage.color = isWin ? winColor : loseColor;
            resultBackImage.color = isWin ? winBackColor : loseBackColor;
            resultText.SetText(isWin ? winText : loseText); 
        }
    }
}