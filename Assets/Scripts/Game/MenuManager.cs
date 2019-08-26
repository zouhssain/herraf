using Custom;
using Data;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private string[] links;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _soundToggle;

        private void Awake()
        {
            _musicToggle.onValueChanged.AddListener(value =>
            {
                Values.Music = value;
                if (SoundManager.Instance != null) SoundManager.Instance.OnMusicChanged(value);
            });
            _soundToggle.onValueChanged.AddListener(value => { Values.Sound = value; });

            _musicToggle.isOn = Values.Music;
            _soundToggle.isOn = Values.Sound;

            ServiceManager.Setup();
        }

        public void Play(bool isMultiplayer)
        {
            if (isMultiplayer)
            {
                ServiceManager.Login(success =>
                {
                    if(success) LoadGame(true);
                });
            }
            else LoadGame(false);
        }
        

        private void LoadGame(bool isMultiplayer)
        {
            Values.Multiplayer = isMultiplayer;
            SceneLoader.Instance.LoadScene(SceneLoader.PlayScene);
        }
        

        public void OpenLink(int index)
        {
            if (links.Length > index) Application.OpenURL(links[index]);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void MoveLeft(Transform t)
        {
            var p1 = new Vector3(-1000, 0, 0);
            var p2 = new Vector3(0, 0, 0);
            StartCoroutine(Utils.Translate(t, 0.2f, AnimationCurve.Linear(0, 0, 1, 1), p1, p2));
        }

        public void Display(Transform child)
        {
            Utils.SetUniqueChildVisible(child);
        }
    }
}