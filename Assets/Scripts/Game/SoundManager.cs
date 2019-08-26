using Data;
using UnityEngine;

namespace Game
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        [SerializeField] private AudioClip cardPlayClip;
        [SerializeField] private AudioClip cardFrictionClip;
        [SerializeField] private AudioClip cardStockClip;
        [SerializeField] private AudioClip winClip;
        [SerializeField] private AudioClip loseClip;
		[SerializeField] private AudioClip drawClip;

		public AudioClip CardPlay { get { return cardPlayClip; } }
        public AudioClip CardFriction { get { return cardFrictionClip; } }
        public AudioClip CardStock { get { return cardStockClip; } }
        public AudioClip Win { get { return winClip; } }
        public AudioClip Lose { get { return loseClip; } }
		public AudioClip Draw { get { return drawClip; } }

		private void Awake()
        {
            if(Instance != null) Destroy(Instance.gameObject);
            DontDestroyOnLoad(Instance = this);
            OnMusicChanged(Values.Music);
        }

        public void OnMusicChanged(bool isOn)
        {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null) audioSource.mute = !isOn;
        }

        public void PlayClip(AudioClip clip)
        {
            if (!Values.Sound || !clip) return;
            var cam = Camera.main;
            var position = cam != null ? cam.transform.position : Vector3.zero;
            AudioSource.PlayClipAtPoint(clip, position);
        }
        
        private void OnDestroy()
        {
            if(Instance == this) Instance = null;
        }
    }
}