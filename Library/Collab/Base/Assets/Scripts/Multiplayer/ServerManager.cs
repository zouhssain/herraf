using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;

namespace Multiplayer
{
    public class ServerManager : MonoBehaviour
    {
        private const string GameId = "herref-lxzdgdcef0ysqmnrq5aca";
        private Client Client{get; set;}

        private void Start()
        {
            PlayerIO.Authenticate(GameId,"public",
                new Dictionary<string, string> {
                    // { "userId", Social.localUser.id },
                    { "userId", Random.Range(0, 10000).ToString() },
                }, 
                new string[]{},
                client =>
                {
                    Debug.Log("Successful authentication : ");
                    Client = client;
                    StartServer();
                }, 
                errorCallback: Debug.LogError);
        }
    
        private void StartServer()
        {
            Client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);
            
        }

        public void JoinRoom(string roomId, string roomPassword, Callback<Connection> callback)
        {
            Debug.Log("JoinRoom : " + roomId + " " + roomPassword);
            Client.Multiplayer.JoinRoom(roomId, 
                new Dictionary<string, string>
                {
                    {"password", roomPassword},
                },
                callback,
                errorCallback: Debug.LogError);
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
                    {"password", password}
                },
                callback, 
                Debug.LogError);
        }
    }
}