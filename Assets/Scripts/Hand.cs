using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] float cardSpacing = 100f;
    private List<CardObj> handCards = new List<CardObj>();

    public void AddCard(CardObj card)
    {
        handCards.Add(card);
        ArrangeCards();
    }

    public void RemoveCard(CardObj card)
    {
        if (handCards.Contains(card))
        {
            handCards.Remove(card);
            ArrangeCards();
        }
    }

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
