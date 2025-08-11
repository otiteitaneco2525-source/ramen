using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] float cardSpacing = 100f; // カード同士の間隔
    private List<CardObj> handCards = new List<CardObj>();

    /// <summary>
    /// Deckから手札にカードを追加して並べ直す
    /// </summary>
    public void AddCard(CardObj card)
    {
        handCards.Add(card);
        ArrangeCards();
    }

    /// <summary>
    /// 手札からカードを取り除いて並べ直す
    /// </summary>
    public void RemoveCard(CardObj card)
    {
        if (handCards.Contains(card))
        {
            handCards.Remove(card);
            ArrangeCards();
        }
    }

    /// <summary>
    /// 手札を横に並べる（UI用）
    /// </summary>
    private void ArrangeCards()
    {
        float totalWidth = (handCards.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < handCards.Count; i++)
        {
            Vector2 targetPos = new Vector2(startX + i * cardSpacing, 0f);
            handCards[i].GetComponent<RectTransform>().anchoredPosition = targetPos;
        }
    }
}
