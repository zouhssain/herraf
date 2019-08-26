using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class LongClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private UnityEvent longClick;

        public void OnPointerDown(PointerEventData eventData)
        {
            StartCoroutine(Wait(.3f));
        }

        private IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            longClick.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopAllCoroutines();
        }
    }
}