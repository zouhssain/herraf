using UnityEngine;
using UnityEngine.SceneManagement;

namespace Custom
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }
        public const string PlayScene = "Play";

        private void Awake()
        {
            if(Instance != null) Destroy(Instance);
            Instance = this;
            
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
