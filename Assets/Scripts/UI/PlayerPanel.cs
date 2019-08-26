using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerPanel : MonoBehaviour
    {
        [SerializeField] private Image avatar;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;

        public void SetPlayer(CardPlayer cardPlayer)
        {
            avatar.sprite = cardPlayer.Avatar;
            nameText.SetText(cardPlayer.Name);
            scoreText.SetText(cardPlayer.Score.ToString());
        }
    }
}