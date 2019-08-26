using System.Collections.Generic;
using Custom;
using PlayerIOClient;
using UnityEngine;
using Services;
using TMPro;
using UnityEngine.Events;

namespace Multiplayer
{
    public class ServerManager : MonoBehaviour
    {
        private const string GameId = "herref-lxzdgdcef0ysqmnrq5aca";
        [SerializeField] private Transform _connectionPanel;
        [SerializeField] private TextMeshProUGUI _errorText;
        [SerializeField] private UnityEvent _onConnection;
        
        
        private Client Client{get; set;}
        private bool IsAuthenticating { get; set; }
        public bool IsOnline
        {
            get { return Client != null; } 
        }
        

        private void Start()
        {
            StartConnection();
        }

        public void StartConnection()
        {
            if(IsOnline || IsAuthenticating) return;
            Utils.SetUniqueChildVisible(_connectionPanel);
            Utils.SetUniqueChildVisible(_errorText.transform, false);
            IsAuthenticating = true;
            PlayerIO.Authenticate(GameId,"public",
                new Dictionary<string, string> {
                    { "userId", ServiceManager.User.Id},
                }, 
                new string[]{}, StartServer, error =>
                {
                    _errorText.SetText(error.ErrorCode.ToString());
                    Utils.SetUniqueChildVisible(_errorText.transform);
                    IsAuthenticating = false;
                });
        }


        private static void LogError(PlayerIOError error)
        {
            Debug.Log(error);
        }

        private void StartServer(Client client)
        {
            Client = client;
            IsAuthenticating = false;
            _onConnection.Invoke();
            Client.Multiplayer.DevelopmentServer = new ServerEndpoint("192.168.100.31", 8184);
        }

        public void JoinRoom(string roomId, string roomPassword, Callback<Connection> callback)
        {
            Client.Multiplayer.JoinRoom(roomId, 
                new Dictionary<string, string>
                {
                    {"name", ServiceManager.User.Name}, 
                    {"password", roomPassword},
                },
                callback,
                LogError);
        }

        public void CreateJoinRoom(string roomId, string roomType, bool isVisible, string password, int maxPlayers,
            int defaultCards, Callback<Connection> callback)
        {
            Client.Multiplayer.CreateJoinRoom(roomId, roomType, isVisible, 
                new Dictionary<string, string>
                {
                    {"maxPlayers", maxPlayers.ToString()},
                    {"password", password},
                    {"defaultCards", defaultCards.ToString()}
                }, new Dictionary<string, string>
                {
                    {"name", ServiceManager.User.Name},
                    {"password", password}
                },
                callback, 
                LogError);
        }

        public void GetRooms(Callback<RoomInfo[]> callback)
        {
            Client.Multiplayer.ListRooms(
                "Ronda", 
                new Dictionary<string, string>(), 
                -1, 
                0, 
                callback, 
                LogError);
        }
    }
}