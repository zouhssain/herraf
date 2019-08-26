using Cards;
using UnityEngine;

namespace Game
{
    public class ColorManager : MonoBehaviour
    {
        public static ColorManager Instance { get; private set; }

        [SerializeField] private Color defaultColor;
        [SerializeField] private Color focusedColor;
        [SerializeField] private Color playedColor;

        private void Start()
        {
            Instance = this;
        }

        public void SetCardFocused(params CardPanel[] cardPanels)
        {
            SetCardColor(focusedColor, cardPanels);
        }

        public void SetCardPlayed(params CardPanel[] cardPanels)
        {
            SetCardColor(playedColor, cardPanels);
        }

        public void SetCardDefault(params CardPanel[] cardPanels)
        {
            SetCardColor(defaultColor, cardPanels);
        }

        private void SetCardColor(Color color, params CardPanel[] cardPanels)
        {
            foreach (var cardPanel in cardPanels) cardPanel.SetBackground(color);
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}