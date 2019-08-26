using System;
using Cards;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class Card
    {
        public Sprite sprite;
        public CardType type;
        public int number;
        public string title;
        public string subtitle;
        public Acronym[] acronyms;
        
        public CardPanel Panel { get; set; }

        
        public Card(int type, int number, string title, string subtitle)
        {
            this.type = (CardType) type;
            this.number = number;
            this.title = title;
            this.subtitle = subtitle;
            var path = string.Format("Sprites/{0}_{1}", type+1, number+1);
            sprite = Resources.Load<Sprite>(path);
        }
    }
    
    [Serializable]
    public class Acronym
    {
        public string key;
        public string value;
        
        public Acronym(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
    
    [Serializable]
    public class CardPack
    {
        public Card[] cards;
    }

    public enum CardType
    {
        Type1,
        Type2,
        Type3,
        Type4
    }
}