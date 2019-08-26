using UnityEngine;

namespace Data
{
    public static class Values
    {
        private const string MusicKey = "Music";
        private const string SoundKey = "Sound";
        private const string MultiplayerKey = "Multiplayer";
        private const string RateKey = "Rate";
        private const string NameKey = "Name";
        private const string FirstNameKey = "FirstName";
        private const string LastNameKey = "LastName";
        private const string EmailKey = "Email";


        public static string Name
        {
            get { return PlayerPrefs.GetString(NameKey, "P" + Random.Range(1000, 10000)); }
            set { PlayerPrefs.SetString(NameKey, value); }
        }
        public static string FirstName
        {
            get { return PlayerPrefs.GetString(FirstNameKey, "Player"); }
            set { PlayerPrefs.SetString(FirstNameKey, value); }
        }
        public static string LastName
        {
            get { return PlayerPrefs.GetString(LastNameKey, "Player"); }
            set { PlayerPrefs.SetString(LastNameKey, value); }
        }
        public static string Email
        {
            get { return PlayerPrefs.GetString(EmailKey, ""); }
            set { PlayerPrefs.SetString(EmailKey, value); }
        }

        public static bool Music
        {
            get { return PlayerPrefs.GetInt(MusicKey, 1) == 1; }
            set { PlayerPrefs.SetInt(MusicKey, value ? 1 : 0); }
        }
        
        public static bool Sound
        {
            get { return PlayerPrefs.GetInt(SoundKey, 1) == 1; }
            set { PlayerPrefs.SetInt(SoundKey, value ? 1 : 0); }
        }
        
        public static bool Multiplayer
        {
            get { return PlayerPrefs.GetInt(MultiplayerKey, 0) == 1; }
            set { PlayerPrefs.SetInt(MultiplayerKey, value ? 1 : 0); }
        }
        
        public static int Rate
        {
            get { return PlayerPrefs.GetInt(RateKey, -1); }
            set { PlayerPrefs.SetInt(RateKey, value); }
        }
    }
}