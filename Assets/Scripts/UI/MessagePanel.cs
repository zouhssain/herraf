using TMPro;
using UnityEngine;

namespace UI
{
    public class MessagePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI timeText;

        public void SetMessage(string message, string time)
        {
            messageText.SetText(message);
            timeText.SetText(time);
        }
    }
}