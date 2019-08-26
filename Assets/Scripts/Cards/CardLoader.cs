using System.Collections.Generic;
using System.IO;
using Data;
using UnityEditor;
using UnityEngine;

namespace Cards
{
    public class CardLoader : MonoBehaviour
    {
        [SerializeField] private string fileName;
        [SerializeField] private string assetName;
        [SerializeField] private CardData data;
        [SerializeField] private CardPanel previewCard;
        [SerializeField] private int previewIndex;

        public void NextCard()
        {
            previewIndex++;
            RefreshCard();
        }
        
        public void PreviousCard()
        {
            previewIndex--;
            RefreshCard();
        }

        public void RefreshCard()
        {
            previewIndex = (previewIndex + data.cards.Length) % data.cards.Length;
            previewCard.SetCard(data.cards[previewIndex]);
        }

        public void GenerateData()
        {
            Generate();
        }

        private void Generate()
        {
            var lines = File.ReadAllLines(string.Format("{0}\\{1}{2}", Application.dataPath, fileName, ".txt"));
            var cards = new List<Card>(40);
            var type = 1;
            for (var i = 0; i < lines.Length; i++)
            {
                if (!lines[i].StartsWith(type + "/")) continue;

                var index = 1;
                for (var j = i + 1; j < lines.Length; j++)
                {
                    if (string.IsNullOrEmpty(lines[j])) continue;
                    if (lines[j].StartsWith(index + "."))
                    {
                        var k = lines[j].LastIndexOf(':');
                        cards.Add(new Card(type - 1,
                            index - 1,
                            lines[j].Substring(index < 10 ? 2 : 3, k - (index < 10 ? 2 : 3)).Trim(),
                            lines[j].Substring(k + 1).Trim()));
                        index++;
                    }

                    else if (lines[j].StartsWith(string.Format("{0}/", type + 1))) break;
                    else
                    {
                        if (cards[cards.Count - 1].acronyms == null) cards[cards.Count - 1].acronyms = new Acronym[0];
                        cards[cards.Count - 1].acronyms = new List<Acronym>(cards[cards.Count - 1].acronyms) 
                            {GetAcronym(lines[j].Trim())}.ToArray();
                    }
                }
                type++;
            }
            OutputResource(cards);
        }

        private void OutputResource(List<Card> cards)
        {
            var cardData = ScriptableObject.CreateInstance<CardData>();
            cardData.cards = cards.ToArray();
            cardData.name = fileName;
            #if UNITY_EDITOR
            AssetDatabase.CreateAsset(cardData, string.Format("Assets\\Resources\\{0}.asset", assetName));
            #endif
        }
        
        private static Acronym GetAcronym(string subtitle)
        {
            string key, value;
            if (subtitle.Contains("::"))
            {
                var index = subtitle.IndexOf(':');
                key = subtitle.Substring(0, index).Trim();
                value = subtitle.Substring(index + 2).Trim();
            }
            else
            {
                value = subtitle;
                key = subtitle[0].ToString();
            }
            Debug.Log(key + " " + value);
            return new Acronym(key, value);
        }
    }
}