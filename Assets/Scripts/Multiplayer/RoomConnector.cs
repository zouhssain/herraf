using Custom;
using TMPro;
using UnityEngine;

namespace Multiplayer
{
    public class RoomConnector : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _roomIdText;
        [SerializeField] private TMP_InputField _roomPasswordText;
        [SerializeField] private RoomManager _roomManager;
        
        public void JoinRoom()
        {
            _roomManager.JoinRoom(_roomIdText.text, _roomPasswordText.text);
        }
        public void JoinRoom(string id)
        {
            _roomManager.JoinRoom(id, "");
        }

        public void Display(string id)
        {
            _roomIdText.text = id;
            Utils.SetUniqueChildVisible(transform);
        }
    }
}