using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer
{
    public class RoomCreator : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _id;
        [SerializeField] private string _type;
        [SerializeField] private TMP_InputField _password;
        [SerializeField] private SliderText _playersSlider;
        [SerializeField] private int _defaultCards;
        [SerializeField] private RoomManager _roomManager;


        public void CreateJoinRoom()
        {
            _roomManager.CreateJoinRoom(_id.text, _type, _password.text, Mathf.RoundToInt(_playersSlider.Value), _defaultCards);
        }

    }
}