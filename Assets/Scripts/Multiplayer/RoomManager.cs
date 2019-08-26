using System;
using System.Collections.Generic;
using Custom;
using PlayerIOClient;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Multiplayer
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private ServerManager serverManager;

        [SerializeField] private TextMeshProUGUI idText;
        [SerializeField] private ScrollRect messageScroll;
        [SerializeField] private TMP_InputField messageText;
        [SerializeField] private Transform messageParent;
        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private GameObject eventPrefab;
        [SerializeField] private GameObject playerPrefab;
        
        [SerializeField] private Transform indicatorsParent;
        [SerializeField] private GameObject playerIndicator;
        [SerializeField] private Color readyColor;
        [SerializeField] private Color notReadyColor;
        [SerializeField] private Color emptyColor;
        
        [SerializeField] private Toggle _readyToggle;
        [SerializeField] private Button _playButton;
        [SerializeField] private MultiplayerManager _multiplayerManager;
        [SerializeField] private UnityEvent _onPlay;

        private Connection _connection;
        private int _playerCount;
        private int _maxCount;
        private bool _isCreator;
        private string _lastPlayerId;
        private string _lastMessageTime;

        private void Awake()
        {
            _playButton.onClick.AddListener(() =>
            {
                if(_connection == null) return;
                _connection.Send("Play");
            });
        }

        private void OnEnable()
        {
            _readyToggle.isOn = false;
        }

        public void JoinRoom(string id, string password)
        {
            serverManager.JoinRoom(id, password, StartConnection);
        }

        public void CreateJoinRoom(string id, string type, string password, int maxPlayers, int defaultCards)
        {
            serverManager.CreateJoinRoom(id, type, true, password, maxPlayers, defaultCards, StartConnection);
        }

        private void StartConnection(Connection connection)
        {
            _connection = connection;
            _connection.OnMessage += OnMessage;
            _connection.OnDisconnect += OnDisconnect;
            Utils.SetUniqueChildVisible(transform);
        }

        private void OnMessage(object sender, Message message)
        {
            Debug.Log(sender + " - " + message);
            switch (message.Type)
            {
                case "Connection":
                    OnConnect(message);
                    break;
                case "Status":
                    OnStatusUpdate(message);
                    break;
                case "Ready":
                    AddEvent(string.Format("{0} is {1} ready", message.GetString(1), message.GetBoolean(0) ? "":"not"));
                    break;
                case "Play":
                    OnPlay(message);
                    break;
                case "PM":
                    AddMessage(message);
                    break;
                case "PlayerJoin":
                    AddUser(message);
                    break;
                case "PlayerLeft":
                    RemoveUser(message);
                    break;
                default:
                    _multiplayerManager.GotMessage(message);
                    break;
            }
        }

        private void OnPlay(Message message)
        {
            if (message.GetInt(0) == 0) _onPlay.Invoke();
            else _playButton.gameObject.SetActive(false);
        }

        private void OnStatusUpdate(Message message)
        {
            for (var i = _maxCount; i < indicatorsParent.childCount; i++) Destroy(indicatorsParent.GetChild(i).gameObject);
            for (var i = indicatorsParent.childCount; i < _maxCount; i++) Instantiate(playerIndicator, indicatorsParent).GetComponent<Image>().color = emptyColor;
            
            var isPlayable = true;
            var indicators = indicatorsParent.GetComponentsInChildren<Image>();
            for (uint i = 0; i < _maxCount; i++)
            {
                indicators[i].color = i < _playerCount ? (message.GetBoolean(i) ? readyColor: notReadyColor) : emptyColor;
                isPlayable = isPlayable && (i >= _playerCount || message.GetBoolean(i));
            }
            _playButton.gameObject.SetActive(_isCreator && isPlayable && _playerCount > 1);
        }

        private void OnConnect(Message message)
        {
            _playerCount = (int) message.Count - 2;
            _maxCount = message.GetInt(1);
            _isCreator = _playerCount == 1;
            idText.SetText(message.GetString(0));
            SetReady(false);
        }

        private void OnDisconnect(object sender, string message)
        {
            Debug.Log("Disconnected !");
            _playerCount = 0;
            _connection = null;
        }

        private void AddMessage(Message message)
        {
            var text = message.GetString(0);
            var playerId = message.GetString(1);
            var playerName = message.GetString(2);
            var time = DateTime.Now.ToString("HH:mm");
            if (!playerId.Equals(_lastPlayerId)) AddPlayer(playerName);

            var messagePanel = Instantiate(messagePrefab, messageParent).GetComponent<MessagePanel>();
            messagePanel.SetMessage(text, playerId.Equals(_lastPlayerId) && time.Equals(_lastMessageTime) ? "" : time);

            _lastMessageTime = time;
            _lastPlayerId = playerId;

            LayoutRebuilder.ForceRebuildLayoutImmediate(messageParent.GetComponent<RectTransform>());
            ScrollToBottom();
        }

        private void AddPlayer(string playerName)
        {
            var playerText = Instantiate(playerPrefab, messageParent).GetComponentInChildren<TextMeshProUGUI>();
            playerText.SetText(playerName);
        }

        private void AddEvent(string message)
        {
            var eventText = Instantiate(eventPrefab, messageParent).GetComponentInChildren<TextMeshProUGUI>();
            eventText.SetText(message);
            _lastMessageTime = "";
            _lastPlayerId = "";
            ScrollToBottom();
        }

        private void AddUser(Message message)
        {
            AddEvent(string.Format("{0} joined", message.GetString(0)));
            _playerCount++;
        }

        private void RemoveUser(Message message)
        {
            AddEvent(string.Format("{0} left", message.GetString(0)));
            _playerCount--;
        }
        
        private void ScrollToBottom()
        {
            if(!gameObject.activeInHierarchy) return;
            StopAllCoroutines();
            StartCoroutine(Utils.ScrollTo(messageScroll, .5f,
                AnimationCurve.Linear(0, 0, 1, 1),
                messageScroll.normalizedPosition, Vector2.zero));
        }

        public void SendMessage()
        {
            if (_connection == null || string.IsNullOrEmpty(messageText.text)) return;
            _connection.Send("PM", messageText.text);
            messageText.text = null;
        }
        
        public void SendMessage(Message message)
        {
            if (_connection != null) _connection.Send(message);
        }

        public void SetReady(bool isReady)
        {
            if (_connection == null) return;
            _connection.Send("Ready", isReady);
        }

        private void Disconnect()
        {
            if (_connection != null) _connection.Disconnect();
        }

        private void OnDestroy()
        {
            Disconnect();
        }
    }
}