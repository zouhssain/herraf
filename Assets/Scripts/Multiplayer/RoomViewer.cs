using System.Collections;
using Custom;
using PlayerIOClient;
using UI;
using UnityEngine;

namespace Multiplayer
{
    public class RoomViewer : MonoBehaviour
    {
        [SerializeField] private ServerManager _serverManager;
        [SerializeField] private GameObject _roomPrefab;
        [SerializeField] private Transform _roomsParent;
        [SerializeField] private RoomConnector _roomConnector;

        private void OnEnable()
        {
            StartCoroutine(WaitForServer());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator WaitForServer()
        {
            if(!_serverManager.IsOnline) yield return new WaitUntil(() => _serverManager.IsOnline);
            Refresh();
        }

        public void Refresh()
        {
            Utils.SetUniqueChildVisible(_roomsParent, false);
            _serverManager.GetRooms(LoadRooms);
        }

        private void LoadRooms(RoomInfo[] rooms)
        {
            for (var i = rooms.Length; i < _roomsParent.childCount; i++) Destroy(_roomsParent.GetChild(i).gameObject);
            for (var i = _roomsParent.childCount; i < rooms.Length; i++) Instantiate(_roomPrefab, _roomsParent);

            var roomPanels = _roomsParent.GetComponentsInChildren<RoomPanel>();
            for (var i = 0; i < rooms.Length; i++) roomPanels[i].SetRoomInfo(rooms[i], JoinRoom);
            Utils.SetUniqueChildVisible(_roomsParent);
        }

        private void JoinRoom(string id, bool hasPassword)
        {
            if(hasPassword) _roomConnector.Display(id);
            else _roomConnector.JoinRoom(id);
        }
    }
}