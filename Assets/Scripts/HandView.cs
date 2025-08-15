using System.Collections.Generic;
using UnityEngine;

public interface IHandView
{
    void AddCard(CardView card);
    void RemoveCard(CardView card);
    void ArrangeCards(List<CardView> cards);
}

public sealed class HandView : MonoBehaviour, IHandView
{
    [SerializeField] float _cardSpacing;

    public void AddCard(CardView card)
    {
        card.transform.SetParent(transform, false);
    }

    public void RemoveCard(CardView card)
    {
        card.gameObject.SetActive(false);
    }

    public void ArrangeCards(List<CardView> cards)
    {
        float totalWidth = (cards.Count - 1) * _cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < cards.Count; i++)
        {
            Vector2 targetPos = new Vector2(startX + i * _cardSpacing, 0f);
            cards[i].GetComponent<RectTransform>().anchoredPosition = targetPos;
        }
    }
}
