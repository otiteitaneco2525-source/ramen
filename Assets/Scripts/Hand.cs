using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] float cardSpacing = 100f; // カード間の距離
    private List<CardObj> handCards = new List<CardObj>();

    /// <summary>
    /// Deckから手札にカードを追加して整列
    /// </summary>
    public void AddCard(CardObj card)
    {
        handCards.Add(card);
        ArrangeCards();
    }

    /// <summary>
    /// 手札からカードを削除して整列
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
    /// 手札を整列する（UI用）
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
