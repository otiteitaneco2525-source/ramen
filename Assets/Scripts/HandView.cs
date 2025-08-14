using System.Collections.Generic;
using UnityEngine;

public interface IHandView
{
    void AddCard(CardView card);
    void RemoveCard(CardView card);
}

public sealed class HandView : MonoBehaviour, IHandView
{
    [SerializeField] float cardSpacing = 100f;
    private List<CardView> _handCards = new List<CardView>();

    public void AddCard(CardView card)
    {
        card.transform.SetParent(transform, false);
        _handCards.Add(card);
        ArrangeCards();
    }

    public void RemoveCard(CardView card)
    {
        if (_handCards.Contains(card))
        {
            _handCards.Remove(card);
            ArrangeCards();
        }
    }

    private void ArrangeCards()
    {
        float totalWidth = (_handCards.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < _handCards.Count; i++)
        {
            Vector2 targetPos = new Vector2(startX + i * cardSpacing, 0f);
            _handCards[i].GetComponent<RectTransform>().anchoredPosition = targetPos;
        }
    }
}
