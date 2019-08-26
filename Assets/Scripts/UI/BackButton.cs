using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class BackButton : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onClick;

        private void Update()
        {
            if (!Input.GetKeyUp(KeyCode.Escape)) return;
            Click();
        }

        public void Click()
        {
            _onClick.Invoke();
        }
    }
}