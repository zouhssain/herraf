using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    [CreateAssetMenu(fileName = "CardData", menuName = "CardData", order = 1)]
    public class CardData : ScriptableObject
    {
        public Card[] cards;
    }
}