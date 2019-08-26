using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SliderText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Slider _slider;
        public float Value
        {
            get { return _slider.value; }
        }

        protected void Start()
        {
            _slider.onValueChanged.AddListener(number =>
            {
                _text.SetText(number.ToString("0"));
            });
        }
    }
}