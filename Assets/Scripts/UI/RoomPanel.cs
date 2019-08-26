using System;
using PlayerIOClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RoomPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _idText;
        [SerializeField] private TextMeshProUGUI _creatorText;
        [SerializeField] private TextMeshProUGUI _playersText;
        [SerializeField] private GameObject _lockPanel;

        public void SetRoomInfo(RoomInfo roomInfo, Action<string, bool> callback)
        {
            var button = GetComponent<Button>();

            _idText.SetText(roomInfo.Id);
            if (roomInfo.RoomData.ContainsKey("creator")) _creatorText.SetText(roomInfo.RoomData["creator"]);
            if (roomInfo.RoomData.ContainsKey("maxPlayers"))
            {
                _playersText.SetText(string.Format("{0}/{1}", roomInfo.OnlineUsers, roomInfo.RoomData["maxPlayers"]));
                var maxPlayers = 0;
                if (int.TryParse(roomInfo.RoomData["maxPlayers"], out maxPlayers))
                    button.interactable = roomInfo.OnlineUsers < maxPlayers;
            }

            _lockPanel.SetActive(!string.IsNullOrEmpty(roomInfo.RoomData["password"]));
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => callback(roomInfo.Id, _lockPanel.activeSelf));
        }
    }
}